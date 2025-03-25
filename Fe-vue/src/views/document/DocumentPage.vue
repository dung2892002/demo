<template>
  <div class="content">
    <div class="content__header">
      <div class="content__filter">
        <div class="folder-route">
          <img
            src="/src/assets/icon/btn-prev-page.svg"
            alt=""
            v-if="currentDocument"
            @click="goPreRequest"
            style="cursor: pointer"
          />
          <span @click="backToRoot" :class="{ available: currentDocument }">Tài liệu</span>
          <div v-if="listParents.length > 1">
            <img
              src="/src/assets/icon/btn-next-page.svg"
              alt=""
              style="width: 16px; height: 16px"
            />
            <span style="font-weight: bold; font-size: 24px">...</span>
          </div>
          <div v-if="listParents.length > 0" @click="routeDocument(listParents[0])">
            <img
              src="/src/assets/icon/btn-next-page.svg"
              alt=""
              style="width: 16px; height: 16px"
            /><span class="available"> {{ listParents[0].Name }}</span>
          </div>
          <div alt="" v-if="currentDocument">
            <img
              src="/src/assets/icon/btn-next-page.svg"
              alt=""
              style="width: 16px; height: 16px"
            /><span> {{ currentDocument?.Name }}</span>
          </div>
        </div>
      </div>
      <div class="header-btn">
        <div class="btn btn--blue" @click="handleAddFile">
          <img src="/src/assets/icon/add-file.png" alt="logo" class="button--add-logo" />
          <span class="button--add-text">Thêm tệp</span>
        </div>
        <div class="btn btn--white" @click="handleAddFolder">
          <img src="/src/assets/icon/add-folder.png" alt="logo" class="button--add-logo" />
          <span class="button--add-text">Thêm thư mục</span>
        </div>
        <div class="btn btn--white" @click="handleDownloadListDocument" v-loading="downloadLoading">
          <font-awesome-icon :icon="['fas', 'download']" class="icon" />
        </div>
      </div>
    </div>
    <div class="content-main">
      <div class="toolbar">
        <input
          v-if="selectedDocuments.length === 0"
          type="text"
          placeholder="Tìm kiếm theo từ khóa"
          v-model="keyword"
          @input="handleInput"
          @keydown.enter="fetchDocument"
        />
        <div v-else class="toolbar--left">
          <font-awesome-icon :icon="['fas', 'arrow-turn-down']" class="toolbar-icon" />
          <div class="toolbar--selected-document">
            Đã chọn
            <span class="selected-document--value">{{ selectedDocuments.length }}</span> dữ liệu
          </div>
          <button class="toolbar__button--clear" @click="resetSelectedDocuments">Bỏ chọn</button>
          <div class="toolbar__buttons">
            <button class="toolbar-button" @click="deleteSelectedDocument">
              <font-awesome-icon :icon="['fas', 'trash']" /><span>Xóa</span>
            </button>
            <button class="toolbar-button" @click="moveSelectedDocument">
              <font-awesome-icon :icon="['fas', 'file-import']" /><span>Di chuyển</span>
            </button>
          </div>
        </div>

        <div class="toolbar__actions">
          <button class="toolbar-action" @click="fetchDocument()">
            <img src="/src/assets/icon/refresh.png" alt="logo" />
          </button>
        </div>
      </div>
      <div class="main-container" ref="tableContainer" v-loading="loading">
        <table class="employee-table">
          <thead>
            <tr>
              <th style="width: 40px">
                <span @click="selectAllDocuments" v-if="selectedDocuments.length === 0">
                  <font-awesome-icon :icon="['fas', 'square']" class="square"
                /></span>

                <span
                  @click="selectAllDocuments"
                  v-else-if="selectedDocuments.length < documents.length"
                  ><font-awesome-icon :icon="['fas', 'square-minus']" class="square--check"
                /></span>
                <span v-else @click="resetSelectedDocuments">
                  <font-awesome-icon :icon="['fas', 'square-check']" class="square--check" />
                </span>
              </th>
              <th>Tên tài liệu</th>
              <th class="w-20">Chủ đề</th>
              <th class="w-20">Phân loại</th>
              <th class="w-20">Ngày tạo</th>
              <th class="w-30" v-if="keyword.trim().length > 0">Vị trí</th>
              <th class="w-15">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(document, index) in documents"
              :key="index"
              @dblclick="handleSelectDocument(document)"
              style="cursor: pointer"
            >
              <td style="text-align: center; width: 40px">
                <span v-if="!checkDocumentSelected(document)" @click="selectDocument(document)">
                  <font-awesome-icon :icon="['fas', 'square']" class="square" />
                </span>
                <span v-else @click="removeSelectDocument(document)">
                  <font-awesome-icon :icon="['fas', 'square-check']" class="square--check" />
                </span>
              </td>
              <td>
                <img
                  :src="`/src/assets/icon/${getSrcIconDocument(document.Type)}`"
                  alt="logo"
                  style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
                />
                <span
                  class="truncate-text"
                  style="vertical-align: middle"
                  v-html="highlightTexts[index]"
                  @mouseenter="handleShowPopupText(document.Name, $event)"
                  @mouseleave="handleClosePopupText"
                ></span>
              </td>
              <td>{{ document.Category?.Name }}</td>
              <td>
                {{
                  document.Type === DocumentType.Folder
                    ? ''
                    : document.IsLaw
                      ? 'Văn bản quy phạm pháp luật'
                      : 'Tri thức nghiệp vụ khác'
                }}
              </td>
              <td style="text-align: center">
                {{ document.CreatedAt ? formatDate(document.CreatedAt) : '' }}
              </td>
              <td v-if="keyword.trim().length > 0">
                <span
                  @mouseenter="handleShowPopupText(document.FolderPath, $event)"
                  @mouseleave="handleClosePopupText"
                  >{{ document.FolderPath }}</span
                >
              </td>
              <td>
                <div class="action" :ref="`action-${index}`">
                  <div class="action-buttons">
                    <button
                      class="action-button"
                      @click.stop="togglePopupAction(index, $event)"
                      @dblclick.stop
                    >
                      <font-awesome-icon
                        :icon="['fas', 'ellipsis-vertical']"
                        class="button__icon"
                        :class="{ selected: showPopupAction === index }"
                      />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span @click="handleSelectDocument(document)"
                        ><font-awesome-icon :icon="['fas', 'eye']" /> <span>Xem</span></span
                      >
                      <span @click="handleUpdateDocument(document)"
                        ><font-awesome-icon icon="pen-to-square" /><span>Sửa</span></span
                      >

                      <span @click="handleMoveDocument(document)"
                        ><font-awesome-icon :icon="['fas', 'file-import']" /><span
                          >Di chuyển</span
                        ></span
                      >
                      <span
                        @click="handleDeleteDocument(document)"
                        v-loading="deleteLoading == index"
                        ><font-awesome-icon :icon="['fas', 'trash']" /><span>Xóa</span></span
                      >
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
        :pageLoading="false"
        @pageChange="handlePageChange"
        @pageSizeChange="handlePageSizeChange"
      />
    </div>

    <div
      v-if="showTextPopup"
      class="popup-text"
      :style="{ top: textPopupPosition.top + 'px', left: textPopupPosition.left + 'px' }"
    >
      <span>{{ textPopupData }}</span>

      <div class="popup-arrow"></div>
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

    <MoveDocument v-if="showMoveDocumentForm" @close-form="closeForm" :documents="moveDocuments" />

    <ConfirmDelete
      v-if="showConfirmDelete"
      @close="closeForm"
      :documents="deleteDocuments"
    ></ConfirmDelete>
  </div>
  <FileDetail v-if="showDocumentDetail" :document="documentDetail" @close-file="closeFile" />
