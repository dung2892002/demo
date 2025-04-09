<template>
  <div class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">{{ title }}</h2>
        <button class="form__button" @click="handleCloseForm">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>

      <form class="cukcuk-form">
        <div class="file-category" v-if="props.state > 4">
          <span class="form__label">Điều<span class="required">*</span></span>
          <div class="category--current">
            <div>
              <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectArticle" />
              <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
            </div>
            <div class="category-data more" v-if="showSelectArticle">
              <div
                v-for="article in articles"
                :key="article.Id!"
                @click.stop="selectArticle(article)"
                :class="{ selected: currentArticle?.Id === article.Id }"
              >
                <span> {{ article.Content }}</span>
                <font-awesome-icon
                  :icon="['fas', 'check']"
                  v-if="currentArticle?.Id === article.Id"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="file-category" v-if="props.state > 5">
          <span class="form__label">Khoản<span class="required">*</span></span>
          <div class="category--current">
            <div>
              <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectClause" />
              <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
            </div>
            <div class="category-data more" v-if="showSelectClause">
              <div
                v-for="clause in clauses"
                :key="clause.Id!"
                @click.stop="selectClause(clause)"
                :class="{ selected: currentClause?.Id === clause.Id }"
              >
                <span> {{ clause.Content }}</span>
                <font-awesome-icon
                  :icon="['fas', 'check']"
                  v-if="currentClause?.Id === clause.Id"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="form-buttons">
          <div class="form-buttons-label" >Nội dung</div>
          <div class="form-buttons-list">
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
import {  onBeforeUnmount, onMounted, ref, type PropType } from 'vue';
import { Editor, EditorContent } from '@tiptap/vue-3';
import Bold from '@tiptap/extension-bold';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';
import type { DocumentBlock } from '@/entities/Document';

const props = defineProps({
  block: {
    type: Object as PropType<DocumentBlock | null>,
    required: true
  },
  loading : {
    type: Boolean,
    required: true
  },
  state: {
    type: Number,
    required: true
  },
  blocksToSelect: {
    type: Array as PropType<DocumentBlock[]>,
    required: true
  },
})

const showSelectArticle = ref(false)
const showSelectClause = ref(false)

const articles = ref<DocumentBlock[]>([])
const clauses = ref<DocumentBlock[]>([])

const currentArticle = ref<DocumentBlock | null>(null)
const currentClause = ref<DocumentBlock | null>(null)

function selectArticle(article: DocumentBlock) {
  currentArticle.value = article
  showSelectArticle.value = false
}

function selectClause(clause: DocumentBlock) {
  currentClause.value = clause
  showSelectClause.value = false
}

const emits = defineEmits(['closeForm', 'submitForm'])
const editor = ref<Editor | undefined>(undefined);

const title = ref<string>('Nội dung')

function getTitle() {
  if (props.state === 1 || props.state === 2 || props.state === 3 || props.state === 4) {
    title.value = 'Thêm phân đoạn'
    return
  }

  if (props.state === 5) {
    title.value = 'Thêm mới Khoản'
    return
  }
  if (props.state === 6) {
    title.value = 'Thêm mới Điểm'
    return
  }

  title.value = `Sửa ${getTitleByLevel(props.block!.Level)}`
}

function getTitleByLevel(level: number) {
  switch (level) {
    case 1:
      return 'Phần'
    case 2:
      return 'Chương'
    case 3:
      return 'Mục'
    case 4:
      return 'Tiểu mục'
    case 5:
      return 'Điều'
    case 6:
      return 'Khoản'
    case 7:
      return 'Điểm'
    default:
      return 'phân đoạn'
  }
}


function handleCloseForm() {
  emits('closeForm')
}

function handleSubmitForm() {
    emits('submitForm', newContent.value?.trim());
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
  if (props.block) {
    newContent.value = await marked.parse(props.block.Content);
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

  getTitle()

  articles.value = props.blocksToSelect.filter((block) => block.Level === 5);
  clauses.value = props.blocksToSelect.filter((block) => block.Level === 6);
});


onBeforeUnmount(() => {
  if (editor.value) {
    editor.value.destroy();
  }
});

</script>
