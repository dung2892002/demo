<template>
  <div class="document-blocks" @scroll="closeContextMenuAndPopup">
    <div v-for="(blocks, index) in blocksData" :key="index">
      <div class="block title">
        <div @click="toggleBlock(index)" v-if="blocksData[index].length > 0" class="control">
          <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="showBlocks[index]" />
          <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
        </div>
        <div v-else class="control"></div>
        <div v-html="marked(blocksTitle[index])" class="markdown-container"></div>
        <div class="action-button" v-if="editMode">
          <font-awesome-icon
            :icon="['fas', 'ellipsis']"
            style="color: black"
            size="lg"
            class="edit-icon"
            @click="togglePopupContent(index, $event)"
          />
        </div>
        <!-- popup them phan doan cho contentType -->
        <div class="popup-action"
          :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
          v-if="showPopupContent === index"
        >
          <span @click.stop="showFormBlock(index + 1)">Thêm phân đoạn</span>
        </div>
      </div>
      <div v-show="showBlocks[index]" >
        <div
          class="block"
          v-for="(block, index) in blocks"
          :key="index"
          :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
          v-show="block.IsShow == true && block.State != 3"
          @contextmenu.prevent="showContextMenu($event, block, false)"
        >
          <div v-if="checkHasChild(block)" @click.stop="toggleExpandBlock(block)" class="control">
            <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="block.IsExpand" />
            <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
          </div>
          <div v-else class="control"></div>
          <div
            class="block__level"
            :style="{ backgroundColor: colors[block.Level - 1].color }"
            v-if="block.Level != 0"
          ></div>
          <div v-html="marked(formatMarkdown(block.Content))" class="markdown-container" @mouseup.left="showContextMenu($event, block, true)"></div>

          <div class="action-button" v-if="editMode">
            <font-awesome-icon
              :icon="['fas', 'ellipsis']"
              style="color: black"
              size="lg"
              class="edit-icon"
              @click.stop="togglePopupAction(block, $event)"
            />
          </div>
          <!-- poup cac hanh dong voi block -->
          <div
            class="popup-action"
            v-if="showPopup != null && showPopup === block.Id"
            :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
          >
            <span
              v-if="block.Level === 5 || block.Level === 6"
              :class="{ disable: !checkUpLevel(block) }"
              @click.stop="showFormBlock(block.Level)"
              >{{ block.Level === 5 ? 'Thểm khoản' : 'Thêm điểm' }}</span
            >

            <span :class="{ disable: !checkUpLevel(block) }" @click.stop="handleUpLevel(block)"
              >Lên 1 cấp</span
            >
            <span :class="{ disable: !checkDownLevel(block) }" @click.stop="handleDownLevel(block)"
              >Xuống 1 cấp</span
            >
            <span @click.stop="showFormBlock(0)">Sửa</span>
            <span @click.stop="handleDelete(block)">Xóa</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- form de nhap noi dung cho block -->
  <UpdateBlockForm
    v-if="showForm != -1"
    :loading="false"
    :content="newContent"
    @close-form="handleCloseForm"
    @submit-form="handleSubmitBlockForm"
  />
  <!-- menu de chon cac hanh dong voi block -->
  <ContextMenu
      v-if="showMenu"
      :actions="contextMenuActions"
      @actionClick="handleActionClick"
      :position="menuPosition"
    ></ContextMenu>
  <!-- canh bao khi khong the xuong level -->
  <div v-if="downLevelWarning" class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">Xác nhận</h2>
        <button class="form__button" @click="downLevelWarning = false">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <span>
              Không thể chuyển cấp do không đúng định dạng mục lục?
            </span>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--complete" id="submitButton" @click="downLevelWarning = false">
          <span src="/src/assets/icon/refresh.png" alt="logo">Xác nhận</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import { marked } from 'marked'
import { computed, onMounted, ref, watch, type PropType } from 'vue'
import UpdateBlockForm from './UpdateBlockForm.vue'

marked.setOptions({
  breaks: true,
  gfm: true,
})


function formatMarkdown(content: string) {
  return content.replace(/^(\d+)\.\s/gm, '$1\\. ');
}

