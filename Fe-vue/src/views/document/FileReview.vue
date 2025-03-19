<template>
  <div class="content-main">
    <div class="file">
      <div
        v-for="(document, index) in documents"
        :key="index"
        class="file-item"
        @click="selectDocument(document)"
      >
        <div style="padding-left: 10px">
          <img
            :src="`/src/assets/icon/${getSrcIconDocument(document.Type)}`"
            alt="logo"
            style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
          />
          <span>{{ document.Name }}</span>
        </div>
        <div class="file-form">
          <div class="form-data">
            <span>Chủ đề </span>
            <select v-model="document!.CategoryId" class="form__input" :disabled="checkEditMode()">
              <option :value="category.Id" v-for="category in categories" :key="category.Id">
                {{ category.Name }}
              </option>
            </select>
          </div>
          <div v-if="document.IsLaw" class="form-data--law">
            <div class="form-data">
              <span>Cơ quan ban hành </span>
              <input v-model="document!.Issuer" class="form__input" :disabled="checkEditMode()" />
            </div>
            <div class="form-data">
              <span>Mã văn bản </span>
              <input
                v-model="document!.DocumentNo"
                class="form__input"
                :disabled="checkEditMode()"
              />
            </div>
            <div class="form-data">
              <span>Người ký</span>
              <input
                v-model="document!.SignerName"
                class="form__input"
                :disabled="checkEditMode()"
              />
            </div>
            <div class="form-data">
              <span>Ngày ban hành</span>
              <input
                v-model="document!.IssueDate"
                class="form__input"
                type="date"
                :disabled="checkEditMode()"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="file-data">
      <div class="file-content">
        <div class="file-content__header" :class="{ 'show-color': !showColorList }">
          <div class="header__color">
            <div class="color-control">
              <div class="title">
                <img src="/src/assets/icon/warning.png" alt="x" />
                <span>Cấp hiển thị mục lục văn bản pháp luật theo màu</span>
              </div>
              <div @click="toggleShowColorList" class="control-icon">
                <font-awesome-icon :icon="['fas', 'chevron-up']" size="2xs" v-if="showColorList" />
                <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
              </div>
            </div>
            <div class="color-list" v-if="showColorList">
              <div v-for="(color, index) in colors" :key="index" class="color-item">
                <div :style="{ backgroundColor: color.color }"></div>
                <span>{{ color.name }}</span>
              </div>
            </div>
          </div>
          <div class="header__option">
            <div :class="{ selected: showBlock }" @click="showBlock = true">
              <span>Phân đoạn tri thức</span>
            </div>
            <div :class="{ selected: !showBlock }" @click="viewMarkdown">
              <span>Văn bản gốc</span>
            </div>
          </div>
        </div>
        <div class="file-content__body">
          <div v-show="showBlock">
            <DocumentBlocks :blocks="showDocument?.DocumentBlocks!" />
          </div>
          <div v-show="!showBlock">
            <div v-html="marked(markdownContent!)" class="markdown-container"></div>
          </div>
        </div>
      </div>
      <div class="footer">
        <div v-if="state">
          <button @click="cancelUpload" v-loading="cancelLoading">Hủy</button>
          <button @click="confirmUpload" v-loading="confirmLoading">Xác nhận</button>
        </div>
        <div v-else>
          <div v-if="!editMode">
            <button @click="editMode = true">Sửa</button>
            <button @click="closeFile(false)">Đóng</button>
          </div>
          <div v-else>
            <button @click="closeFile(true)">Huỷ</button>
            <button @click="updateFile" v-loading="updateLoading">Xác nhận</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Document, DocumentCategory } from '@/entities/Document'
import { getSrcIconDocument } from '@/utils'
import axios from 'axios'
import { marked } from 'marked'
import { onMounted, ref, type PropType } from 'vue'
import DocumentBlocks from './DocumentBlocks.vue'

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

const showColorList = ref(true)

function toggleShowColorList() {
  showColorList.value = !showColorList.value
}

const editMode = ref(false)

const props = defineProps({
  documents: {
    type: Array as PropType<Document[]>,
    required: true,
  },
  state: {
    type: Boolean,
    required: true,
  },
  confirmLoading: {
    type: Boolean,
  },
  cancelLoading: {
    type: Boolean,
  },
  updateLoading: {
    type: Boolean,
  },
})

const emits = defineEmits(['cancelUpload', 'confirmUpload', 'updateFile', 'close'])

function cancelUpload() {
  emits('cancelUpload')
}

function confirmUpload() {
  emits('confirmUpload')
}

function closeFile(value: boolean) {
  emits('close', value)
}

function updateFile() {
  emits('updateFile', showDocument.value!)
}

const showBlock = ref(true)

function checkEditMode(): boolean {
  if (props.state) {
    return false
  }
  if (editMode.value) {
    return false
  }
  return true
}

//chon document de xem
const showDocument = ref<Document | null>(props.documents[0])

function selectDocument(document: Document) {
  showDocument.value = document
}

//xem van ban goc
const markdownContent = ref<string>('')

function viewMarkdown() {
  showBlock.value = false
  fetchMarkdownData()
}

async function fetchMarkdownData() {
  try {
    if (props.state) {
      const response = await axios.get('https://localhost:7160/api/v1/Documents/markdown-review', {
        params: {
          path: showDocument.value!.Path,
        },
      })
      markdownContent.value = response.data
    } else {
      const response = await axios.get(
        `https://localhost:7160/api/v1/Documents/content/${showDocument.value!.Id}`,
      )
      markdownContent.value = response.data
    }
  } catch (error) {
    console.error(error)
  }
}

//danh sach chu de
const categories = ref<DocumentCategory[]>([])

async function fetchCategories() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Documents/categories')
    categories.value = response.data
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => {
  fetchCategories()
  showDocument.value = props.documents[0]
})
</script>

<style lang="scss" scoped>
.markdown-container {
  margin: 0 auto;
  width: 100%;
  padding: 0 30px;
}

::v-deep(.markdown-container strong) {
  font-weight: bold;
}

::v-deep(.markdown-container em) {
  font-style: italic;
}

::v-deep(.markdown-container ul) {
  padding-left: 20px;
  list-style-position: inside;
}

::v-deep(.markdown-container ol) {
  margin-left: 20px;
  // list-style-position: inside;
}
</style>
