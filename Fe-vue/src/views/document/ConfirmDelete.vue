<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Xóa tài liệu, thư mục</h2>
        <button class="form__button" @click="closeForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <span v-if="documents.length > 1">Các tài liệu/ thư mục đã chọn sẽ bị xóa</span>

            <span v-else>
              {{ documents[0].Type === DocumentType.Folder ? 'Thư mục' : 'Tài liệu' }}
              <span style="font-weight: bold">{{ documents[0].Name }}</span> sẽ bị xóa</span
            >
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="closeForm(false)">Hủy bỏ</button>
        <button class="button--complete" id="submitButton" @click="deleteDocument">
          <span src="/src/assets/icon/refresh.png" alt="logo">Xóa</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { DocumentType, type Document } from '@/entities/Document'
import axios from 'axios'
import type { PropType } from 'vue'

const props = defineProps({
  documents: {
    type: Array as PropType<Document[]>,
    required: true,
  },
})

const emits = defineEmits(['close'])

function closeForm(state: boolean) {
  emits('close', state)
}

async function deleteDocument() {
  const ids = props.documents.map((doc) => doc.Id)
  try {
    await axios.put('https://localhost:7160/api/v1/Documents/delete', ids)
    emits('close', true)
  } catch (error) {
    console.log(error)
  }
}
</script>
