<template>
  <div class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">Nội dung</h2>
        <button class="form__button" @click="handleCloseForm">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form">
        <div class="form-buttons">
          <div class="form-buttons">
      <button
        @click.prevent="toggleFormat('bold')"
        :disabled="!editor?.can().toggleBold()"
      >
        <font-awesome-icon :icon="['fas', 'bold']" />
      </button>
      <button
        @click.prevent="toggleFormat('italic')"
        :disabled="!editor?.can().toggleItalic()"
      >
        <font-awesome-icon :icon="['fas', 'italic']" />
      </button>
      <button
        @click.prevent="toggleFormat('underline')"
        :disabled="!editor?.can().toggleUnderline()"
      >
        <font-awesome-icon :icon="['fas', 'underline']" />
      </button>
      <button
        @click.prevent="toggleFormat('strike')"
        :disabled="!editor?.can().toggleStrike()"
      >
        <font-awesome-icon :icon="['fas', 'strikethrough']" />
      </button>
    </div>
        </div>
        <div class="form-group">
          <EditorContent :editor="editor" class="markdown-container" />
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
import {  onBeforeUnmount, onMounted, ref } from 'vue';
import { Editor, EditorContent } from '@tiptap/vue-3';
import Bold from '@tiptap/extension-bold';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';

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
const editor = ref<Editor | undefined>(undefined);




function handleCloseForm() {
  emits('closeForm')
}

function handleSubmitForm() {

    emits('submitForm', newContent.value);
}

const newContent = ref<string|null>(null)

function toggleFormat(mark: string) {
  if (!editor.value) return;

  const { from, to } = editor.value.state.selection;
  const isActive = editor.value.isActive(mark);

  editor.value
    .chain()
    .focus()
    .setTextSelection({ from, to })
    [isActive ? 'unsetMark' : 'setMark'](mark)
    .run();
}

onMounted(async () => {
  if (props.content) {
    newContent.value = await marked.parse(props.content);
    console.log(newContent.value)
  }
  editor.value = new Editor({
    extensions: [
      StarterKit.configure({
        bold: false,
        heading: false,     // Tắt tự động định dạng tiêu đề
        bulletList: false,  // Tắt danh sách không thứ tự (ul)
        orderedList: false, // Tắt danh sách có thứ tự (ol)
        blockquote: false,  // Tắt trích dẫn
        codeBlock: false,
      }),
      Underline,
      Bold
    ],
    content: newContent.value,
    onUpdate: ({ editor }) => {
      newContent.value = editor.getHTML();
    },
  });
});

onBeforeUnmount(() => {
  if (editor.value) {
    editor.value.destroy();
  }
});

</script>