const showMenu = ref(false)
const showForm = ref(-1)
const newContent = ref<string | null>(null)
const parentBlock = ref<DocumentBlock | null>(null)
const showPopup = ref<string | null>(null)
const currentType = ref(-1)


import ContextMenu from '@/components/ContextMenu.vue'
import type { ActionMenu } from '@/entities/ActionMenu'


const menuPosition = ref({
  top: 0,
  left: 0,
})

function showContextMenu(event: MouseEvent, block: DocumentBlock, state: boolean) {
  closeContextMenuAndPopup()
  beforeText.value = ''
  afterText.value = ''
  selectedText.value = ''
  if (block && props.editMode) {
    if (state) {
      handleSelection(block)
      if (block.ContentType === 2) {
        if (selectedText.value.length === 0) contextMenuActions.value[block.Level - 1].state = false
      }
      else {
        if (block.ContentType === 1) {
          contextMenuActions.value[0].state = false
        }
        if (block.ContentType === 3) {
          contextMenuActions.value[7].state = false
        }
        if (block.ContentType === 4) {
          contextMenuActions.value[8].state = false
        }
      }
    }
    else {
      contextMenuActions.value[block.Level - 1].state = false
      editBlock.value = block
      selectedText.value = block.Content
    }
    menuPosition.value.top = event.clientY
    menuPosition.value.left = event.clientX
    if (event.clientY + 330 > window.innerHeight) {
      menuPosition.value.top -= 340
    }
    if (event.clientX + 200 > window.innerWidth) {
      menuPosition.value.left -= 200
    }
    showMenu.value = true
    showPopup.value = null
  }
}

function handleActionClick(action: ActionMenu) {
  closeContextMenuAndPopup()
  if (action.action === '0') {
    return
  }

  if (action.action === '8') {
    selectedText.value = selectedText.value + afterText.value
    afterText.value = ''
    handleConvertToSightBlock()
    return
  }

  if (action.action === '9') {
    return
  }

  handleUpdateSelectionBlock(editBlock.value!, Number(action.action))
}

const contextMenuActions = ref<ActionMenu[]>([
  { label: 'Đánh dấu là Mở đầu', action: '0',state: true  },
  { label: 'Đánh dấu là Chương', action: '2', state: true },
  { label: 'Đánh dấu là Mục', action: '3', state: true },
  { label: 'Đánh dấu là Tiểu mục', action: '4', state: true },
  { label: 'Đánh dấu là Điều', action: '5', state: true },
  { label: 'Đánh dấu là Khoản', action: '6', state: true },
  { label: 'Đánh dấu là Điểm', action: '7', state: true },
  { label: 'Đánh dấu là Chữ ký', action: '8', state: true },
  { label: 'Đánh dấu là Nội dung khác', action: '9', state: true },
])


function closeContextMenuAndPopup() {
  contextMenuActions.value.forEach((action) => {
    action.state = true
  })
  showMenu.value = false
  showPopup.value = null
  showPopupContent.value = -1
}

defineExpose({closeContextMenuAndPopup})

const fullMarkdown = ref<string| null>(null)

const beforeText = ref<string>('')
const selectedText = ref<string>('')
const afterText = ref<string>('')


function removeHtmlTags(str: string): string {
  return str
    .replace(/<br\s*\/?>/gi, '\n') // Thay thế thẻ <br> bằng xuống dòng
    .replace(/<\/?[^>]+(>|$)/g, ""); // Xóa tất cả các thẻ HTML khác
}

