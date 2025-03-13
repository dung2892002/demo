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
        <div class="current-position">
          <div style="padding: 12px; width: 120px">Vị trí hiện tại</div>
          <div class="current-position--value">
            <img src="/src/assets/icon/folder.png" style="width: 24px; height: 24px" />
            <div>
              {{ parentFolderName }}
            </div>
          </div>
        </div>
        <form class="cukcuk-form" id="form" style="display: flex; flex-direction: row; gap: 60px">
          <div style="width: 120px">Di chuyển tới</div>
          <div class="nodes">
            <div class="tree-view">
              <DocumentNode
                :document="rootDocument"
                :selectedDocuments="props.documents"
                :parentId="selectedDocumentId"
                @select="handleSelectDocument"
              />
            </div>
          </div>
        </form>
      </div>
      <div class="form__footer" style="margin-top: 10px">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
        <button class="button--complete" id="submitButton" @click="showConfirmForm">
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>

  <ConfirmMove
    v-if="showConfirm"
    :toDocumentId="selectedDocumentId!"
    @close="closeConfirm"
    @submit="handleSubmitForm"
  />
</template>

<script setup lang="ts">
import { DocumentType, type Document } from '@/entities/Document'
import axios from 'axios'
import { onMounted, ref, type PropType } from 'vue'
import DocumentNode from './DocumentNode.vue'
import ConfirmMove from './ConfirmMove.vue'

const emits = defineEmits(['closeForm'])
const parentFolderName = ref<string>('Tài liệu')

const showConfirm = ref(false)

function closeConfirm() {
  showConfirm.value = false
}

function showConfirmForm() {
  if (selectedDocumentId.value) {
    showConfirm.value = true
  }
}

async function getNameParent() {
  if (props.documents.length > 1) {
    parentFolderName.value = ''
    return
  }
  if (props.documents[0].ParentId == null) return

  try {
    const response = await axios.get(
      `https://localhost:7160/api/v1/Documents/${props.documents[0].ParentId}`,
    )

    const parentDocument: Document = response.data

    parentFolderName.value = parentDocument.Name
  } catch (error) {
    console.log(error)
  }
}

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

const props = defineProps({
  documents: {
    type: Array as PropType<Document[]>,
    required: true,
  },
})

const selectedDocumentId = ref<string | null>(null)

function handleSelectDocument(id: string) {
  selectedDocumentId.value = id
}

async function handleSubmitForm() {
  const ids = props.documents.map((doc) => doc.Id)

  try {
    await axios.put(`https://localhost:7160/api/v1/Documents/move`, ids, {
      params: selectedDocumentId.value !== 'null' ? { parentId: selectedDocumentId.value } : {},
    })

    handleCloseForm(true)
  } catch (error) {
    console.error('Lỗi tải tệp:', error)
  }
}

const rootDocument = ref<Document>({
  Id: 'null',
  Name: 'Tài liệu',
  Type: DocumentType.Folder,
  CategoryId: null,
  CreatedAt: '',
  FolderPath: '',
  IsExpend: true,
  IsLoaded: true,
  DocumentBlocks: [],
  DocumentNo: null,
  IssueDate: null,
  SignerName: null,
  Issuer: null,
  IsLaw: false,
})

async function fetchDocuments(parentId: string | null) {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Documents/subs', {
      params: { id: parentId },
    })

    rootDocument.value.Children = response.data.map((doc: Document) => ({
      ...doc,
      Children: doc.Children ?? [],
    }))
  } catch (error) {
    console.error(error)
  }
}
onMounted(() => {
  fetchDocuments(null)
  getNameParent()
})
</script>

<style lang="scss" scoped>
.nodes {
  padding: 10px;
  border: 1px solid gainsboro;
  border-radius: 6px;
  min-width: 280px;
  max-height: 480px;
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

.current-position {
  display: flex;
  flex-direction: row;
  gap: 72px;
  align-items: center;
  margin-bottom: 10px;

  .current-position--value {
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
  }
}
</style>
