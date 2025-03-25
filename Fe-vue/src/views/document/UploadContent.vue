<template>
  <div class="file-upload-form" v-if="docReviewId == null">
    <div class="form">
      <SelectTypeUpload :type-upload="typeUpload" @select-type-upload="selectTypeUpload" />
      <div class="form-data">
        <ToastComponent ref="toastRef" />
        <div>
          <SelectCategoryAndKnowledge
            :categories="categories"
            :is-law="false"
            @select-category="selectCategory"
            @select-knowled-type="selectLaw"
          />
          <div class="content--input" v-if="!importMode">
            <div class="form-group">
              <div class="form__item">
                <span class="form__label">Tiêu đề <span class="required">*</span></span>
                <input type="text" v-model="title" />
              </div>
            </div>

            <div class="form-group">
              <div class="form__item">
                <span class="form__label">Nội dung<span class="required">*</span></span>
                <textarea type="text" rows="10" v-model="content"></textarea>
              </div>
            </div>
          </div>
          <div
            v-else
            class="file-upload"
            style="margin-top: 20px"
            @click="openFileDialog"
            @dragover.prevent="handleDragOver"
            @dragenter.prevent="handleDragEnter"
            @dragleave.prevent="handleDragLeave"
            @drop.prevent="handleDrop"
            :class="[{ 'drag-active': isDragging }, { 'has-file': uploadedFiles.length > 0 }]"
          >
            <div v-if="uploadedFiles.length === 0">
              <img
                src="https://testamisapp.misa.vn/ava-admin-mic/static/upload-c9bd497f.png"
                alt=""
              />
              <div class="upload-description">
                <span class="upload__title">Nhấp hoặc kéo thả tệp nguồn vào đây</span>
                <span class="upload__notice">
                  Chương trình chỉ hỗ trợ các tệp có định dạng *.xls và *.xlsx
                </span>
              </div>
            </div>
            <div v-else class="review">
              <img
                src="/src/assets/icon/excel.png"
                alt="logo"
                style="width: 64px; height: 64px; margin-right: 6px; vertical-align: middle"
              />
              <div>{{ uploadedFiles[0].name }}</div>
              <span>({{ formatFileSize(uploadedFiles[0].size) }})</span>
              <button>Chọn lại tệp</button>
            </div>
            <input type="file" ref="fileInput" style="display: none" @change="handleFileUpload" />
          </div>

          <div class="footer">
            <div v-if="!importMode" class="more">
              <div class="buttons--left">
                <button class="button--cancel" @click="importMode = true">Nhập khẩu</button>
              </div>
              <div class="buttons--right">
                <button class="button--cancel" @click="handleCloseForm(true)">Hủy</button>
                <button class="button--cancel" @click="submitForm(false)">Lưu</button>
                <button class="button--complete" @click="submitForm(true)">Lưu và thêm</button>
              </div>
            </div>

            <div v-else class="more">
              <div></div>
              <div class="buttons--right">
                <button class="button--cancel" @click="importMode = false">Quay lại</button>
                <button class="button--complete" @click="submitFileForm">Tiếp tục</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <ContentReview :document-id="docReviewId!" @close="handleCloseForm(true)" v-else />
</template>

<script setup lang="ts">
import ToastComponent from '@/components/ToastComponent.vue'

import { type DocumentCategory } from '@/entities/Document'
import { ref, type PropType } from 'vue'
import SelectTypeUpload from './SelectTypeUpload.vue'
import SelectCategoryAndKnowledge from './SelectCategoryAndKnowledge.vue'
import axios from 'axios'
import ContentReview from './ContentReview.vue'

const typeUpload = ref(3)
const importMode = ref(false)

const currentCategory = ref<DocumentCategory | null>(null)

const fileInput = ref<HTMLInputElement | null>(null)
const uploadedFiles = ref<File[]>([])
const toastRef = ref<InstanceType<typeof ToastComponent> | null>(null)

const emits = defineEmits(['closeForm', 'selectKnowledType', 'selectTypeUpload'])

const docReviewId = ref<string | null>(null)

const props = defineProps({
  categories: {
    type: Array as PropType<DocumentCategory[]>,
    required: true,
  },
  parentId: {
    type: String,
    required: true,
  },
})

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
      toastRef.value?.addToastError(`File ${file.name} không đúng định dạng .xls, hoặc .xlsx`)
      return
    }

    uploadedFiles.value = [file]
  })

  if (uploadedFiles.value.length === 0) {
    toastRef.value?.addToastError('Không có tệp nào được tải lên')
  }
}

function handleFileUpload(event: Event): void {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  processFiles([file])

  target.value = ''
}

const allowedFormats = [
  'application/vnd.ms-excel', // .xls
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
]

const title = ref<string | null>(null)
const content = ref<string | null>(null)

const formatFileSize = (size: number): string => {
  if (size < 1024) return `${size} b`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} kb`
  return `${(size / (1024 * 1024)).toFixed(1)} mb`
}

function checkAvailable() {
  let check = true
  if (!currentCategory.value) {
    toastRef.value?.addToastError('Chưa chọn chủ đề cho tài liệu')
    check = false
  }
  if (uploadedFiles.value.length === 0 && importMode.value) {
    toastRef.value?.addToastError('Không có tệp nào được tải lên')
    check = false
  }

  if (!importMode.value && (!title.value || !content.value)) {
    toastRef.value?.addToastError('Chưa nhập tiêu đề hoặc nội dung cho tài liệu')
    check = false
  }

  return check
}

export interface AddContentRequest {
  Title: string
  Content: string
  ParentId: string | null
  CategoryId: string
}

async function submitForm(state: boolean) {
  if (!checkAvailable()) return
  const request: AddContentRequest = {
    Title: title.value!,
    Content: content.value!,
    CategoryId: currentCategory.value!.Id,
    ParentId: props.parentId ? props.parentId : null,
  }

  try {
    await axios.post('https://localhost:7160/api/v1/Documents/content', request)

    if (!state) handleCloseForm(true)
    else {
      toastRef.value?.addToastSuccess(`Thêm nội dung ${title.value} thành công`)
      title.value = null
      content.value = null
    }
  } catch (error) {
    console.log(error)
  }
}

async function submitFileForm() {
  if (!checkAvailable()) return

  const formData = new FormData()
  uploadedFiles.value.forEach((file) => {
    formData.append('files', file)
  })
  formData.append('parentId', props.parentId)
  formData.append('categoryId', currentCategory.value!.Id)

  try {
    const response = await axios.post(
      'https://localhost:7160/api/v1/Documents/content/file',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      },
    )

    docReviewId.value = response.data
  } catch (error) {
    console.log(error)
  }
}
</script>

<style lang="scss" scoped>
.review {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;

  div {
    font-weight: bold;
  }

  span {
    color: #6e737a;
  }

  button {
    background-color: transparent;
    color: #078cf8;
  }
}
</style>
