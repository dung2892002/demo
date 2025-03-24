<template>
  <div class="file-upload-form" v-if="stepUploadLaw === 1">
    <div class="form">
      <SelectTypeUpload
        :type-upload="typeUpload"
        @select-type-upload="selectTypeUpload"
        v-if="!isLaw"
      />
      <div class="form-data">
        <ToastComponent ref="toastRef" />
        <div
          class="file-upload"
          @click="openFileDialog"
          @dragover.prevent="handleDragOver"
          @dragenter.prevent="handleDragEnter"
          @dragleave.prevent="handleDragLeave"
          @drop.prevent="handleDrop"
          :class="[{ 'drag-active': isDragging }, { 'has-file': uploadedFiles.length > 0 }]"
        >
          <img src="https://testamisapp.misa.vn/ava-admin-mic/static/upload-c9bd497f.png" alt="" />
          <div class="upload-description">
            <span class="upload__title">Nhấp hoặc kéo thả tệp nguồn vào đây</span>
            <span class="upload__notice">
              Chương trình chỉ hỗ trợ các tệp có định dạng *.docx, *.doc và *.pdf
            </span>
          </div>
          <input
            type="file"
            ref="fileInput"
            multiple
            style="display: none"
            @change="handleFileUpload"
          />
        </div>
        <div class="file-review" v-if="uploadedFiles.length > 0">
          <table>
            <thead>
              <tr>
                <th class="w-38">Tên tài liệu</th>
                <th class="w-15">Tình trạng</th>
                <th class="w-10">Kích cỡ</th>
                <th class="w-10 button">Hoạt động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(file, index) in uploadedFiles" :key="index">
                <td class="name">
                  <img
                    :src="`/src/assets/icon/${getIconFile(file)}`"
                    alt="logo"
                    style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
                  />{{ file.name }}
                </td>
                <td>Hoàn tất</td>
                <td>{{ formatFileSize(file.size) }}</td>
                <td @click="deleteFile(index)" class="button">
                  <font-awesome-icon :icon="['fas', 'trash-can']" style="color: red" />
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <SelectCategoryAndKnowledge
          :categories="categories"
          :is-law="isLaw"
          @select-category="selectCategory"
          @select-knowled-type="selectLaw"
        />
        <div class="footer">
          <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
          <button class="button--complete" @click="handleSubmitForm" v-loading="saveLoading">
            Lưu
          </button>
        </div>
      </div>
    </div>
  </div>
  <div>
    <FileReview
      v-if="documentsDemo.length > 0"
      :documents="documentsDemo"
      :state="true"
      :confirmLoading="confirmLoading"
      :cancelLoading="cancelLoading"
      @cancel-upload="cancelUpload"
      @confirm-upload="confirmUpload"
    />
  </div>
</template>

<script setup lang="ts">
import ToastComponent from '@/components/ToastComponent.vue'
import { DocumentType, type Document, type DocumentCategory } from '@/entities/Document'
import { formatDateForm, getSrcIconDocument } from '@/utils'
import axios from 'axios'
import { ref, type PropType } from 'vue'
import FileReview from './FileReview.vue'
import SelectTypeUpload from './SelectTypeUpload.vue'
import SelectCategoryAndKnowledge from './SelectCategoryAndKnowledge.vue'

const fileInput = ref<HTMLInputElement | null>(null)
const uploadedFiles = ref<File[]>([])
const toastRef = ref<InstanceType<typeof ToastComponent> | null>(null)

const currentCategory = ref<DocumentCategory | null>(null)
const stepUploadLaw = ref(1)
const saveLoading = ref(false)
const typeUpload = ref(1)

const props = defineProps({
  categories: {
    type: Array as PropType<DocumentCategory[]>,
    required: true,
  },

  isLaw: {
    type: Boolean,
    required: true,
  },

  parentId: {
    type: String,
    required: true,
  },
})

const emits = defineEmits(['closeForm', 'selectKnowledType', 'selectTypeUpload'])

function selectTypeUpload(type: number) {
  typeUpload.value = type
  emits('selectTypeUpload', type)
}

function selectCategory(category: DocumentCategory) {
  currentCategory.value = category
}

function selectLaw(state: boolean) {
  emits('selectKnowledType', state)
}

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

function deleteFile(index: number) {
  uploadedFiles.value.splice(index, 1)
}

const allowedFormats = [
  'application/pdf',
  'application/msword',
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
]

