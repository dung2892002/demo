//hoan thien cac the html con thieu va xoa cac the thua
export function fixAndCompleteHTML(input: string): string {
  const tagPattern = /<\/?([a-zA-Z0-9]+)[^>]*?>/g
  const selfClosingTags = ['br', 'img', 'hr', 'input', 'meta', 'link']

  // 0. Tự động hoàn chỉnh thẻ HTML chưa đóng ở cuối (ví dụ </strong)
  input = input.replace(/(<\/?[a-zA-Z0-9]+[^>]*)(?=$)/, '$1>')

  // 1. Xoá các thẻ đóng và <br> ở đầu chuỗi
  const cleanedInput = input.replace(/^(\s*<\/[^>]+>|\s*<br\s*\/?>)+/gi, '')

  const openStack: string[] = []
  const missingOpen: string[] = []

  const matches = cleanedInput.matchAll(tagPattern)

  for (const match of matches) {
    const fullTag = match[0]
    const tagName = match[1].toLowerCase()

    const isClosing = fullTag.startsWith('</')
    const isSelfClosing = fullTag.endsWith('/>') || selfClosingTags.includes(tagName)

    if (isSelfClosing) continue

    if (isClosing) {
      const openIndex = openStack.lastIndexOf(tagName)
      if (openIndex !== -1) {
        openStack.splice(openIndex, 1)
      } else {
        missingOpen.unshift(`<${tagName}>`)
      }
    } else {
      openStack.push(tagName)
    }
  }

  const missingClose = openStack.reverse().map((tag) => `</${tag}>`)

  return missingOpen.join('') + cleanedInput + missingClose.join('')
}

//hoan thien cac the html cho phan text duoc chon
export function fixHTMLSubstring(input: string, startIndex: number, endIndex: number): string {
  const selfClosingTags = ['br', 'img', 'hr', 'input', 'meta', 'link']
  const openingTagsStack: string[] = []

  // 1. Phân tích từ đầu đến startIndex → tìm các thẻ đang mở
  const beforeSlice = input.slice(0, startIndex)
  const tagPattern = /<\/?([a-zA-Z0-9]+)[^>]*?>/g
  let match: RegExpExecArray | null

  while ((match = tagPattern.exec(beforeSlice)) !== null) {
    const fullTag = match[0]
    const tagName = match[1].toLowerCase()
    const isClosing = fullTag.startsWith('</')
    const isSelfClosing = selfClosingTags.includes(tagName) || fullTag.endsWith('/>')

    if (isSelfClosing) continue

    if (isClosing) {
      const lastOpen = openingTagsStack.lastIndexOf(tagName)
      if (lastOpen !== -1) {
        openingTagsStack.splice(lastOpen, 1)
      }
    } else {
      openingTagsStack.push(tagName)
    }
  }

  // 2. Lấy đoạn cắt
  const extracted = input.slice(startIndex, endIndex)

  // 3. Phân tích đoạn trích để tìm các thẻ mở chưa đóng (và đã đóng)
  const unclosedTagsInExtracted: string[] = []
  const closedTagsInExtracted: string[] = []
  tagPattern.lastIndex = 0

  while ((match = tagPattern.exec(extracted)) !== null) {
    const fullTag = match[0]
    const tagName = match[1].toLowerCase()
    const isClosing = fullTag.startsWith('</')
    const isSelfClosing = selfClosingTags.includes(tagName) || fullTag.endsWith('/>')

    if (isSelfClosing) continue

    if (isClosing) {
      const lastOpen = unclosedTagsInExtracted.lastIndexOf(tagName)
      if (lastOpen !== -1) {
        unclosedTagsInExtracted.splice(lastOpen, 1)
      } else {
        closedTagsInExtracted.push(tagName)
      }
    } else {
      unclosedTagsInExtracted.push(tagName)
    }
  }

  // 4. Tạo các thẻ mở + đóng còn thiếu
  const openingHTML = openingTagsStack.map((tag) => `<${tag}>`).join('')

  // Chỉ đóng lại các thẻ còn mở mà chưa được đóng trong đoạn trích
  const closingFromBefore = openingTagsStack
    .filter((tag) => !closedTagsInExtracted.includes(tag))
    .reverse()
    .map((tag) => `</${tag}>`)
    .join('')

  const closingFromExtracted = unclosedTagsInExtracted
    .reverse()
    .map((tag) => `</${tag}>`)
    .join('')

  return openingHTML + extracted + closingFromExtracted + closingFromBefore
}

//lay do dai trong markdown
export function getSelectedTextLengthInMarkdown(
  indexInMarkdown: number,
  fullMarkdown: string,
  selectedText: string,
): number {
  if (fullMarkdown === null) return 0

  const length = selectedText.length - (selectedText.match(/\n/g) || []).length
  let textLength = 0
  let check = false
  let countText = 0
  for (let i = indexInMarkdown; i < fullMarkdown.length; i++) {
    textLength = textLength + 1
    if (fullMarkdown[i] === '<') {
      check = true
      continue
    }
    if (fullMarkdown[i] === '>') {
      check = false
      continue
    }
    if (!check) countText = countText + 1
    if (countText === length) {
      break
    }
  }
  return textLength
}

//lay vi tri trong markdown
export function calculateIndexInMarkdown(
  startIndex: number,
  selectedText: string,
  fullText: string,
  fullMarkdown: string,
): number {
  if (fullMarkdown === null) return 0

  let indexInMarkdown = 0
  let count = startIndex
  let checkCount = true

  for (let i = 0; i < fullMarkdown.length; i++) {
    if (count === 0) {
      break
    }
    if (fullMarkdown[i] === '<') {
      checkCount = false
      continue
    }
    if (fullMarkdown[i] === '>') {
      checkCount = true
      continue
    }
    if (checkCount) {
      count--
      indexInMarkdown = i
    }
  }

  const totalNewlines = (selectedText.match(/\n/g) || []).length
  if (fullText[startIndex + selectedText.length] === '\n' && startIndex) {
    indexInMarkdown = indexInMarkdown + 1
  }

  if (startIndex != 0) indexInMarkdown = indexInMarkdown + totalNewlines

  return indexInMarkdown
}

//lay vi tri chinh xac cua doan text duoc chon trong text goc
export function calculateExactIndex(fullText: string, selectedText: string, range: Range): number {
  // Lấy vị trí của đoạn bôi đen trong DOM
  const container = range.startContainer
  const offset = range.startOffset

  // Tìm vị trí của đoạn bôi đen trong nội dung gốc
  let currentIndex = 0
  let matchIndex = -1

  while ((matchIndex = fullText.indexOf(selectedText, currentIndex)) !== -1) {
    // Kiểm tra xem đoạn tìm thấy có khớp với vị trí trong DOM không
    const textBeforeMatch = fullText.substring(0, matchIndex)
    const domTextBeforeMatch = container.textContent?.substring(0, offset) || ''

    if (textBeforeMatch.endsWith(domTextBeforeMatch)) {
      return matchIndex // Trả về vị trí chính xác
    }

    currentIndex = matchIndex + 1 // Tiếp tục tìm kiếm
  }

  return -1 // Không tìm thấy
}

//xoa cac the html
export function removeHtmlTags(str: string): string {
  return str
    .replace(/<br\s*\/?>/gi, '\n') // Thay thế thẻ <br> bằng xuống dòng
    .replace(/<\/?[^>]+(>|$)/g, '') // Xóa tất cả các thẻ HTML khác
}
