<template>
  <div class="content--file">
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
          <div class="form-data">
            <span>Chủ đề </span>
            <select v-model="document!.CategoryId" class="form__input" :disabled="!isEditMode">
              <option :value="category.Id" v-for="category in categories" :key="category.Id">
                {{ category.Name }}
              </option>
            </select>
          </div>
          <div v-if="document.IsLaw" class="form-data--law">
            <div class="form-data">
              <span>Cơ quan ban hành </span>
              <input v-model="document!.Issuer" class="form__input" :disabled="!isEditMode" />
            </div>
            <div class="form-data">
              <span>Mã văn bản </span>
              <input v-model="document!.DocumentNo" class="form__input" :disabled="!isEditMode" />
            </div>
            <div class="form-data">
              <span>Người ký</span>
              <input v-model="document!.SignerName" class="form__input" :disabled="!isEditMode" />
            </div>
            <div class="form-data">
              <span>Ngày ban hành</span>
              <input
                v-model="document!.IssueDate"
                class="form__input"
                :disabled="!isEditMode"
                type="date"
              />
            </div>
          </div>
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
import { formatDateForm, getSrcIconDocument } from '@/utils'
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
    fileContent.value = response.data
    console.log(compiledMarkdown)
  } catch (error) {
    console.error('Lỗi khi lấy nội dung Markdown:', error)
  }
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

function formatDate() {
  // eslint-disable-next-line vue/no-mutating-props
  props.document.IssueDate = formatDateForm(props.document.IssueDate!)
}

onMounted(() => {
  formatDate()
  fetchHtmlData()
  fetchCategories()
})
</script>

<style scoped lang="scss">
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

::v-deep(.markdown-container ul) {
  padding-left: 20px; /* Đảm bảo lùi vào đúng */
  list-style-position: inside; /* Di chuyển ::marker vào trong */
}

::v-deep(.markdown-container ol) {
  margin-left: 20px;
  // list-style-position: inside;
}
</style>
