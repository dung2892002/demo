<template>
  <div class="content">
    <div class="content__header">
      <div>
        <img src="/src/assets/icon/left.png" @click="closeFile" />
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
        <div>
          <select v-model="document!.CategoryId" class="form__input">
            <option :value="category.Id" v-for="category in categories" :key="category.Id">
              {{ category.Name }}
            </option>
          </select>
        </div>
      </div>
      <div class="file-data">
        <div v-html="fileContent"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, type PropType } from 'vue'
import axios from 'axios'
import { type DocumentCategory, type Document } from '@/entities/Document'
import { getSrcIconDocument } from '@/utils'

const fileContent = ref('')

const props = defineProps({
  document: {
    type: Object as PropType<Document>,
    required: true,
  },
})

const emits = defineEmits(['closeFile'])

function closeFile() {
  emits('closeFile')
}

async function fetchHtmlData() {
  try {
    const response = await axios.get(
      `https://localhost:7160/api/v1/Documents/content/${props.document.Id}`,
    )
    fileContent.value = response.data
    removeWatermark()
  } catch (error) {
    console.error('Lỗi khi lấy nội dung Markdown:', error)
  }
}

function removeWatermark() {
  fileContent.value = fileContent.value
    .replace(/Created with a trial version of Syncfusion Word library[^<]+/g, '')
    .replace(/<div[^>]*>Created with a trial version[^<]+<\/div>/g, '')
    .replace(/<a[^>]*syncfusion\.com\/account\/claim-license-key[^>]*>.*?<\/a>/g, '')
    .replace(/to obtain the valid key\./g, '')
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
    margin: 0 20px;
    display: flex;
    flex-direction: row;
    gap: 20px;
    background-color: gainsboro;

    div {
      border-radius: 10px;
      padding: 10px 20px;
    }

    .file {
      width: 25%;
      background-color: #fff;
      height: 760px;
    }

    .file-data {
      background-color: #fff;

      height: 720px;
      width: 100%;

      div {
        padding: 0 10px;
        overflow-y: scroll;
        &::-webkit-scrollbar {
          width: 8px;
          height: 40px;
        }

        &::-webkit-scrollbar-track {
          background: #f2f2f2;
        }

        &::-webkit-scrollbar-thumb {
          background-color: #b9b9b9;
        }

        &::-webkit-scrollbar-thumb:hover {
          background-color: #555;
        }
      }
    }
  }
}
</style>
