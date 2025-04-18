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
            <span  class="form__label"
              >{{ document.Type === DocumentType.Folder ? 'Tên thư mục' : 'Tên tài liệu' }}
              <span class="required">*</span></span
            >
            <input type="text" v-model="document.Name" class="form__input" />
          </div>
        </div>
        <div class="form-group" v-if="document.Type != DocumentType.Folder">
          <div class="form__item">
            <div class="form-data data-select" style="width: 100%;" v-if="!loadCategories">
              <span class="form__label">Lĩnh vực <span class="required">*</span></span>
              <div class="value--current" @click.stop="toggleShowSelectCategory()" style="width: 100%;">
                <span>{{  currentCategory!.Name  }}</span>
                <div>
                  <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectCategory" />
                  <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
                </div>
                <div class="value-data more" v-if="showSelectCategory">
                  <div
                    v-for="category in categories"
                    :key="category.Id"
                    @click.stop="selectCategory(category)"
                    :class="{ selected: currentCategory!.Id === category.Id }"
                    class="data"
                  >
                    <span> {{ category.Name }}</span>
                    <font-awesome-icon
                      :icon="['fas', 'check']"
                      v-if="currentCategory!.Id === category.Id"
                    />
                  </div>
                </div>
              </div>
            </div>
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
    currentCategory.value = categories.value.find(
      (category) => category.Id === document.value?.CategoryId
    ) as DocumentCategory

    loadCategories.value = false
  } catch (error) {
    console.error('Lỗi :', error)
  }
}

const currentCategory = ref<DocumentCategory | null>(null)
const showSelectCategory = ref(false)

function toggleShowSelectCategory() {
  showSelectCategory.value = !showSelectCategory.value
}

function selectCategory(category: DocumentCategory) {
  currentCategory.value = category
  showSelectCategory.value = false
  document.value!.CategoryId = category.Id
}

const categories = ref<DocumentCategory[]>([])
const loadCategories = ref(true)

onMounted(() => {
  fetchData()
})
</script>
