<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">
          {{ document?.Type === DocumentType.Folder ? 'Sửa thư mục' : 'Chỉnh sửa tài liệu' }}
        </h2>
        <button class="form__button" @click="handleCloseForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form" v-if="document">
        <div class="form-group">
          <div class="form__item">
            <label for="employee-code" class="form__label"
              >{{ document.Type === DocumentType.Folder ? 'Tên thư mục' : 'Tên tài liệu' }}
              <span class="required">*</span></label
            >
            <input type="text" v-model="document.Name" class="form__input" />
          </div>
        </div>
        <div class="form-group" v-if="document.Type != DocumentType.Folder">
          <div class="form__item">
            <label for="employee-code" class="form__label"
              >Chủ đề <span class="required">*</span></label
            >
            <select v-model="document.CategoryId" class="form__input">
              <option :value="category.Id" v-for="category in categories" :key="category.Id">
                {{ category.Name }}
              </option>
            </select>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy bỏ</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm">
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { type DocumentCategory, type Document, DocumentType } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'

const emits = defineEmits(['closeForm'])

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const props = defineProps({
  id: {
    type: String,
    required: true,
  },
})

async function handleSubmitForm() {
  try {
    await axios.put(`https://localhost:7160/api/v1/Documents/${props.id}`, document.value)

    handleCloseForm(true)
  } catch (error) {
    console.error('Lỗi :', error)
  }
}
const document = ref<Document | null>(null)

async function fetchData() {
  try {
    const response = await axios.get(`https://localhost:7160/api/v1/Documents/${props.id}`)
    document.value = response.data

    if (document.value?.Type != DocumentType.Folder) fetchCategoriesData()
  } catch (error) {
    console.error('Lỗi :', error)
  }
}

async function fetchCategoriesData() {
  try {
    const response = await axios.get(`https://localhost:7160/api/v1/Documents/categories`)
    categories.value = response.data
  } catch (error) {
    console.error('Lỗi :', error)
  }
}

const categories = ref<DocumentCategory[]>([])

onMounted(() => {
  fetchData()
})
</script>
