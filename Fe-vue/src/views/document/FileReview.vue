<template>
  <div class="content-main">
    <div class="file" @click="handleClosePopup" v-if="!loadingCategories">
      <div
        v-for="(document, index) in documents"
        :key="index"
        class="file-item"
        @click.stop="selectDocument(document, index)"
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
          <div v-if="document.IsLaw" class="form-data--law">
            <div class="form-data data-select" style="width: 100%;" :disable="checkEditMode()" >
              <span class="form__label">Lĩnh vực <span class="required">*</span></span>
              <div class="value--current" @click.stop="toggleShowSelectCategory(index)" style="width: 100%;" :class="{ 'disabled': checkEditMode() }">
                <span>{{  currentCategories[index].Name  }}</span>
                <div>
                  <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectCategory === index && !checkEditMode()" />
                  <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
                </div>
                <div class="value-data more" v-if="showSelectCategory === index && !checkEditMode()">
                  <div
                    v-for="category in categories"
                    :key="category.Id"
                    @click.stop="selectCategory(category, index)"
                    :class="{ selected: currentCategories[index].Id === category.Id }"
                    class="data"
                  >
                    <span> {{ category.Name }}</span>
                    <font-awesome-icon
                      :icon="['fas', 'check']"
                      v-if="currentCategories[index].Id === category.Id"
                    />
                  </div>
                </div>
              </div>
            </div>
            <div class="form-data">
              <span>Cơ quan ban hành </span>
              <input v-model="document!.Issuer" class="form__input" :disabled="checkEditMode()" :class="{ 'disabled': checkEditMode() }"/>
            </div>
            <div class="form-data">
              <span>Ngày ban hành</span>
              <DateInput v-model="document.IssueDate!" :state="checkEditMode()"/>
            </div>
            <div class="form-data">
              <span>Ngày có hiệu lực</span>
              <DateInput v-model="document.EffectiveDate!" :state="checkEditMode()"/>
            </div>
            <div class="form-data">
              <span>Mã văn bản </span>
              <input
                v-model="document!.DocumentNo"
                class="form__input"
                :disabled="checkEditMode()"
                :class="{ 'disabled': checkEditMode() }"
              />
            </div>
            <div class="form-data">
              <span>Tên văn bản </span>
              <input
                v-model="document!.DocumentName"
                class="form__input"
                :disabled="checkEditMode()"
                :class="{ 'disabled': checkEditMode() }"
              />
            </div>
            <div class="form-data">
              <span>Người ký</span>
              <input
                v-model="document!.SignerName"
                class="form__input"
                :disabled="checkEditMode()"
                :class="{ 'disabled': checkEditMode() }"
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
        <div class="file-content__body" @scroll="handleClosePopup">
          <div v-show="showBlock" style="min-height: 500px;" >
            <DocumentBlocks
              :propBlocks="showDocument?.DocumentBlocks!"
              :edit-mode="editMode"
              :state="props.state"
              @update-blocks="handleUpdateBlocks"
              ref="blocksRef"
            />
          </div>
          <div v-show="!showBlock && showDocument?.MarkdownContent">
            <div v-html="marked(handleCretaeTableFromMarkdown(showDocument?.MarkdownContent!))" class="markdown-container"></div>
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
import type { Document, DocumentBlock, DocumentCategory } from '@/entities/Document'
import { getSrcIconDocument } from '@/ts/utils'
import axios from 'axios'
import { marked } from 'marked'
import {  onMounted, ref, type PropType } from 'vue'
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

// const parentRef = ref(null);
const blocksRef = ref<InstanceType<typeof DocumentBlocks> | null>(null);

import { debounce } from "lodash";
import { handleCretaeTableFromMarkdown } from '@/ts/markdown'
import DateInput from '@/components/DateInput.vue'

const handleClosePopup = debounce(() => {
  if (blocksRef.value?.closeContextMenuAndPopup) {
    blocksRef.value.closeContextMenuAndPopup();
  }
}, 100); // Chỉ gọi sau 200ms nếu không có scroll mới


const currentCategories = ref<DocumentCategory[]>([])
const showSelectCategory = ref(-1)
function toggleShowSelectCategory(index : number) {
  if (showSelectCategory.value === index || checkEditMode()) {
    showSelectCategory.value = -1
    return
  }
  showSelectCategory.value = index
}

function selectCategory(category: DocumentCategory, index: number) {
  currentCategories.value[index] = category
  showDocument.value!.CategoryId = category.Id
  showSelectCategory.value = -1
}

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
  }
})

const emits = defineEmits(['cancelUpload', 'confirmUpload', 'updateFile', 'close'])

function cancelUpload() {
  emits('cancelUpload')
}

const updatedBlocks = ref<DocumentBlock[]>([])

function handleUpdateBlocks(updated: DocumentBlock[]) {
  updatedBlocks.value = updated
}

function confirmUpload() {
  emits('confirmUpload')
}

function closeFile(value: boolean) {
  emits('close', value)
}

function updateFile() {
  emits('updateFile', showDocument.value!, updatedBlocks.value)
}

const showBlock = ref(true)

function checkEditMode(): boolean {
  if (props.state) {
    return true
  }
  if (editMode.value) {
    return false
  }
  return true
}

//chon document de xem
const showDocument = ref<Document | null>(props.documents[0])
const currentIndex = ref(0)
function selectDocument(document: Document, index: number) {
  showDocument.value = document
  currentIndex.value = index
  if (showBlock.value == false) {
    fetchMarkdownData()
  }
}

//xem van ban goc

function viewMarkdown() {
  showBlock.value = false
  fetchMarkdownData()
}

async function fetchMarkdownData() {
  try {
    if (showDocument.value?.MarkdownContent?.length === 0) {
      if (props.state) {
        const response = await axios.get(
          'https://localhost:7160/api/v1/Documents/markdown-review',
          {
            params: {
              path: showDocument.value!.Path,
            },
          },
        )
        props.documents[currentIndex.value]!.MarkdownContent = response.data
      } else {
        const response = await axios.get(
          `https://localhost:7160/api/v1/Documents/content/${showDocument.value!.Id}`,
        )
        props.documents[currentIndex.value]!.MarkdownContent = response.data
      }
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

    props.documents.forEach((document) => {
    const category = categories.value.find((category) => category.Id === document.CategoryId)
    if (category) {
      currentCategories.value.push(category)
    }
  })
  } catch (error) {
    console.error(error)
  }
}

const loadingCategories = ref(true)

onMounted(async () => {
  await fetchCategories()
  loadingCategories.value = false
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
  font-weight: 500;
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

}
</style>
