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
          <span class="truncate-text">{{ block.Title }}</span>
        </div>
      </div>
    </div>
    <div class="file-data">
      <div class="file-content">
        <div class="file-content__body">
          <div v-for="(block, index) in blocks" :key="index" class="content-item">
            <span>{{ block.Content }}</span>
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
  <div v-if="updateBlock" class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">Nội dung</h2>
        <button class="form__button" @click="handleCloseForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <textarea type="text" rows="10" v-model="newContent" style="width: 600px"></textarea>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
        <button
          class="button--complete"
          id="submitButton"
          @click="handleSubmitForm"
          v-loading="loading"
        >
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref } from 'vue'

const updateBlock = ref<DocumentBlock | null>(null)
const loading = ref(false)

const newContent = ref('')

function showUpdateBlock(block: DocumentBlock) {
  updateBlock.value = block
  newContent.value = block.Content
}

function handleCloseForm(state: boolean) {
  updateBlock.value = null
  if (state) fetchBlocksData()
}

async function handleSubmitForm() {
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