async function handleSelection(block: DocumentBlock) {
  editBlock.value = block;

  const selection = window.getSelection();
  if (!selection || selection.rangeCount === 0) return;

  selectedText.value = selection.toString().trim().split(/\n\s*\n/).pop()!;
  if (!selectedText.value) return;

  // Lấy Markdown gốc
  fullMarkdown.value = await marked.parse(formatMarkdown(block.Content));
  fullMarkdown.value = fullMarkdown.value.slice(3, -5)

  // Loại bỏ các thẻ HTML khỏi Markdown
  const fullText = removeHtmlTags(fullMarkdown.value);

  let startIndex = 0;
  // Xác định vị trí chính xác trong văn bản gốc
  if (selection.toString().trim().split(/\n\s*\n/).length === 1) {
    startIndex = calculateExactIndex(fullText, selectedText.value, selection.getRangeAt(0));
  }

  const indexInMarkdown = calculateIndexInMarkdown(startIndex);
  const textLenght = getSelectedTextLengthInMarkdown(indexInMarkdown)
  /*
  console.log('Vị trí bắt đầu trong văn bản:', startIndex);
  console.log('Vị trí bắt đầu trong markdown:', indexInMarkdown);
  console.log('Độ dài Văn bản được chọn:', selectedText.value.length);
  console.log('Độ dài Văn bản được chọn trong markdown:', textLenght);
  */

  selectedText.value = fixHTMLSubstring(fullMarkdown.value, indexInMarkdown, indexInMarkdown + textLenght).trim()
  beforeText.value = fixAndCompleteHTML(fullMarkdown.value.substring(0, indexInMarkdown).trim())
  afterText.value = fixAndCompleteHTML(fullMarkdown.value.substring(indexInMarkdown + textLenght).trim());

  console.log('Trước:', beforeText.value);
  console.log('Được chọn:', selectedText.value);
  console.log('Sau:', afterText.value);
}


//hoan thien cac the html con thieu va xoa cac the thua
function fixAndCompleteHTML(input: string): string {
  const tagPattern = /<\/?([a-zA-Z0-9]+)[^>]*?>/g;
  const selfClosingTags = ['br', 'img', 'hr', 'input', 'meta', 'link'];

  // 1. Xoá các thẻ đóng và <br> ở đầu chuỗi
  const cleanedInput = input.replace(/^(\s*<\/[^>]+>|\s*<br\s*\/?>)+/gi, '');

  const openStack: string[] = [];
  const missingOpen: string[] = [];

  let match: RegExpExecArray | null;

  tagPattern.lastIndex = 0; // reset regex
  while ((match = tagPattern.exec(cleanedInput)) !== null) {
    const fullTag = match[0];
    const tagName = match[1].toLowerCase();

    const isClosing = fullTag.startsWith('</');
    const isSelfClosing = fullTag.endsWith('/>') || selfClosingTags.includes(tagName);

    if (isSelfClosing) continue;

    if (isClosing) {
      const openIndex = openStack.lastIndexOf(tagName);
      if (openIndex !== -1) {
        openStack.splice(openIndex, 1); // matched → remove
      } else {
        missingOpen.unshift(`<${tagName}>`); // thẻ đóng không có mở → thêm mở
      }
    } else {
      openStack.push(tagName); // thẻ mở
    }
  }

  // 2. Các thẻ chưa được đóng sẽ cần đóng ở cuối
  const missingClose = openStack.reverse().map(tag => `</${tag}>`);

  // 3. Trả kết quả hoàn chỉnh
  return missingOpen.join('') + cleanedInput + missingClose.join('');
}


