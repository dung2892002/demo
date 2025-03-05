<template>
  <div class="content">
    <div class="content__header">
      <div class="content__filter">
        <div class="folder-route">
          <img
            src="/src/assets/icon/btn-prev-page.svg"
            alt=""
            v-if="listDocuments.length > 0"
            @click="goBackDoucument"
          />
          <span @click="backToRoot">Tài liệu</span>
          <div
            v-for="(document, index) in listDocuments"
            :key="index"
            @click="routeDocument(document, index)"
          >
            <img
              src="/src/assets/icon/btn-next-page.svg"
              alt=""
              style="width: 16px; height: 16px"
            /><span> {{ document.Name }}</span>
          </div>
        </div>
      </div>
      <div class="content--row">
        <button class="content__button button--add">
          <div class="content__button button--add" @click="handleAddFile">
            <img src="/src/assets/icon/add-file.png" alt="logo" class="button--add-logo" />
            <span class="button--add-text">Thêm tệp</span>
          </div>
        </button>
        <button class="content__button button--add">
          <div class="content__button button--add" @click="handleAddFolder">
            <img src="/src/assets/icon/add-folder.png" alt="logo" class="button--add-logo" />
            <span class="button--add-text">Thêm thư mục</span>
          </div>
        </button>
      </div>
    </div>
    <div class="content-main">
      <div class="toolbar">
        <div style="display: flex; flex-direction: row; gap: 20px">
          <div class="toolbar_search">
            <input
              type="text"
              id="search-employee"
              placeholder="Tìm kiếm theo từ khóa"
              v-model="keyword"
              @keydown.enter="fetchDocument"
            />
          </div>
        </div>
        <div class="toolbar__actions"></div>
      </div>
      <div class="main-container" ref="tableContainer" v-loading="loading">
        <table class="employee-table">
          <thead>
            <tr>
              <th class="w-20">Tên tài liệu</th>
              <th class="w-10">Thể loại</th>
              <th class="w-10">Ngày tạo</th>
              <th class="w-30" v-if="keyword.trim().length > 0">Vị trí</th>
              <th class="w-10">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(document, index) in documents"
              :key="index"
              @dblclick="handleSelectDocument(document)"
              style="cursor: pointer"
            >
              <td>
                <img
                  :src="`/src/assets/icon/${getSrcIconDocument(document.Type)}`"
                  alt="logo"
                  style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
                />
                <span style="vertical-align: middle" v-html="highlightTexts[index]"></span>
              </td>
              <td>{{ document.Category?.Name }}</td>
              <td>{{ document.CreatedAt ? formatDate(document.CreatedAt) : '' }}</td>
              <td v-if="keyword.trim().length > 0">{{ document.FolderPath }}</td>
              <td>
                <div class="action" :ref="`action-${index}`">
                  <div class="action-buttons">
                    <button class="action-button" @click="togglePopupAction(index, $event)">
                      <img src="/src/assets/icon/kebab-menu.png" alt="" />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span @click="handleSelectDocument(document)">Xem</span>
                      <span @click="handleUpdateDocument(document)">Sửa</span>
                      <span
                        @click="handleDeleteDocument(document.Id, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span @click="handleMoveDocument(document)">Di chuyển</span>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <ThePagnigation
        :pageNumber="pageNumber"
        :pageLoading="pageLoading"
        @pageChange="handlePageChange"
        @pageSizeChange="handlePageSizeChange"
      />
    </div>

    <AddFileForm
      v-if="showAddFileForm"
      @close-form="closeForm"
      :parent-id="currentDocument?.Id || ''"
    />

    <AddFolderForm
      v-if="showAddFolderForm"
      @close-form="closeForm"
      :parent-id="currentDocument?.Id || ''"
    />

    <UpdateForm v-if="showUpdateForm" @close-form="closeForm" :id="documentUpdatedId!" />

    <MoveDocument v-if="showMoveDocumentForm" @close-form="closeForm" :document="moveDocument" />
  </div>

  <FileDetail v-if="showDocumentDetail" :document="documentDetail" @close-file="closeFile" />
</template>

<script setup lang="ts">
import ThePagnigation from '@/components/ThePagnigation.vue'
import { DocumentType, type Document } from '@/entities/Document'
import { formatDate, getSrcIconDocument } from '@/utils'
import axios from 'axios'
import { onMounted, ref, watch } from 'vue'
import { useStore } from 'vuex'
import AddFileForm from './AddFileForm.vue'
import AddFolderForm from './AddFolderForm.vue'
import MoveDocument from './MoveDocument.vue'
import FileDetail from './FileDetail.vue'
import UpdateForm from './UpdateForm.vue'

import debounce from 'lodash/debounce'

const keyword = ref<string>('')
const store = useStore()

const loading = ref(false)

const pageLoading = ref(false)
const deleteLoading = ref<number>(-1)
const documentUpdatedId = ref<string | null>(null)

const currentDocument = ref<Document | null>(null)
const pageSize = ref(20)
const pageNumber = ref(1)

const categoryFilterId = ref<string | null>(null)
const typeFilter = ref<DocumentType | null>(null)

const documents = ref<Document[]>([])

const listDocuments = ref<Document[]>([])

const showAddFileForm = ref(false)
const showAddFolderForm = ref(false)
const showMoveDocumentForm = ref(false)
const showUpdateForm = ref(false)