function checkAvailable() {
  let check = true
  if (uploadedFiles.value.length === 0) {
    toastRef.value?.addToastError('Không có tệp nào được tải lên')
    check = false
  }
  if (!currentCategory.value) {
    toastRef.value?.addToastError('Chưa chọn chủ đề cho tài liệu')
    check = false
  }

  return check
}

function getIconFile(file: File) {
  if (file.type === 'application/pdf') return getSrcIconDocument(DocumentType.Pdf)
  return getSrcIconDocument(DocumentType.Word)
}

function openFileDialog(): void {
  fileInput.value?.click()
}

function handleDragOver(event: DragEvent): void {
  event.preventDefault()
}

const isDragging = ref(false)
function handleDragEnter(event: DragEvent): void {
  event.preventDefault()
  isDragging.value = true
}

function handleDragLeave(event: DragEvent): void {
  event.preventDefault()
  isDragging.value = false
}

function handleDrop(event: DragEvent): void {
  event.preventDefault()
  isDragging.value = false

  const files = event.dataTransfer?.files
  if (!files) return

  processFiles(Array.from(files))
}

function processFiles(files: File[]): void {
  files.forEach((file) => {
    if (!allowedFormats.includes(file.type)) {
      toastRef.value?.addToastError(`File ${file.name} không đúng định dạng .doc, .docx hoặc .pdf`)
      return
    }

    const isFileExist = uploadedFiles.value.some((existingFile) => existingFile.name === file.name)

    if (isFileExist) {
      toastRef.value?.addToastError(`File ${file.name} đã được tải lên trước đó`)
    } else {
      uploadedFiles.value.push(file)
    }
  })

  if (uploadedFiles.value.length === 0) {
    toastRef.value?.addToastError('Không có tệp nào được tải lên')
  }
}

function handleFileUpload(event: Event): void {
  const target = event.target as HTMLInputElement
  if (!target.files) return

  const files = Array.from(target.files)
  processFiles(files)

  target.value = ''
}

const formatFileSize = (size: number): string => {
  if (size < 1024) return `${size} b`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} kb`
  return `${(size / (1024 * 1024)).toFixed(1)} mb`
}

async function handleSubmitForm() {
  if (!checkAvailable()) return
  if (props.isLaw) await SubmitFormLaw()
  else await SubmitForm()
}

async function SubmitForm() {
  const formData = new FormData()
  uploadedFiles.value.forEach((file) => {
    formData.append('files', file)
  })
  formData.append('parentId', props.parentId)
  formData.append('categoryId', currentCategory.value!.Id)

  try {
    saveLoading.value = true
    await axios.post('https://localhost:7160/api/v1/Documents/file', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    saveLoading.value = false
    handleCloseForm(true)
  } catch (error) {
    console.log(error)
  }
}

async function SubmitFormLaw() {
  const formData = new FormData()
  uploadedFiles.value.forEach((file) => {
    formData.append('files', file)
  })
  formData.append('parentId', props.parentId)
  formData.append('categoryId', currentCategory.value!.Id)

  try {
    saveLoading.value = true
    const response = await axios.post('https://localhost:7160/api/v1/Documents/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    cacheId.value = response.data.CacheDataId
    documentsDemo.value = response.data.Documents.map((document: Document) => {
      document.IssueDate = formatDateForm(document.IssueDate!)
      document.MarkdownContent = ''
      return document
    })
    saveLoading.value = false

    stepUploadLaw.value = 2
  } catch (error) {
    console.log(error)
  }
}

const cacheId = ref<string | null>(null)

const documentsDemo = ref<Document[]>([])

const confirmLoading = ref(false)
const cancelLoading = ref(false)

async function confirmUpload() {
  try {
    confirmLoading.value = true
    await axios.post(
      'https://localhost:7160/api/v1/Documents/upload-confirm',
      {},
      {
        params: {
          cacheId: cacheId.value,
        },
      },
    )
    confirmLoading.value = false
    handleCloseForm(true)
  } catch (error) {
    console.log(error)
  }
}

async function cancelUpload() {
  try {
    cancelLoading.value = true
    await axios.post(
      'https://localhost:7160/api/v1/Documents/upload-cancel',
      {},
      {
        params: {
          cacheId: cacheId.value,
        },
      },
    )
    cancelLoading.value = false
    handleCloseForm(true)
  } catch (error) {
    console.log(error)
  }
}
</script>