//hoan thien cac the html cho phan text duoc chon
function fixHTMLSubstring(input: string, startIndex: number, endIndex: number): string {
  const selfClosingTags = ['br', 'img', 'hr', 'input', 'meta', 'link'];
  const openingTagsStack: string[] = [];

  // 1. Phân tích từ đầu đến startIndex → tìm các thẻ đang mở
  const beforeSlice = input.slice(0, startIndex);
  const tagPattern = /<\/?([a-zA-Z0-9]+)[^>]*?>/g;
  let match: RegExpExecArray | null;


  while ((match = tagPattern.exec(beforeSlice)) !== null) {
    const fullTag = match[0];
    const tagName = match[1].toLowerCase();
    const isClosing = fullTag.startsWith('</');
    const isSelfClosing = selfClosingTags.includes(tagName) || fullTag.endsWith('/>');

    if (isSelfClosing) continue;

    if (isClosing) {
      // Xóa thẻ mở tương ứng nếu có
      const lastOpen = openingTagsStack.lastIndexOf(tagName);
      if (lastOpen !== -1) {
        openingTagsStack.splice(lastOpen, 1);
      }
    } else {
      openingTagsStack.push(tagName);
    }
  }

  // 2. Lấy đoạn cắt
  const extracted = input.slice(startIndex, endIndex);

  // 3. Phân tích trong đoạn cắt để tìm các thẻ mở chưa đóng
  const innerOpenStack: string[] = [];
  tagPattern.lastIndex = 0; // reset regex

  while ((match = tagPattern.exec(extracted)) !== null) {
    const fullTag = match[0];
    const tagName = match[1].toLowerCase();
    const isClosing = fullTag.startsWith('</');
    const isSelfClosing = selfClosingTags.includes(tagName) || fullTag.endsWith('/>');

    if (isSelfClosing) continue;

    if (isClosing) {
      if (innerOpenStack.length > 0 && innerOpenStack[innerOpenStack.length - 1] === tagName) {
        innerOpenStack.pop();
      }
    } else {
      innerOpenStack.push(tagName);
    }
  }

  const openingHTML = openingTagsStack.map(tag => `<${tag}>`).join('');
  const closingHTML = [...openingTagsStack.reverse(), ...innerOpenStack.reverse()]
    .map(tag => `</${tag}>`)
    .join('');

  return openingHTML + extracted + closingHTML;
}

//lay do dai trong markdown
function getSelectedTextLengthInMarkdown(indexInMarkdown: number): number {
  if (fullMarkdown.value === null) return 0

  let textLength = 0
  let check = false
  let countText = 0
  for (let i = indexInMarkdown; i < fullMarkdown.value.length; i++) {
    textLength = textLength + 1
    if (fullMarkdown.value[i] === '<') {
      check = true
      continue
    }
    if (fullMarkdown.value[i] === '>') {
      check = false
      continue
    }
    if (!check) countText = countText + 1
    if (countText === selectedText.value.length) {
      break
    }
  }
  return textLength
}

//lay vi tri trong markdown
function calculateIndexInMarkdown(startIndex: number): number {
  if (fullMarkdown.value === null) return 0

  let indexInMarkdown = 0;
  let count = startIndex;
  let checkCount = true;

  for (let i = 0; i < fullMarkdown.value.length; i++) {
    if (count === 0) {
        break
      }
    if (fullMarkdown.value[i] === '<') {
      checkCount = false
      continue
    }
    if (fullMarkdown.value[i] === '>') {
      checkCount = true
      continue
    }
    if (checkCount) {
      count--
      indexInMarkdown = i
    }
  }
  if (indexInMarkdown != 0) indexInMarkdown++

  return indexInMarkdown
}

//lay vi tri chinh xac cua doan text duoc chon trong text goc
function calculateExactIndex(fullText: string, selectedText: string, range: Range): number {
  // Lấy vị trí của đoạn bôi đen trong DOM
  const container = range.startContainer;
  const offset = range.startOffset;

  // Tìm vị trí của đoạn bôi đen trong nội dung gốc
  let currentIndex = 0;
  let matchIndex = -1;

  while ((matchIndex = fullText.indexOf(selectedText, currentIndex)) !== -1) {
    // Kiểm tra xem đoạn tìm thấy có khớp với vị trí trong DOM không
    const textBeforeMatch = fullText.substring(0, matchIndex);
    const domTextBeforeMatch = container.textContent?.substring(0, offset) || '';

    if (textBeforeMatch.endsWith(domTextBeforeMatch)) {
      return matchIndex; // Trả về vị trí chính xác
    }

    currentIndex = matchIndex + 1; // Tiếp tục tìm kiếm
  }

  return -1; // Không tìm thấy
}