</template>

<script setup lang="ts">
import '/src/styles/component/input.scss'
import '/src/styles/layout/header.scss'
import ThePagnigation from '@/components/ThePagnigation.vue'
import { DocumentType, type Document } from '@/entities/Document'
import { formatDate, formatDateForm, getSrcIconDocument } from '@/utils'
import axios from 'axios'
import { onMounted, onUnmounted, ref } from 'vue'
import { useStore } from 'vuex'
import AddFileForm from './AddFileForm.vue'
import AddFolderForm from './AddFolderForm.vue'
import MoveDocument from './MoveDocument.vue'
import FileDetail from './FileDetail.vue'
import UpdateForm from './UpdateForm.vue'
import ConfirmDelete from './ConfirmDelete.vue'

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

const listParents = ref<Document[]>([])

const showAddFileForm = ref(false)
const showAddFolderForm = ref(false)
const showMoveDocumentForm = ref(false)
const showUpdateForm = ref(false)
const downloadLoading = ref(false)

const selectedDocuments = ref<Document[]>([])

function selectAllDocuments() {
  selectedDocuments.value = documents.value
}

function resetSelectedDocuments() {
  selectedDocuments.value = []
}

function selectDocument(document: Document) {
  selectedDocuments.value.push(document)
}

function removeSelectDocument(document: Document) {
  const index = selectedDocuments.value.findIndex((doc) => doc.Id === document.Id)
  if (index > -1) {
    selectedDocuments.value = [
      ...selectedDocuments.value.slice(0, index),
      ...selectedDocuments.value.slice(index + 1),
    ]
  }
}

