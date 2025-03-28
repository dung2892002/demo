<template>
  <div class="content--file">
    <div class="content__header">
      <div>
        <img src="/src/assets/icon/left.png" @click="closeFile(false)" />
        <span>Xem tài liệu</span>
      </div>
    </div>
    <FileReview
      :documents="[document]"
      :state="false"
      :update-loading="updateLoading"
      @update-file="handleUpdateFile"
      @close="closeFile"
      v-if="document.IsLaw"
    />
    <ContentReview v-else :documentId="document.Id!" @close="closeFile" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, type PropType } from 'vue'
import axios from 'axios'
import { type DocumentCategory, type Document, type DocumentBlock } from '@/entities/Document'

import FileReview from './FileReview.vue'
import ContentReview from './ContentReview.vue'

const props = defineProps({
  document: {
    type: Object as PropType<Document>,
    required: true,
  },
})

const emits = defineEmits(['closeFile'])

function closeFile(state: boolean) {
  emits('closeFile', state)
}

const updateLoading = ref(false)

async function handleUpdateFile(file: Document, blocks: DocumentBlock[]) {
  try {
    updateLoading.value = true
    file.DocumentBlocks = blocks
    console.log(file)
    await axios.put(`https://localhost:7160/api/v1/Documents/${props.document.Id}`, file)
    updateLoading.value = false
    closeFile(true)
  } catch (error) {
    console.error('Lỗi :', error)
  }
}

const categories = ref<DocumentCategory[]>([])

async function fetchCategories() {
  try {
    const response = await axios.get(`https://localhost:7160/api/v1/Documents/categories`)
    categories.value = response.data
  } catch (error) {
    console.error('Lỗi khi lấy nội dung Markdown:', error)
  }
}

//lay danh sach block khi xem
async function fetchBlocksData() {
  if (!props.document || !props.document.Id) return

  try {
    const response = await axios.get('https://localhost:7160/api/v1/Documents/blocks', {
      params: { documentId: props.document.Id },
    })
    props.document.DocumentBlocks! = response.data
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => {
  fetchCategories()
  fetchBlocksData()
})
</script>

<style scoped lang="scss"></style>
