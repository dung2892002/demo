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
        <div class="file-category" v-if="props.block.Level >= 6" style="width: 100%;">
          <span class="form__label">Điều <span class="required">*</span></span>
          <div class="category--current" @click.stop="toggleShowSelectArticle" :class="{ 'disabled': props.state < 5 }">
            <span v-if="currentArticle">
              {{ currentArticle!.Content }}
            </span>
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
                class="data"
              >
                <span > {{ article.Content }}</span>
                <font-awesome-icon
                  :icon="['fas', 'check']"
                  v-if="currentArticle?.Id === article.Id"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="file-category" v-if="props.block.Level >= 7" style="width: 100%;">
          <span class="form__label">Khoản <span class="required">*</span></span>
          <div class="category--current" @click.stop="toggleShowSelectClause" :class="{ 'disabled': props.state < 6 }">
            <span v-if="currentClause" >{{ currentClause!.Content }}</span>
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
                class="data"
              >
                <span> {{ clause.Content }}</span>
                <font-awesome-icon
                  :icon="['fas', 'check']"
                  v-if="currentClause?.Id === clause.Id"
                />
              </div>
              <div v-if="clauses.length === 0"> Không tìm thấy dữ liệu</div>
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
          <EditorContent :editor="editor" class="markdown-container"/>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm">Hủy</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm"  v-loading="loading"
            :class="{ 'disabled': !checkContentAvailable() }">
          <span src="/src/assets/icon/refresh.png" alt="logo">
            Xác nhận
          </span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { marked } from 'marked';
import {  onBeforeUnmount, onMounted, ref, type PropType } from 'vue';
import type { DocumentBlock } from '@/entities/Document';
import { removeHtmlTags } from '@/ts/markdown';


import { Editor, EditorContent } from '@tiptap/vue-3';
import Bold from '@tiptap/extension-bold';
import StarterKit from '@tiptap/starter-kit';
import Underline from '@tiptap/extension-underline';
import Table from '@tiptap/extension-table'
import TableRow from '@tiptap/extension-table-row'
import TableCell from '@tiptap/extension-table-cell'
import TableHeader from '@tiptap/extension-table-header'




marked.setOptions({
  breaks: true,
  gfm: true,
})

const props = defineProps({
  block: {
    type: Object as PropType<DocumentBlock>,
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

function toggleShowSelectArticle() {
  if (props.state < 5) {
    showSelectClause.value = false
    showSelectArticle.value = false
    return
  }
  showSelectClause.value = false
  showSelectArticle.value = !showSelectArticle.value
}

function toggleShowSelectClause() {
  if (props.state < 6) {
    showSelectClause.value = false
    showSelectArticle.value = false
    return
  }
  showSelectArticle.value = false
  showSelectClause.value = !showSelectClause.value
}

function selectArticle(article: DocumentBlock) {
  currentArticle.value = article
  clauses.value = props.blocksToSelect.filter((block) => block.Level === 6 && block.ParentId === article.Id);
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

function checkContentAvailable() {
  if (newContent.value === null || removeHtmlTags(newContent.value.trim()).length === 0) {
    return false
  }
  return true
}

function handleSubmitForm() {
  if (!checkContentAvailable()) {
    return
  }

  const block = props.block
  block.Content = newContent.value?.trim() || ''
  block.Title = removeHtmlTags(block.Content).trim()
  if (props.state === 5) {
    console.log('Them khoan')
    block.ParentId = currentArticle.value!.Id
  }
  if (props.state=== 6) {
    console.log('Them diem')
    block.ParentId = currentClause.value!.Id
  }
  emits('submitForm', block);
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
      Bold,
      Table.configure({
        resizable: true,
        HTMLAttributes: {
          style: 'width: 100%;',
        },
      }),
      TableRow.configure({
        HTMLAttributes: {
          style: 'width: 100%;',
        },
      }),
      TableHeader,
      TableCell.configure({
        HTMLAttributes: {
          style: 'text-align: center; width: 50%',
        },
      }),
    ],
    content: newContent.value,
    onUpdate: ({ editor }) => {
      newContent.value = editor.getHTML();
    },
  });


  getTitle()

  if (props.block.Level >= 6) {
    articles.value = props.blocksToSelect.filter((block) => block.Level === 5);
    clauses.value = props.blocksToSelect.filter((block) => block.Level === 6);

    const clause = props.blocksToSelect.find((block) => block.Level === 6 && block.Id === props.block.ParentId);
    if (clause) {
      currentClause.value = clause;
      currentArticle.value = props.blocksToSelect.find((block) => block.Level === 5 && block.Id === clause.ParentId) || null;
      clauses.value = props.blocksToSelect.filter((block) => block.Level === 6 && block.ParentId === currentArticle.value?.Id);
    }
    else {
      currentArticle.value = props.blocksToSelect.find((block) => block.Level === 5 && block.Id === props.block.ParentId) || null;
      clauses.value = props.blocksToSelect.filter((block) => block.Level === 6 && block.ParentId === currentArticle.value?.Id);
    }
  }

});


onBeforeUnmount(() => {
  if (editor.value) {
    editor.value.destroy();
  }
});

</script>