//cap nhat lai cac block
function handleUpdateSelectionBlock(block: DocumentBlock, levelSelect: number) {
  block.State = 1;
  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)

  if (beforeText.value.length === 0 && afterText.value.length === 0) {
    block.Level = levelSelect
    handleUpadteLevelBlockAndChild(block)
    updateBlocks()
    handleSplitBlock()
    return
  }
  if (beforeText.value.length === 0) {
    block.Content = afterText.value
    const newBlock: DocumentBlock =
    {
      Id: crypto.randomUUID(),
      ParentId: block.ParentId,
      DocumentId: block.DocumentId,
      Title: selectedText.value,
      Content: selectedText.value,
      Level: levelSelect,
      ContentType: 2,
      Order: calculatorOrder(blockIndex - 1),
      IsExpand: true,
      IsShow: true,
      State: 2,
    }


    if (newBlock.Level > blocks.value[blockIndex - 1].Level) {
       newBlock.ParentId = blocks.value[blockIndex - 1].Id
    }
    blocks.value.splice(blockIndex , 0, newBlock)

    handleUpadteLevelBlockAndChild(newBlock);
  }
  else {
    if (afterText.value.length === 0) {
      block.Content = beforeText.value
      const newBlock: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: selectedText.value,
        Content: selectedText.value,
        Level: levelSelect,
        ContentType: 2,
        Order: calculatorOrder(blockIndex),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      if (newBlock.Level > editBlock.value!.Level) newBlock.ParentId = editBlock.value!.Id

      blocks.value.splice(blockIndex + 1, 0, newBlock)
      handleUpadteLevelBlockAndChild(newBlock);
    }
    else {
      block.Content = beforeText.value
      const newBlock: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: selectedText.value,
        Content: selectedText.value,
        Level: levelSelect,
        ContentType: 2,
        Order: calculatorOrder(blockIndex),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      if (newBlock.Level > editBlock.value!.Level) newBlock.ParentId = editBlock.value!.Id

      blocks.value.splice(blockIndex + 1, 0, newBlock)

      const newBlock2: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: afterText.value,
        Content: afterText.value,
        Level: block.Level,
        ContentType: block.ContentType,
        Order: calculatorOrder(blockIndex+1),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      blocks.value.splice(blockIndex + 2, 0, newBlock2)


      handleUpadteLevelBlockAndChild(newBlock2);
      handleUpadteLevelBlockAndChild(newBlock);
    }
  }

  function handleUpadteLevelBlockAndChild(block: DocumentBlock) {
    const index = blocks.value.findIndex((b) => b.Id === block.Id)

    const childIds : string[] = []
    for (let i = index + 1; i< blocks.value.length; i++) {
      const b = blocks.value[i]
      if (b.Level <= block.Level) break
      if (!childIds.includes(b.ParentId!)) {
        b.ParentId = block.Id
        b.State = 1
      }
      childIds.push(b.Id!)
    }

    for (let i = index + 1; i< blocks.value.length; i++) {
      const b = blocks.value[i]
      if (b.Level <= block.Level && b.ParentId === block.Id) {
        updateParentBlock(b)
        b.State = 1
      }
    }

    updateParentBlock(block)
    updateBlocks()
    handleSplitBlock()
    return
  }
}


function handleConvertToSightBlock() {
  const index = blocks.value.findIndex((b) => b.Id === editBlock.value!.Id)
  if (beforeText.value.length != 0) {
    editBlock.value!.Content = beforeText.value
    editBlock.value!.State = 1
  } else {
    editBlock.value!.State = 3
  }

  let lastContentIndex = blocks.value.length - 1;

  for (let i = index + 1; i < blocks.value.length; i++) {
    const b = blocks.value[i]
    if (b.ContentType != 2) {
      lastContentIndex = i - 1
      break
    }
  }


  const newBlock: DocumentBlock =
  {
    Id: crypto.randomUUID(),
    ParentId: null,
    DocumentId: blocks.value[0].DocumentId,
    Title: 'Chu ky',
    Content: selectedText.value,
    Level: 0,
    ContentType: 3,
    Order: calculatorOrder(lastContentIndex),
    IsExpand: true,
    IsShow: true,
    State: 2,
  }

  for (let i = index + 1; i <= lastContentIndex; i++) {
    const b = blocks.value[i]
    b.State = 3
    newBlock.Content += '\n' + marked.parse(formatMarkdown(b.Content));
  }

  blocks.value.splice(lastContentIndex + 1, 0, newBlock)

  updateBlocks()
  handleSplitBlock()
}

