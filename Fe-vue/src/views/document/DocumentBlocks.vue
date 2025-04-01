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
        <div
          class="popup-action"
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
          <div v-html="marked(block.Content)" class="markdown-container"></div>

          <div class="action-button" v-if="editMode">
            <font-awesome-icon
              :icon="['fas', 'ellipsis']"
              style="color: black"
              size="lg"
              class="edit-icon"
              @click="togglePopupAction(block, $event)"
            />
          </div>
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
  <UpdateBlockForm v-if="showForm != -1" :loading="false" :content="newContent" @close-form="handleCloseForm" @submit-form="handleSubmitBlockForm"/>
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

const showForm = ref(-1)
const newContent = ref<string | null>(null)
const parentBlock = ref<DocumentBlock | null>(null)
const showPopup = ref<string | null>(null)
const currentType = ref(-1)

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

function handleCloseForm() {
  showForm.value = -1
  editBlock.value = null
  newContent.value = null
}

function handleSubmitBlockForm(data: string) {
  newContent.value = data
  if (showForm.value === 0) {
    editBlock.value!.Content = newContent.value!
    editBlock.value!.State = 1
    updateBlocks()
    handleCloseForm()
    return
  }

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

  if (parentBlock.value) {
    newBlock.ParentId = parentBlock.value.Id
    newBlock.ContentType = parentBlock.value.ContentType
    newBlock.Level = showForm.value + 1

    const lastIndex = findLastChildIndex(parentBlock.value)
    newBlock.Order =calculatorOrder(lastIndex)

    blocks.value.splice(lastIndex + 1, 0, newBlock)
  }
  else {
    newBlock.ContentType = showForm.value
    const lastIndex = findLastIndexByContentType(showForm.value)
    newBlock.Order =calculatorOrder(lastIndex)
    blocks.value.splice(lastIndex + 1, 0, newBlock)
  }

  updateBlocks()
  handleSplitBlock()
  handleCloseForm()
}


function calculatorOrder(index: number) {
  if (index === blocks.value.length - 1) return blocks.value[index].Order + 2000;
  return  Math.round(
      (blocks.value[index].Order + blocks.value[index + 1].Order) / 2,
    )
}

function findLastIndexByContentType(type: number) {
  if (type === 4) return blocks.value.length - 1;
  for (let i = blocks.value.length - 1; i >= 0; i--) {
        if (blocks.value[i].ContentType === type) {
            return i;
        }
    }
    return blocks.value.length - 1;
}

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

function checkUpLevel(block: DocumentBlock) {
  if (block.Level < 3) return false
  return true
}

function checkDownLevel(block: DocumentBlock) {
  if (block.Level === 0) return false
  if (block.Level === 7) return false
  return true
}
const blocks = ref<DocumentBlock[]>([])

function handleUpLevel(block: DocumentBlock) {
  if (!checkUpLevel(block)) return
  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)
  for (let i = blockIndex + 1; i < blocks.value.length; i++) {
    const b = blocks.value[i]
    if (b.State === 3) continue
    if (b.Level < block.Level) break
    if (b.Level === block.Level) {
      b.ParentId = block.Id
      if (b.State != 2) b.State = 1
      continue
    }
    if (b.ParentId === block.Id) {
      handleUpdateChildsWhenUpLevel(b)
      b.Level = block.Level
      if (b.State != 2) b.State = 1
    }
  }

  for (let i = blockIndex - 1; i > 0; i--) {
    const b = blocks.value[i]
    if (b.State === 3) continue
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

function handleUpdateChildsWhenUpLevel(block: DocumentBlock) {
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    handleUpdateChildsWhenUpLevel(b)
    b.Level = block.Level
    if (b.State != 2) b.State = 1
  })
}

function handleDownLevel(block: DocumentBlock) {
  if (!checkDownLevel(editBlock.value!)) return
  const blockIndex = blocks.value.findIndex((b) => b.Id === block.Id)
  for (let i = blockIndex - 1; i > 0; i--) {
    const b = blocks.value[i]
    if (b.State === 3) continue
    if (b.Level <= block.Level) {
      if (b.Id === block.ParentId) {
        console.log('khong the ha cap')
        editBlock.value = null
        return
      }
      block.ParentId = b.Id
      break
    }
  }

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

function handleDelete(block: DocumentBlock) {
  removeChild(block)
  updateBlocks()
  handleSplitBlock()
}

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

  popupPosition.value = {
    top: buttonRect.top - 10,
    right: window.innerWidth - buttonRect.left + 10,
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

function handleCollapseBlock(block: DocumentBlock) {
  block.IsExpand = false
  hiddenBlock(block)
  block.IsShow = true
}

function handleExpandBlock(block: DocumentBlock) {
  block.IsExpand = true
  showBlock(block)
  block.IsShow = true
}

function hiddenBlock(block: DocumentBlock) {
  block.IsShow = false
  const childenBlocks = blocks.value.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    hiddenBlock(b)
  })
}

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