const debouncedFind = debounce(fetchDocument, 500)

watch(keyword, () => {
  debouncedFind()
})

const moveDocument = ref<Document>({
  Id: '',
  Name: '',
  Type: DocumentType.Unknown,
  CategoryId: null,
  CreatedAt: '',
  FolderPath: '',
})

const documentDetail = ref<Document>({
  Id: '',
  Name: '',
  Type: DocumentType.Unknown,
  CategoryId: null,
  CreatedAt: '',
  FolderPath: '',
})

const showDocumentDetail = ref(false)

const popupPosition = ref({
  top: 0,
  right: 100,
})

const showPopupAction = ref<number>(-1)

function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.target instanceof HTMLElement ? event.target : null
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - buttonRect.height,
    right: window.innerWidth - buttonRect.left,
  }

  showPopupAction.value = showPopupAction.value === index ? -1 : index
}

async function handleUpdateDocument(document: Document) {
  documentUpdatedId.value = document.Id
  showUpdateForm.value = true
  showPopupAction.value = -1
}

async function handleDeleteDocument(id: string | null, index: number) {
  deleteLoading.value = index
  try {
    await axios.delete(`https://localhost:7160/api/v1/Documents/${id}`)
    await fetchDocument()
    deleteLoading.value = -1
    showPopupAction.value = -1
  } catch (error) {
    console.log(error)
  }
}

function handleMoveDocument(document: Document) {
  moveDocument.value = document
  showMoveDocumentForm.value = true
  showPopupAction.value = -1
}

function handleAddFile() {
  showAddFileForm.value = true
  showPopupAction.value = -1
}

function handleAddFolder() {
  showAddFolderForm.value = true
  showPopupAction.value = -1
}

function closeForm(value: boolean) {
  showAddFileForm.value = false
  showAddFolderForm.value = false
  showMoveDocumentForm.value = false
  showUpdateForm.value = false
  if (value == true) {
    fetchDocument()
  }
}

function closeFile() {
  showDocumentDetail.value = false
  documentDetail.value = {
    Id: '',
    Name: '',
    Type: DocumentType.Unknown,
    CategoryId: null,
    CreatedAt: '',
    FolderPath: '',
  }
}

function handleSelectDocument(document: Document) {
  if (document.Type === DocumentType.Folder) {
    resetQuery()
    currentDocument.value = document
    listDocuments.value.push(document)
    showPopupAction.value = -1
    fetchDocument()
  } else {
    documentDetail.value = document
    showDocumentDetail.value = true
  }
}

function goBackDoucument() {
  resetQuery()
  if (listDocuments.value.length > 1) {
    listDocuments.value.pop()
    currentDocument.value = listDocuments.value[listDocuments.value.length - 1]
    fetchDocument()
  } else {
    listDocuments.value = []
    currentDocument.value = null
    fetchDocument()
  }
}

function routeDocument(document: Document, index: number) {
  if (index < listDocuments.value.length - 1) {
    resetQuery()
    listDocuments.value = listDocuments.value.slice(0, index + 1)
    currentDocument.value = document
    fetchDocument()
  }
}

function backToRoot() {
  if (listDocuments.value.length > 0) {
    resetQuery()
    listDocuments.value = []
    currentDocument.value = null
    fetchDocument()
  }
}

function resetQuery() {
  categoryFilterId.value = null
  typeFilter.value = null
  pageNumber.value = 1
}

async function handlePageSizeChange(newPageSize: number) {
  pageLoading.value = true
  pageSize.value = newPageSize
  pageNumber.value = 1
  await fetchDocument()
  pageLoading.value = false
}

async function handlePageChange(newPageNumber: number) {
  pageLoading.value = true
  pageNumber.value = newPageNumber
  await fetchDocument()
  pageLoading.value = false
}

const highlightTexts = ref<string[]>([])

function removeDiacritics(str: string) {
  return str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
}

function highlightText(text: string, keyword: string) {
  if (!keyword) return text

  const normalizedText = removeDiacritics(text)
  const normalizedKeyword = removeDiacritics(keyword.trim())
  const regex = new RegExp(`(${normalizedKeyword})`, 'gi')

  let result = ''
  let lastIndex = 0

  normalizedText.replace(regex, (match, _, index) => {
    result += text.slice(lastIndex, index)
    result += `<span class="highlight">${text.slice(index, index + match.length)}</span>`
    lastIndex = index + match.length
    return match
  })

  result += text.slice(lastIndex)
  return result
}

async function fetchDocument() {
  loading.value = true
  const response = await axios.get('https://localhost:7160/api/v1/Documents/filter', {
    params: {
      parentId: currentDocument.value?.Id,
      keyword: keyword.value,
      pageSize: pageSize.value,
      pageNumber: pageNumber.value,
      categoryId: categoryFilterId.value,
      type: typeFilter.value,
    },
  })
  loading.value = false

  documents.value = response.data.Items
  highlightTexts.value = documents.value.map((doc) => highlightText(doc.Name, keyword.value))
  store.commit('setTotalRecords', response.data.TotalItems)
  store.commit('setTotalPages', response.data.TotalPages)
}

onMounted(() => {
  fetchDocument()
})
</script>

<style scoped>
.main-container {
  height: 580px;
}
</style>
