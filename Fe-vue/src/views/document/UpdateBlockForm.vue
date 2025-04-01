<template>
  <div class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">Nội dung</h2>
        <button class="form__button" @click="handleCloseForm">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <div
              class="input markdown-container"
              v-html="newContent"
              contenteditable="true"
              style="width: 600px; min-height: 200px;"
              @input="handleContentEditableInput"
            ></div>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm">Hủy</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm"  v-loading="loading">
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { marked } from 'marked';
import {  onMounted, ref } from 'vue';


const props = defineProps({
  content: {
    type: [String, null],
    required: true
  },
  loading : {
    type: Boolean,
    required: true
  }
})

const emits = defineEmits(['closeForm', 'submitForm'])


function handleContentEditableInput(event: Event) {
  newContent.value = (event.target as HTMLElement).innerHTML;
}


function handleCloseForm() {
  emits('closeForm')
}

import TurndownService from 'turndown';
function handleSubmitForm() {
  if (newContent.value) {
    const turndownService = new TurndownService();
    const markdown = turndownService.turndown(newContent.value);
    emits('submitForm', markdown);
  }
}

const newContent = ref<string|null>(null)


onMounted(async () => {
  if (props.content) {
    newContent.value = await marked.parse(props.content);
  }
})

</script>
