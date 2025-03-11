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
        <span v-if="error" class="error-message"> {{ error }}</span>
        <div class="form-data">
          <div class="file-upload" @click="openFileDialog">
            <img
              src="https://testamisapp.misa.vn/ava-admin-mic/static/upload-c9bd497f.png"
              alt=""
            />
            <span class="upload__title">Nhấp hoặc kéo thả tệp nguồn vào đây</span>
            <span class="upload__notice">
              Chỉ hỗ trợ các tệp có định dạng *.docx, *.doc và *.pdf
            </span>
            <input
              type="file"
              ref="fileInput"
              multiple
              style="display: none"
              @change="handleFileUpload"
            />
            <ul v-if="uploadedFiles.length">
              <li v-for="(file, index) in uploadedFiles" :key="index">
                {{ file.name }} ({{ formatFileSize(file.size) }})
              </li>
            </ul>
          </div>
          <div class="file-category">
            <span class="form__label">Chủ đề <span class="required">*</span></span>
            <div class="category--current" @click="toggleShowSelectCategory">
              <span>{{ currentCategory ? currentCategory.Name : 'Chọn chủ đề' }}</span>
              <div>
                <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectCategory" />
                <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
              </div>
            </div>
            <div class="category-data" v-if="showSelectCategory">
              <div
                v-for="category in categories"
                :key="category.Id"
                @click.stop="selectCategory(category)"
                :class="{ selected: currentCategory?.Id === category.Id }"
              >
                <span>{{ category.Name }}</span>
                <font-awesome-icon
                  :icon="['fas', 'check']"
                  v-if="currentCategory?.Id === category.Id"
                />
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
import { type DocumentCategory } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'

const emits = defineEmits(['closeForm'])

const currentCategory = ref<DocumentCategory | null>(null)

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

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

function checkAvailable() {
  if (uploadedFiles.value.length === 0) {
    error.value = 'Không có tệp nào được tải lên'
    return false
  }
  if (!currentCategory.value) {
    error.value = 'Chọn chủ đề cho các tài liệu'
    return false
  }

  return true
}

const categories = ref<DocumentCategory[]>([])

const fileInput = ref<HTMLInputElement | null>(null)
const uploadedFiles = ref<File[]>([])

const openFileDialog = (): void => {
  fileInput.value?.click()
}

const error = ref<string | null>(null)

const allowedFormats = ['application/pdf', 'application/msword']

const handleFileUpload = (event: Event): void => {
  const target = event.target as HTMLInputElement
  if (!target.files) return

  const files = Array.from(target.files)
  const validFiles = files.filter((file) => allowedFormats.includes(file.type))

  if (validFiles.length !== files.length) {
    error.value = 'Một số tệp không đúng định dạng (chỉ hỗ trợ .doc, .docx, .pdf)'
  } else {
    error.value = null
  }

  uploadedFiles.value.push(...validFiles)
}

const formatFileSize = (size: number): string => {
  if (size < 1024) return `${size} B`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`
  return `${(size / (1024 * 1024)).toFixed(1)} MB`
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
