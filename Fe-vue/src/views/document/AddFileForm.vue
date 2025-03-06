<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Thêm tệp</h2>
        <button class="form__button" @click="handleCloseForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <label for="employee-code" class="form__label"
              >Chủ đề <span class="required">*</span></label
            >
            <select v-model="categoryId" class="form__input">
              <option :value="category.Id" v-for="category in categories" :key="category.Id">
                {{ category.Name }}
              </option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item">
            <label class="form__label">Tải lên tệp <span class="required">*</span></label>
            <div class="file-upload">
              <input type="file" ref="fileInput" @change="handleFileUpload" class="hidden" />
            </div>
            <div
              class="preview"
              style="display: flex; flex-direction: row; align-items: center"
              v-if="selectedFile"
            >
              <img src="/src/assets/icon/file.png" alt="logo" style="width: 24px; height: 24px" />
              <span>{{ selectedFile.name }}</span>
            </div>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm">
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { type DocumentCategory } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'

const emits = defineEmits(['closeForm'])

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const categoryId = ref<string | null>(null)

const props = defineProps({
  parentId: {
    type: String,
    required: true,
  },
})

const categories = ref<DocumentCategory[]>([])

const fileInput = ref<HTMLInputElement | null>(null)
const selectedFile = ref<File | null>(null)

function handleFileUpload(event: Event) {
  const target = event.target as HTMLInputElement

  if (target.files && target.files.length > 0) {
    const file = target.files[0]

    if (!selectedFile.value || selectedFile.value.name !== file.name) {
      selectedFile.value = file
    }

    target.value = ''
  }
}

async function handleSubmitForm() {
  const formData = new FormData()
  formData.append('file', selectedFile.value!)
  formData.append('categoryId', categoryId.value!)
  formData.append('parentId', props.parentId)
  try {
    await axios.post('https://localhost:7160/api/v1/Documents/file', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })

    handleCloseForm(true)
  } catch (error) {
    console.error('Lỗi tải tệp:', error)
  }
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