function updateParentBlock(block: DocumentBlock) {
  const index = blocks.value.findIndex((b) => b.Id === block.Id)
  let check = false
  for (let i = index - 1; i >= 0; i--) {
    const b = blocks.value[i]
    if (b.ContentType != block.ContentType) break
    if (b.Level < block.Level) {
      block.ParentId = b.Id
      check = true
      break
    }
  }

  if (!check) {
    block.ParentId = null
  }
}

//mo form them hoac sua block
function showFormBlock(state: number) {
  /*trang thai state
  0: sua block
  1,2,3,4: them moi block cho cac khoi first, content, sight, other
  5: them khoan
  6: them diem
  */
  showPopup.value = null
  showPopupContent.value = -1
  console.log(state)
  showForm.value = state
  if (state == 0) {
    newContent.value = editBlock.value!.Content
    return
  }
  parentBlock.value = editBlock.value
  editBlock.value = null
}


//dong form
function handleCloseForm() {
  showForm.value = -1
  editBlock.value = null
  newContent.value = null
}


//them hoac sua block
function handleSubmitBlockForm(data: string) {
  newContent.value = data
  //sua block
  if (showForm.value === 0) {
    editBlock.value!.Content = newContent.value!
    editBlock.value!.State = 1
    updateBlocks()
    handleCloseForm()
    return
  }

  //tao block moi
  const newBlock: DocumentBlock = {
    Id: crypto.randomUUID(),
    ParentId: null,
    DocumentId: blocks.value[0].DocumentId,
    Title: newContent.value!,
    Content: newContent.value!,
    Level: 0,
    ContentType: showForm.value,
    Order: 0,
    IsExpand: true,
    IsShow: true,
    State: 2,
  }

  if (newBlock.ContentType === 2) {
    newBlock.Level = 2
  }

  //kieam tra block tao moi co thuoc block nao khong
  if (parentBlock.value) {
    newBlock.ParentId = parentBlock.value.Id
    newBlock.ContentType = parentBlock.value.ContentType
    newBlock.Level = showForm.value + 1

    const lastIndex = findLastChildIndex(parentBlock.value)
    newBlock.Order = calculatorOrder(lastIndex)

    blocks.value.splice(lastIndex + 1, 0, newBlock)
  } else {
    newBlock.ContentType = showForm.value
    const lastIndex = findLastIndexByContentType(showForm.value)
    newBlock.Order = calculatorOrder(lastIndex)
    blocks.value.splice(lastIndex + 1, 0, newBlock)
  }

  updateBlocks()
  handleSplitBlock()
  handleCloseForm()
}


//tinh vi tri cho block moi tao
function calculatorOrder(index: number) {
  if (index === blocks.value.length - 1) return blocks.value[index].Order + 2000
  return Math.round((blocks.value[index].Order + blocks.value[index + 1].Order) / 2)
}


//tim vi tri cuoi cung cua block theo contentType
function findLastIndexByContentType(type: number) {
  if (type === 4) return blocks.value.length - 1
  for (let i = blocks.value.length - 1; i >= 0; i--) {
    if (blocks.value[i].ContentType === type) {
      return i
    }
  }
  return blocks.value.length - 1
}


//tim vi tri block con cuoi cung cua 1 block khi them con
function findLastChildIndex(block: DocumentBlock) {
  const children = blocks.value.filter((b) => b.ParentId === block.Id)

  // Nếu không có block con nào, trả về chỉ số của block đầu vào
  if (children.length === 0) {
    return blocks.value.findIndex((b) => b.Id === block.Id)
  }

  // Lấy block con cuối cùng theo thứ tự trong mảng
  const lastChild = children[children.length - 1]

  // Đệ quy để tìm phần tử con cuối cùng của lastChild
  return findLastChildIndex(lastChild)
}


//danh sach cac mau cua level
const colors = [
  {
    color: '#e81c2b',
    name: 'Phần',
  },
  {
    color: '#f4891e',
    name: 'Chương',
  },
  {
    color: '#eace2a',
    name: 'Mục',
  },
  {
    color: '#0aa34f',
    name: 'Tiểu mục',
  },
  {
    color: '#459fe3',
    name: 'Điều',
  },
  {
    color: '#d80b8f',
    name: 'Khoản',
  },
  {
    color: '#6a3499',
    name: 'Điểm',
  },
]

