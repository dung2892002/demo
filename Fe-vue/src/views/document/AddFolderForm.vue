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
              >Tên thư mục <span class="required">*</span></label
            >
            <input type="text" v-model="folderName" class="form__input" />
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy bỏ</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm">
          <span src="/src/assets/icon/refresh.png" alt="logo">Thêm</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { ref } from 'vue'

const emits = defineEmits(['closeForm'])

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const folderName = ref<string>('')

const props = defineProps({
  parentId: {
    type: String,
    required: true,
  },
})

async function handleSubmitForm() {
  try {
    await axios.post('https://localhost:7160/api/v1/Documents/folder', {
      Name: folderName.value,
      ParentId: props.parentId || null,
    })

    handleCloseForm(true)
  } catch (error) {
    console.error('Lỗi tải tệp:', error)
  }
}
</script>