function checkDocumentSelected(document: Document) {
  return selectedDocuments.value.some((doc) => doc.Id === document.Id)
}

let timeout: ReturnType<typeof setTimeout> | null = null

const textPopupPosition = ref({
  top: 0,
  left: 0,
})

const showTextPopup = ref(false)
const textPopupData = ref<string | null>(null)

async function handleDownloadListDocument() {
  downloadLoading.value = true
  const response = await axios.get('https://localhost:7160/api/v1/Documents/export', {
    params: {
      folderId: currentDocument.value?.Id,
      keyword: keyword.value,
    },
    responseType: 'blob',
  })
  downloadLoading.value = false
  const url = window.URL.createObjectURL(new Blob([response.data]))
  const link = document.createElement('a')
  link.href = url
  link.setAttribute('download', `Danh sách tài liệu.xlsx`)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

function handleShowPopupText(data: string, event: FocusEvent) {
  if (keyword.value == '') return
  const target = event.target instanceof HTMLElement ? event.target : null
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  textPopupPosition.value = {
    top: buttonRect.top - 40,
    left: buttonRect.left + buttonRect.width / 2,
  }

  textPopupData.value = data
  showTextPopup.value = true
}

function handleClosePopupText() {
  textPopupData.value = null
  showTextPopup.value = false
}

function handleInput() {
  if (timeout) {
    clearTimeout(timeout)
  }

  timeout = setTimeout(() => {
    fetchDocument()
    handleAddRequest()
  }, 500)
}

interface Request {
  document: Document | null
  keyword: string
}

const requests = ref<Request[]>([
  {
    document: null,
    keyword: '',
  },
])

function goPreRequest() {
  requests.value.pop()

  const preRequest = requests.value[requests.value.length - 1]
  if (preRequest) {
    keyword.value = preRequest.keyword
    currentDocument.value = preRequest.document

    fetchDocument()

    if (preRequest.document?.Id) {
      fetchParentFolders(preRequest.document.Id!)
    } else {
      listParents.value = []
      store.dispatch('setupListFolder', listParents.value)
    }
  }
}

const moveDocuments = ref<Document[]>([])

const documentDetail = ref<Document>({
  Id: '',
  Name: '',
  Type: DocumentType.Unknown,
  CategoryId: null,
  CreatedAt: '',
  FolderPath: '',
  DocumentBlocks: [],
  DocumentNo: null,
  IssueDate: null,
  SignerName: null,
  Issuer: null,
  IsLaw: false,
})

const showDocumentDetail = ref(false)

const popupPosition = ref({
  top: 0,
  right: 100,
})

const showPopupAction = ref<number>(-1)

function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - buttonRect.height,
    right: window.innerWidth - buttonRect.left,
  }

  showPopupAction.value = showPopupAction.value === index ? -1 : index
}