const editBlock = ref<DocumentBlock | null>(null)

const popupPosition = ref({
  top: 0,
  right: 200,
})


//kiem tra co the len cap block khong
function checkUpLevel(block: DocumentBlock) {
  if (block.Level < 3) return false
  return true
}


//kiem tra co the xuong cap block khong
function checkDownLevel(block: DocumentBlock) {
  if (block.Level === 0) return false
  if (block.Level === 7) return false
  return true
}

const blocks = ref<DocumentBlock[]>([])


//xu ly viec len cap block
function handleUpLevel(block: DocumentBlock) {
  if (!checkUpLevel(block)) return

  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)

  //duyet cac phan tu o sau no
  for (let i = blockIndex + 1; i < blocks.value.length; i++) {
    const b = blocks.value[i]
    if (b.State === 3) continue

    //gap block level cao hon thi dung lai
    if (b.Level < block.Level) break

    //neu ngang level thi se cho lam con
    if (b.Level === block.Level) {
      b.ParentId = block.Id
      if (b.State != 2) b.State = 1
      continue
    }

    //neu dang lam con thi update them cac con cua no
    if (b.ParentId === block.Id) {
      handleUpdateChildsWhenUpLevel(b)
      b.Level = block.Level
      if (b.State != 2) b.State = 1
    }
  }

  //duyet cac block o truoc no
  for (let i = blockIndex - 1; i > 0; i--) {
    const b = blocks.value[i]

    //neu block bi xoa thi bo qua
    if (b.State === 3) continue

    //neu la cha cua block dang len cap, thi cap nhat cha cua block len cap
    if (b.Id === block.ParentId) {
      if (block.Level - 1 === b.Level) block.ParentId = b.ParentId
    }
  }

  block.Level--
  if (block.State != 2) block.State = 1
  editBlock.value = null
  updateBlocks()
  handleSplitBlock()
  showPopup.value = null
}


//dequy cap nhat cac block con,chau... cua 1 block
function handleUpdateChildsWhenUpLevel(block: DocumentBlock) {
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    handleUpdateChildsWhenUpLevel(b)
    b.Level = block.Level
    if (b.State != 2) b.State = 1
  })
}

//canh bao khi khong the xuong cap
const downLevelWarning = ref(false)


//ham xu ly xuong cap 1 block
function handleDownLevel(block: DocumentBlock) {
  if (!checkDownLevel(block)) return
  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)

  //duyet cac block o truoc no
  for (let i = blockIndex - 1; i >= 0; i--) {
    const b = blocks.value[i]
    //neu block xuong cap la phan dau tien cua contentBlock thi khong cho xuong cap
    if (b.ContentType === 1) {
        downLevelWarning.value = true
        showPopup.value = null
        editBlock.value = null
        return
    }

    //neu block bi xoa thi bo qua
    if (b.State === 3) continue

    //neu no la cap cao hon hoac bang cua block xuong cap
    if (b.Level <= block.Level) {
      //neu la cha cua block xuong cap(khong co block nao de chua no)
      if (b.Id === block.ParentId) {
        downLevelWarning.value = true
        showPopup.value = null
        editBlock.value = null
        return
      }
      block.ParentId = b.Id
      if (b.IsExpand === false) {
        b.IsShow = false
      }
      break
    }
  }

  //duyet cac phan tu o sau no
  for (let i = blockIndex + 1; i < blocks.value.length; i++) {
    const b = blocks.value[i]
    if (b.State === 3) continue
    if (b.ParentId === block.Id) {
      b.ParentId = block.ParentId
      if (b.State != 2) b.State = 1
    }
  }

  block.Level++
  if (block.State != 2) block.State = 1
  editBlock.value = null
  updateBlocks()
  handleSplitBlock()
  showPopup.value = null
}


