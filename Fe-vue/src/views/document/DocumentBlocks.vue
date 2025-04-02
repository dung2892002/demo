<template>
  <div class="document-blocks">
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
      <div v-show="showBlocks[index]">
        <div
          class="block"
          v-for="(block, index) in blocks"
          :key="index"
          :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
          v-show="block.IsShow == true && block.State != 3"
        >
          <div v-if="checkHasChild(block)" @click="toggleExpandBlock(block)" class="control">
            <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="block.IsExpand" />
            <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
          </div>
          <div v-else class="control"></div>
          <div
            class="block__level"
            :style="{ backgroundColor: colors[block.Level - 1].color }"
            v-if="block.Level != 0"
          ></div>
          <div v-html="marked(formatMarkdown(block.Content))" class="markdown-container" @mouseup="handleSelection(block)"></div>

          <div class="action-button" v-if="editMode">
            <font-awesome-icon
              :icon="['fas', 'ellipsis']"
              style="color: black"
              size="lg"
              class="edit-icon"
              @click="togglePopupAction(block, $event)"
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

const showForm = ref(-1)
const newContent = ref<string | null>(null)
const parentBlock = ref<DocumentBlock | null>(null)
const showPopup = ref<string | null>(null)
const currentType = ref(-1)


import TurndownService from 'turndown';

const turndownService = new TurndownService();

const reverseMarked = (html: string) => {
  return turndownService.turndown(html);
};

//xu ly viec boi den 1 phan oi dung block
function handleSelection(block: DocumentBlock) {
  console.log('danh dau')

  const selection = window.getSelection();
  if (!selection || selection.rangeCount === 0) return;

  const range = selection.getRangeAt(0);
  const selectedText = selection.toString().trim();
  if (!selectedText) return;

  // Lấy HTML đã chọn
  const tempDiv = document.createElement('div');
  tempDiv.appendChild(range.cloneContents());
  const selectedHtml = tempDiv.innerHTML; // Nội dung HTML đã chọn

  console.log("Selected HTML:", selectedHtml);

  // Chuyển ngược HTML về Markdown để xác định vị trí
  const tempMarkdown = reverseMarked(selectedHtml); // Hàm này cần tự cài đặt

  // Lấy Markdown gốc
  const fullMarkdown = block.Content;

  // Xác định vị trí trong Markdown gốc
  const startIndex = fullMarkdown.indexOf(tempMarkdown);

  const beforeText = fullMarkdown.substring(0, startIndex).trim();
  const afterText = fullMarkdown.substring(startIndex + tempMarkdown.length).trim();

  console.log('Trước:', beforeText.length);
  console.log('Được chọn:', selectedText.length);
  console.log('Sau:', afterText.length);

  handleUpdateSelectionBlock(block, 5, beforeText, selectedText, afterText)

}

function handleUpdateSelectionBlock(block: DocumentBlock, levelSelect: number, beforeText: string, selectedText: string, afterText: string) {
  block.State = 1;
  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)
  console.log('index cua block duoc danh dau: ', blockIndex)
  const gapLevel = levelSelect - block.Level
  console.log(gapLevel)

  if (beforeText.length === 0 && afterText.length === 0) {
    handleUpadteLevelBlockAndChild(block);
    return
  }
  if (beforeText.length === 0) {
    console.log('chen ra truoc')
    block.Content = afterText
    const newBlock: DocumentBlock =
    {
      Id: crypto.randomUUID(),
      ParentId: block.ParentId,
      DocumentId: block.DocumentId,
      Title: selectedText,
      Content: selectedText,
      Level: block.Level,
      ContentType: block.ContentType,
      Order: calculatorOrder(blockIndex - 1),
      IsExpand: true,
      IsShow: true,
      State: 2,
    }

    blocks.value.splice(blockIndex , 0, newBlock)
    handleUpadteLevelBlockAndChild(newBlock);
  }
  else {
    if (afterText.length === 0) {
      console.log('chen ra sau')
      block.Content = beforeText
      const newBlock: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: selectedText,
        Content: selectedText,
        Level: block.Level,
        ContentType: block.ContentType,
        Order: calculatorOrder(blockIndex),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      blocks.value.splice(blockIndex + 1, 0, newBlock)
      handleUpadteLevelBlockAndChild(newBlock);
    }
    else {
      console.log('chen 2 block ra sau')
      block.Content = beforeText
      const newBlock: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: selectedText,
        Content: selectedText,
        Level: block.Level,
        ContentType: block.ContentType,
        Order: calculatorOrder(blockIndex),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      blocks.value.splice(blockIndex + 1, 0, newBlock)

      const newBlock2: DocumentBlock =
      {
        Id: crypto.randomUUID(),
        ParentId: block.ParentId,
        DocumentId: block.DocumentId,
        Title: afterText,
        Content: afterText,
        Level: block.Level,
        ContentType: block.ContentType,
        Order: calculatorOrder(blockIndex+1),
        IsExpand: true,
        IsShow: true,
        State: 2,
      }
      blocks.value.splice(blockIndex + 2, 0, newBlock2)

      handleUpadteLevelBlockAndChild(newBlock);
      if (gapLevel === 0) handleUpadteLevelBlockAndChild(newBlock2);
    }
  }

  function handleUpadteLevelBlockAndChild(block: DocumentBlock) {
    if (gapLevel === 0) {
      const index = blocks.value.findIndex((b) => b.Id === block.Id)
      console.log('index cua block moi duoc tach ra la: ', index)
      for (let i = index + 1; i< blocks.value.length; i++) {
        const b = blocks.value[i]
        if (b.Level <= block.Level) break
        console.log('cap nhat cha moi cho block: ', b.Content)
        b.ParentId = block.Id
        b.State = 1
      }
      updateBlocks()
      handleSplitBlock()
      return
    }

    if (gapLevel < 0) {
      for (let i = 0; i> gapLevel; i--) {
        handleUpLevel(block)
      }
    }
    else {
      for (let i = 0; i < gapLevel; i++) {
        handleDownLevel(block)
      }
    }
    updateBlocks()
    handleSplitBlock()
    return
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
    ContentType: 0,
    Order: 0,
    IsExpand: true,
    IsShow: true,
    State: 2,
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
