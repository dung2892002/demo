<template>
  <div class="content--file">
    <div class="content__header header--shadow">
      <div>
        <img src="/src/assets/icon/left.png" @click="handleCloseForm(false)" />
        <span>Thêm mới tài liệu</span>
      </div>
    </div>
    <div class="file-upload-form">
      <div class="form">
        <ToastComponent ref="toastRef" />
        <div v-if="errors.length > 0" class="error-list">
          <span v-for="(err, index) in errors" :key="index" class="error-message"> {{ err }}</span>
        </div>
        <div class="form-data">
          <div
            class="file-upload"
            @click="openFileDialog"
            @dragover.prevent="handleDragOver"
            @dragenter.prevent="handleDragEnter"
            @dragleave.prevent="handleDragLeave"
            @drop.prevent="handleDrop"
            :class="[{ 'drag-active': isDragging }, { 'has-file': uploadedFiles.length > 0 }]"
          >
            <img
              src="https://testamisapp.misa.vn/ava-admin-mic/static/upload-c9bd497f.png"
              alt=""
            />
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
          <div class="file-category">
            <span class="form__label">Chủ đề <span class="required">*</span></span>
            <div class="category--current" @click="toggleShowSelectCategory">
              <span>{{ currentCategory ? currentCategory.Name : 'Chọn chủ đề' }}</span>
              <div>
                <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectCategory" />
                <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
              </div>
              <div class="category-data" v-if="showSelectCategory">
                <div
                  v-for="category in categories"
                  :key="category.Id"
                  @click.stop="selectCategory(category)"
                  :class="{ selected: currentCategory?.Id === category.Id }"
                >
                  <span> {{ category.Name }}</span>
                  <font-awesome-icon
                    :icon="['fas', 'check']"
                    v-if="currentCategory?.Id === category.Id"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="footer">
          <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
          <button class="button--complete" @click="handleSubmitForm">Lưu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import ToastComponent from '@/components/ToastComponent.vue'
import { DocumentType, type DocumentCategory } from '@/entities/Document'
import { getSrcIconDocument } from '@/utils'
import axios from 'axios'
import { onMounted, ref } from 'vue'

const emits = defineEmits(['closeForm'])

const currentCategory = ref<DocumentCategory | null>(null)

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const toastRef = ref<InstanceType<typeof ToastComponent> | null>(null)

const showSelectCategory = ref(false)

function toggleShowSelectCategory() {
  showSelectCategory.value = !showSelectCategory.value
}

function selectCategory(category: DocumentCategory) {
  currentCategory.value = category
  showSelectCategory.value = false
}

const props = defineProps({
  parentId: {
    type: String,
    required: true,
  },
})

async function handleSubmitForm() {
  if (checkAvailable()) {
    const formData = new FormData()
    uploadedFiles.value.forEach((file) => {
      formData.append('files', file)
    })
    formData.append('parentId', props.parentId)
    formData.append('categoryId', currentCategory.value!.Id)

    try {
      await axios.post('https://localhost:7160/api/v1/Documents/file', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      })

      handleCloseForm(true)
    } catch (error) {
      console.log(error)
    }
  }
}

function deleteFile(index: number) {
  uploadedFiles.value.splice(index, 1)
}

const categories = ref<DocumentCategory[]>([])

const fileInput = ref<HTMLInputElement | null>(null)
const uploadedFiles = ref<File[]>([])

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

const errors = ref<string[]>([])

const openFileDialog = (): void => {
  fileInput.value?.click()
}

const handleDragOver = (event: DragEvent): void => {
  event.preventDefault()
}

const isDragging = ref(false)
const handleDragEnter = (event: DragEvent): void => {
  event.preventDefault()
  isDragging.value = true
}

const handleDragLeave = (event: DragEvent): void => {
  event.preventDefault()
  isDragging.value = false
}

const handleDrop = (event: DragEvent): void => {
  event.preventDefault()
  isDragging.value = false

  const files = event.dataTransfer?.files
  if (!files) return

  processFiles(Array.from(files))
}

const processFiles = (files: File[]): void => {
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

const handleFileUpload = (event: Event): void => {
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
})
</script>