function closePopupAction() {
  if (showPopupAction.value != -1) showPopupAction.value = -1
}

async function handleUpdateDocument(document: Document) {
  documentUpdatedId.value = document.Id
  showUpdateForm.value = true
}

const deleteDocuments = ref<Document[]>([])
const showConfirmDelete = ref(false)

function deleteSelectedDocument() {
  deleteDocuments.value = selectedDocuments.value
  showConfirmDelete.value = true
}

async function handleDeleteDocument(document: Document) {
  deleteDocuments.value = []
  deleteDocuments.value.push(document)
  showConfirmDelete.value = true
}

function moveSelectedDocument() {
  moveDocuments.value = selectedDocuments.value
  showMoveDocumentForm.value = true
}

function handleMoveDocument(document: Document) {
  moveDocuments.value = []
  moveDocuments.value.push(document)
  showMoveDocumentForm.value = true
}

function handleAddFile() {
  showAddFileForm.value = true
}

function handleAddFolder() {
  showAddFolderForm.value = true
}

function closeForm(state: boolean) {
  showAddFileForm.value = false
  showAddFolderForm.value = false
  showMoveDocumentForm.value = false
  showUpdateForm.value = false
  showConfirmDelete.value = false
  if (state) fetchDocument()
}

function closeFile(state: boolean) {
  showDocumentDetail.value = false
  documentDetail.value = {
    Id: '',
    Name: '',
    Type: DocumentType.Unknown,
    CategoryId: null,
    CreatedAt: '',
    FolderPath: '',
    DocumentBlocks: [],
    DocumentNo: null,
    IssueDate: null,
    SignerName: null,
    Issuer: null,
    IsLaw: false,
  }

  if (state == true) fetchDocument()
}

function handleAddRequest() {
  requests.value.push({
    document: currentDocument.value || null,
    keyword: keyword.value,
  })
}

function handleSelectDocument(document: Document) {
  if (document.Type === DocumentType.Folder) {
    resetQuery()
    currentDocument.value = document
    showPopupAction.value = -1
    keyword.value = ''
    fetchDocument()
    fetchParentFolders(document.Id!)
    handleAddRequest()
  } else {
    documentDetail.value = document
    documentDetail.value.IssueDate = formatDateForm(documentDetail.value.IssueDate!)
    documentDetail.value.MarkdownContent = ''
    showDocumentDetail.value = true
  }
}

function routeDocument(document: Document) {
  resetQuery()
  currentDocument.value = document
  fetchParentFolders(document.Id!)
  fetchDocument()
}

function backToRoot() {
  if (currentDocument.value) {
    resetQuery()
    requests.value = [
      {
        document: null,
        keyword: '',
      },
    ]
    listParents.value = []
    store.dispatch('setupListFolder', listParents.value)
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

async function fetchParentFolders(id: string) {
  try {
    const response = await axios.get(`https://localhost:7160/api/v1/Documents/parents`, {
      params: {
        id: id,
      },
    })

    listParents.value = response.data
    store.dispatch('setupListFolder', listParents.value)
  } catch (error) {
    console.log(error)
  }
}

async function fetchDocument() {
  showPopupAction.value = -1
  loading.value = true
  selectedDocuments.value = []
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
  document.addEventListener('click', closePopupAction)
  document.addEventListener('scroll', closePopupAction)
})

onUnmounted(() => {
  document.removeEventListener('click', closePopupAction)
  document.removeEventListener('scroll', closePopupAction)
})
</script>

<style scoped lang="scss">
.square {
  color: #ffffff !important;
  border: 1px solid gray !important;
  border-radius: 2px;
  cursor: pointer;
  box-sizing: border-box;
  padding: 0;
  width: 14px;
  height: 14px;
  &:hover {
    border-color: #078cf8 !important;
  }
}

.square--check {
  color: #078cf8;
  border-radius: 2px;
  width: 16px;
  height: 16px;
  box-sizing: border-box;
  cursor: pointer;
}
</style>
