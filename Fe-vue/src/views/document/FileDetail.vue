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
      :loading="loading"
      @update-file="handleUpdateFile"
      @close="closeFile"
      v-if="document.IsLaw"
      v-loading="loading"
    />
    <ContentReview v-else :documentId="document.Id!" @close="closeFile" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, type PropType, onUnmounted } from 'vue'
import axios from 'axios'
import { type Document, type DocumentBlock } from '@/entities/Document'

import FileReview from './FileReview.vue'
import ContentReview from './ContentReview.vue'

const loading = ref(true)

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

    await axios.put(`https://localhost:7160/api/v1/Documents/${props.document.Id}`, file)
    updateLoading.value = false
    closeFile(true)
  } catch (error) {
    console.error('Lỗi :', error)
  }
}


//lay danh sach block khi xem
async function fetchBlocksData() {
  if (!props.document || !props.document.Id) return

  try {
    // console.time('fetchBlocksData')
    const response = await axios.get('https://localhost:7160/api/v1/Documents/blocks', {
      params: { documentId: props.document.Id },
    })
    loading.value = false
    props.document.DocumentBlocks! = response.data
    // console.timeEnd('fetchBlocksData')
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => {
  fetchBlocksData()
})

onUnmounted(() => {
  props.document.DocumentBlocks! = []
})
</script>

