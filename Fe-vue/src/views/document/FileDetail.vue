<template>
  <div class="content">
    <div class="content__header">
      <div>
        <img src="/src/assets/icon/left.png" @click="closeFile(false)" />
        <span>Xem tài liệu</span>
      </div>
    </div>
    <div class="content-main">
      <div class="file">
        <div>
          <img
            :src="`/src/assets/icon/${getSrcIconDocument(document.Type)}`"
            alt="logo"
            style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
          />
          <span>{{ props.document.Name }}</span>
        </div>
        <div class="file-form">
          <span>Chủ đề *</span>
          <select v-model="document!.CategoryId" class="form__input" :disabled="!isEditMode">
            <option :value="category.Id" v-for="category in categories" :key="category.Id">
              {{ category.Name }}
            </option>
          </select>
        </div>
      </div>
      <div class="file-data">
        <div class="file-content">
          <div v-html="compiledMarkdown" class="markdown-container"></div>
        </div>
        <div class="footer">
          <div v-if="!isEditMode">
            <button @click="isEditMode = true">Sửa</button>
            <button @click="closeFile(true)">Đóng</button>
          </div>
          <div v-else>
            <button @click="closeFile(true)">Hủy</button>
            <button @click="handleUpdateFile">Xác nhận</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, type PropType, computed } from 'vue'
import axios from 'axios'
import { type DocumentCategory, type Document } from '@/entities/Document'
import { getSrcIconDocument } from '@/utils'
import { marked } from 'marked'

const fileContent = ref('')
const compiledMarkdown = computed(() => marked(fileContent.value))

const isEditMode = ref(false)

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

async function handleUpdateFile() {
  try {
    await axios.put(`https://localhost:7160/api/v1/Documents/${props.document.Id}`, props.document)

    closeFile(true)
  } catch (error) {
    console.error('Lỗi :', error)
  }
}

async function fetchHtmlData() {
  try {
    const response = await axios.get(
      `https://localhost:7160/api/v1/Documents/content/${props.document.Id}`,
    )
    fileContent.value = removeSyncfusionTrialNotes(response.data)
    console.log(compiledMarkdown)
  } catch (error) {
    console.error('Lỗi khi lấy nội dung Markdown:', error)
  }
}

function removeSyncfusionTrialNotes(text) {
  return text
    .replace(
      /\*\*Created with a trial version of Syncfusion Word library.*?obtain the valid key\.\*\*\n?/gs,
      '',
    )
    .trim()
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

onMounted(() => {
  fetchHtmlData()
  fetchCategories()
})
</script>

<style scoped lang="scss">
.content {
  position: fixed;
  top: 0;
  left: 0;
  z-index: 100;
  padding: 0;
  background-color: gainsboro;
  width: 100%;
  min-height: 100vh;
  .content__header {
    background-color: #fff;
    height: 56px;
    margin-bottom: 20px;
    padding: 0 20px;

    div {
      display: flex;
      flex-direction: row;
      align-items: center;

      img {
        width: 36px;
        height: 36px;
        margin-right: 8px;
        cursor: pointer;
      }

      span {
        font-size: 20px;
        font-weight: 600;
        color: #606266;
      }
    }
  }

  .content-main {
    display: flex;
    flex-direction: row;
    background-color: gainsboro;
    gap: 20px;

    div {
      border-radius: 10px;
      padding: 10px;
    }

    .file {
      width: 320px;
      background-color: #fff;
      height: 850px;

      .file-form {
        display: flex;
        flex-direction: column;
        justify-content: left;
      }
    }

    .file-data {
      width: 100%;
      display: flex;
      flex-direction: column;
      margin: 0;
      padding: 0;
      .file-content {
        background-color: #fff;
        height: 800px;
        padding: 10px;
        overflow-y: scroll;
        width: 100%;
        margin: 0, auto;

        &::-webkit-scrollbar {
          width: 8px;
          height: 40px;
        }

        &::-webkit-scrollbar-track {
          background: #f2f2f2;

          border-radius: 10px;
        }

        &::-webkit-scrollbar-thumb {
          background-color: #b9b9b9;
        }

        &::-webkit-scrollbar-thumb:hover {
          background-color: #555;
        }
      }

      .footer {
        margin: 0;
        padding: 0;
        display: flex;
        flex-direction: row;
        justify-content: right;

        div {
          display: flex;
          flex-direction: row;
          gap: 8px;

          button {
            padding: 8px 24px;
            border-radius: 6px;

            &:first-child {
              background-color: #ffffff;
              &:hover {
                background-color: #dddddd;
                border: 1px solid gray;
              }
            }

            &:last-child {
              background-color: #078cf8;
              color: #ffffff;
              &:hover {
                background-color: #9bd2ff;
              }
            }
          }
        }
      }
    }
  }
}

.markdown-container {
  margin: 0 auto;
  width: 100%;
}

::v-deep(.markdown-container strong) {
  font-weight: bold;
}

::v-deep(.markdown-container em) {
  font-style: italic;
}
</style>