//xu ly xoa block
function handleDelete(block: DocumentBlock) {
  removeChild(block)
  updateBlocks()
  handleSplitBlock()
}


//xoa block thi se de quy xoa cac ptu con,chau cua no
function removeChild(block: DocumentBlock) {
  block.State = 3
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    removeChild(b)
  })
}



function togglePopupAction(block: DocumentBlock, event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement
  showMenu.value = false
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  let newTop = buttonRect.top - 10
  const newRight = window.innerWidth - buttonRect.left + 10
  const screenHeight = window.innerHeight

  if (newTop + 150 > screenHeight) {
    newTop = newTop - 100
  }

  popupPosition.value = {
    top: newTop,
    right: newRight,

  }

  showPopupContent.value = -1
  editBlock.value = editBlock.value?.Id === block.Id ? null : block
  if (showPopup.value === block.Id) showPopup.value = null
  else showPopup.value = block.Id
}


const showPopupContent = ref(-1)

function togglePopupContent(index: number, event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - 10,
    right: window.innerWidth - buttonRect.left + 10,
  }
  showPopup.value = null
  editBlock.value = null
  currentType.value = index
  showPopupContent.value = showPopupContent.value === index ? -1 : index
}

function checkHasChild(block: DocumentBlock) {
  return blocks.value.some((b) => b.ParentId == block.Id && b.State != 3)
}


function toggleExpandBlock(block: DocumentBlock) {
  showMenu.value = false
  showPopup.value = null
  showPopupContent.value = -1
  if (block.IsExpand) {
    handleCollapseBlock(block)
  } else {
    handleExpandBlock(block)
  }
}


//thu gon
function handleCollapseBlock(block: DocumentBlock) {
  block.IsExpand = false
  hiddenBlock(block)
  block.IsShow = true
}


//mo rong
function handleExpandBlock(block: DocumentBlock) {
  block.IsExpand = true
  showBlock(block)
  block.IsShow = true
}


//an block
function hiddenBlock(block: DocumentBlock) {
  block.IsShow = false
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    hiddenBlock(b)
  })
}


//hien block
function showBlock(block: DocumentBlock) {
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    b.IsShow = true
    if (b.IsExpand) {
      showBlock(b)
    }
  })
}

const props = defineProps({
  propBlocks: {
    type: Array as PropType<DocumentBlock[]>,
    required: true,
  },
  editMode: {
    type: Boolean,
    required: true,
  },
})

const emits = defineEmits(['updateBlocks'])
const updatedBlocks = computed(() => blocks.value.filter((b) => b.State != 0))

function updateBlocks() {
  emits('updateBlocks', updatedBlocks.value)
}

const showBlocks = ref([true, true, true, true])
const blocksTitle = ['Mở đầu', 'Nội dung', 'Chữ ký', 'Khác']
function toggleBlock(index: number) {
  showBlocks.value[index] = !showBlocks.value[index]
}

const blocksData = ref<DocumentBlock[][]>([])


//chia blocks theo contentType
function handleSplitBlock() {
  const firstBlocks: DocumentBlock[] = []
  const contentBlocks: DocumentBlock[] = []
  const signBlocks: DocumentBlock[] = []
  const otherBlocks: DocumentBlock[] = []

  blocks.value.forEach((block) => {
    if (block.State === 3) return
    if (block.ContentType == 1) {
      firstBlocks.push(block)
    } else if (block.ContentType == 2) {
      contentBlocks.push(block)
    } else if (block.ContentType == 3) {
      signBlocks.push(block)
    } else {
      otherBlocks.push(block)
    }
  })

  blocksData.value = [firstBlocks, contentBlocks, signBlocks, otherBlocks]
}

watch(
  () => props.propBlocks,
  () => {
    blocks.value = props.propBlocks.map((block) => ({
      ...block,
      State: 0,
      IsExpand: true,
      IsShow: true,
    }))

    handleSplitBlock()
  },
)

onMounted(() => {
  blocks.value = props.propBlocks.map((block) => ({
    ...block,
    State: 0,
    IsExpand: true,
    IsShow: true,
  }))

  handleSplitBlock()
})
</script>
