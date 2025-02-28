<template>
  <div class="form-container">
    <div class="form__content" v-draggable style="padding-left: 10px">
      <div class="form__header">
        <h2 class="form__title">Di chuyển tài liệu</h2>
        <button class="form__button" @click="handleCloseForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <div style="margin-top: 10px">
        <div
          style="
            display: flex;
            flex-direction: row;
            gap: 72px;
            align-items: center;
            margin-bottom: 10px;
          "
        >
          <div style="padding: 12px; width: 120px">Vị trí hiện tại</div>
          <div
            style="
              padding: 6px 20px;
              border: 1px solid gainsboro;
              border-radius: 6px;
              min-width: 280px;
              height: 36px;
              box-sizing: border-box;
              display: flex;
              flex-direction: row;
              align-items: center;
              gap: 10px;
              background-color: gainsboro;
            "
          >
            <img src="/src/assets/icon/folder.png" style="width: 24px; height: 24px" />
            <div>
              {{ getNameParent() }}
            </div>
          </div>
        </div>
        <form class="cukcuk-form" id="form" style="display: flex; flex-direction: row; gap: 60px">
          <div style="width: 120px">Di chuyển tới</div>
          <div
            style="padding: 10px; border: 1px solid gainsboro; border-radius: 6px; min-width: 280px"
          >
            <div class="tree-view">
              <DocumentNode
                :document="rootDocument"
                :selectedDocument="props.document"
                :parentId="selectedDocumentId"
                @select="handleSelectRootFolder"
              />
              <DocumentNode
                v-for="document in documents"
                :key="document.Id!"
                :document="document"
                :selectedDocument="props.document"
                :parentId="selectedDocumentId"
                @select="handleSelectDocument"
              />
            </div>
          </div>
        </form>
      </div>
      <div class="form__footer" style="margin-top: 10px">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
        <button class="button--complete" id="submitButton" @click="handleSubmitForm">
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { DocumentType, type Document } from '@/entities/Document'
import axios from 'axios'
import { computed, onMounted, ref, type PropType } from 'vue'
import DocumentNode from './DocumentNode.vue'
import { useStore } from 'vuex'

const emits = defineEmits(['closeForm'])
const store = useStore()

const listDocuments = computed(() => store.getters.getListDocuments)

function getNameParent(): string {
  const len = listDocuments.value.length
  if (len == 0) return 'Tài liệu'

  return listDocuments.value[len - 1].Name
}

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const props = defineProps({
  document: {
    type: Object as PropType<Document>,
    required: true,
  },
})

const selectedDocumentId = ref<string | null>(null)

function handleSelectDocument(id: string) {
  selectedDocumentId.value = id
}

function handleSelectRootFolder() {
  selectedDocumentId.value = 'null'
}

async function handleSubmitForm() {
  const formData = new FormData()
  if (selectedDocumentId.value != null && selectedDocumentId.value != 'null')
    formData.append('parentId', selectedDocumentId.value)

  try {
    await axios.patch(
      `https://localhost:7160/api/v1/Documents/move/${props.document.Id}`,
      formData,
      {
        headers: { 'Content-Type': 'multipart/form-data' },
      },
    )

    handleCloseForm(true)
  } catch (error) {
    console.error('Lỗi tải tệp:', error)
  }
}

const rootDocument = ref<Document>({
  Id: 'null',
  Name: 'Tài liệu',
  Type: DocumentType.Unknown,
  CategoryId: null,
  CreatedAt: '',
  DocumentPath: '',
})

const documents = ref<Document[]>([])

async function fetchDocuments(parentId: string | null) {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Documents/subs', {
      params: { id: parentId },
    })

    documents.value = response.data.map((doc: Document) => ({
      ...doc,
      Children: doc.Children ?? [],
    }))
  } catch (error) {
    console.error(error)
  }
}
onMounted(() => {
  fetchDocuments(null)
})
</script>
