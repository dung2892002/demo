<template>
  <div class="content-main">
    <div class="file">
      <h2>Tất cả nội dung</h2>
      <div v-for="(block, index) in blocks" :key="index" class="file-item">
        <div style="padding-left: 10px; display: flex; flex-direction: row; gap: 8px">
          <div
            style="
              border-radius: 50%;
              background-color: #c6d9ec9c;
              width: 24px !important;
              height: 24px;
              text-align: center;
            "
          >
            {{ index + 1 }}
          </div>
          <span class="truncate-text" v-html="marked(block.Title)"></span>
        </div>
      </div>
    </div>
    <div class="file-data">
      <div class="file-content">
        <div class="file-content__body">
          <div v-for="(block, index) in blocks" :key="index" class="content-item">
            <div v-html="marked(block.Content)" class="markdown-container"></div>
            <button @click="showUpdateBlock(block)">
              <font-awesome-icon :icon="['fas', 'pen-to-square']" size="xl" />
            </button>
          </div>
        </div>
      </div>
      <div class="footer">
        <button @click="close" class="button--complete">Xác nhận</button>
      </div>
    </div>
  </div>

  <UpdateBlockForm v-if="updateBlock" :loading="loading" :content="newContent" @close-form="handleCloseForm" @submit-form="handleSubmitForm"/>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'
import UpdateBlockForm from './UpdateBlockForm.vue'
import { marked } from 'marked'

const updateBlock = ref<DocumentBlock | null>(null)
const loading = ref(false)


const newContent = ref<string | null>(null)

function showUpdateBlock(block: DocumentBlock) {
  updateBlock.value = block
  newContent.value = block.Content
}

function handleCloseForm(state: boolean) {
  updateBlock.value = null
  if (state) fetchBlocksData()
}

async function handleSubmitForm(data: string) {
  newContent.value = data
  const formData = new FormData()
  formData.append('newContent', newContent.value)
  console.log('FormData:', [...formData.entries()])

  try {
    loading.value = true
    await axios.patch(
      `https://localhost:7160/api/v1/Documents/blocks/${updateBlock.value!.Id}`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      },
    )
    loading.value = false

    handleCloseForm(true)
  } catch (e) {
    console.log(e)
  }
}

const emits = defineEmits(['close'])

const props = defineProps({
  documentId: {
    type: String,
    required: true,
  },
})

const blocks = ref<DocumentBlock[]>([])

async function fetchBlocksData() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Documents/blocks', {
      params: { documentId: props.documentId },
    })
    blocks.value = response.data
  } catch (error) {
    console.error(error)
  }
}

function close() {
  emits('close')
}

onMounted(() => {
  fetchBlocksData()
})
</script>
