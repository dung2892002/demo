<template>
  <div class="content--file">
    <div class="content__header header--shadow">
      <div>
        <img src="/src/assets/icon/left.png" @click="handleCloseForm(false)" />
        <span>Thêm mới tài liệu</span>
      </div>
    </div>
    <UploadFile
      :categories="categories"
      :is-law="isLaw"
      :parent-id="parentId"
      @close-form="handleCloseForm"
      @select-knowled-type="handleSelectKnowledgeType"
      @select-type-upload="handleSelectTypeUpload"
      v-if="typeUpload === 1"
    />

    <UploadLink
      :categories="categories"
      :parent-id="parentId"
      @close-form="handleCloseForm"
      @select-knowled-type="handleSelectKnowledgeType"
      @select-type-upload="handleSelectTypeUpload"
      v-if="typeUpload === 2"
    ></UploadLink>

    <UploadContent
      :categories="categories"
      :parent-id="parentId"
      @close-form="handleCloseForm"
      @select-knowled-type="handleSelectKnowledgeType"
      @select-type-upload="handleSelectTypeUpload"
      v-if="typeUpload === 3"
    ></UploadContent>
  </div>
</template>

<script setup lang="ts">
import { type DocumentCategory } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'
import UploadFile from './UploadFile.vue'
import UploadLink from './UploadLink.vue'
import UploadContent from './UploadContent.vue'
const emits = defineEmits(['closeForm'])

const isLaw = ref(true)

const typeUpload = ref(1)

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

function handleSelectTypeUpload(type: number) {
  typeUpload.value = type
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  parentId: {
    type: String,
    required: true,
  },
})

function handleSelectKnowledgeType(state: boolean) {
  isLaw.value = state

  if (isLaw.value) {
    typeUpload.value = 1
  }
}

const categories = ref<DocumentCategory[]>([])

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
