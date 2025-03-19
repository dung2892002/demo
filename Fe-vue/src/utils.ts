import { DocumentType } from './entities/Document'

export function formatDate(inputDate: string) {
  const date = new Date(inputDate)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

export function getSrcIconDocument(type: DocumentType) {
  switch (type) {
    case DocumentType.Folder:
      return 'folder.png'
    case DocumentType.Word:
      return 'word.png'
    case DocumentType.Pdf:
      return 'pdf.png'
    case DocumentType.Excel:
      return 'excel.png'
    case DocumentType.Ppt:
      return 'ppt.png'
    case DocumentType.Image:
      return 'image.png'
    default:
      return 'file.png'
  }
}

export function formatDateForm(date: string) {
  const d = new Date(date)
  let month = '' + (d.getMonth() + 1)
  let day = '' + d.getDate()
  const year = d.getFullYear()

  if (month.length < 2) month = '0' + month
  if (day.length < 2) day = '0' + day

  return [year, month, day].join('-')
}

// markdownParser.ts

/**
 * Chuyển đổi bảng Markdown sang HTML Table
 * @param markdown - Chuỗi Markdown chứa bảng
 * @returns Chuỗi HTML Table
 */
export function markdownToTable(markdown: string): string {
  const lines = markdown
    .split('\n')
    .map((line) => line.trim())
    .filter((line) => line)
  let tableHTML = `<table style="width:100%; text-align:center; border-collapse:collapse; border: 1px solid black;">`

  lines.forEach((line) => {
    if (line.includes('|')) {
      const columns = line.split('|').map((col) => col.trim())
      tableHTML += `<tr>`
      columns.forEach((col) => {
        if (col.startsWith('**') && col.endsWith('**')) {
          col = `<strong>${col.replace(/\*\*/g, '')}</strong>`
        }
        if (col.startsWith('*') && col.endsWith('*')) {
          col = `<em>${col.replace(/\*/g, '')}</em>`
        }
        tableHTML += `<td style="border: 1px solid black; padding: 5px;">${col}</td>`
      })
      tableHTML += `</tr>`
    }
  })

  tableHTML += `</table>`
  return tableHTML
}

/**
 * Chuyển đổi một chuỗi Markdown hỗn hợp thành HTML
 * @param markdownText - Chuỗi Markdown chứa bảng và văn bản thường
 * @returns Chuỗi HTML đầu ra
 */
export function convertMarkdownToHTML(markdownText: string): string {
  const lines = markdownText.split('\n').map((line) => line.trim())
  let htmlOutput = ''
  let tableBuffer: string[] = []
  let isTable = false

  lines.forEach((line) => {
    if (line.includes('|')) {
      // Nếu phát hiện dấu '|' thì có thể là bảng
      isTable = true
      tableBuffer.push(line)
    } else {
      if (isTable && tableBuffer.length > 0) {
        // Nếu bảng đã kết thúc, chuyển bảng sang HTML
        htmlOutput += markdownToTable(tableBuffer.join('\n'))
        tableBuffer = []
        isTable = false
      }

      // Nếu là dòng văn bản bình thường
      if (line !== '') {
        htmlOutput += `<p>${line}</p>`
      }
    }
  })

  // Xử lý bảng còn lại nếu có
  if (isTable && tableBuffer.length > 0) {
    htmlOutput += markdownToTable(tableBuffer.join('\n'))
  }

  return htmlOutput
}
