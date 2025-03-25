<template>
  <div class="file-upload-form" v-if="docReviewId == null">
    <div class="form">
      <SelectTypeUpload :type-upload="typeUpload" @select-type-upload="selectTypeUpload" />
      <div class="form-data">
        <ToastComponent ref="toastRef" />
        <div>
          <SelectCategoryAndKnowledge
            :categories="categories"
            :is-law="false"
            @select-category="selectCategory"
            @select-knowled-type="selectLaw"
          />
          <div class="form-group">
            <div class="form__item">
              <span class="form__label">Liên kết <span class="required">*</span></span>
              <input type="text" v-model="link" />
            </div>
          </div>
          <div class="footer">
            <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
            <button class="button--complete" @click="submitForm" v-loading="loading">Lưu</button>
          </div>
        </div>
      </div>
    </div>
  </div>
  <ContentReview :document-id="docReviewId!" @close="handleCloseForm(true)" v-else />
</template>

<script setup lang="ts">
import ToastComponent from '@/components/ToastComponent.vue'
import type { DocumentCategory } from '@/entities/Document'
import { ref, type PropType } from 'vue'
import SelectTypeUpload from './SelectTypeUpload.vue'
import SelectCategoryAndKnowledge from './SelectCategoryAndKnowledge.vue'
import axios from 'axios'
import ContentReview from './ContentReview.vue'

const typeUpload = ref(2)
const link = ref<string | null>(null)
const loading = ref(false)
const currentCategory = ref<DocumentCategory | null>(null)

const emits = defineEmits(['closeForm', 'selectKnowledType', 'selectTypeUpload'])

const props = defineProps({
  categories: {
    type: Array as PropType<DocumentCategory[]>,
    required: true,
  },
  parentId: {
    type: String,
    required: true,
  },
})

function selectTypeUpload(type: number) {
  typeUpload.value = type
  emits('selectTypeUpload', type)
}

function selectCategory(category: DocumentCategory) {
  currentCategory.value = category
}

function selectLaw(state: boolean) {
  emits('selectKnowledType', state)
}

function handleCloseForm(value: boolean) {
  emits('closeForm', value)
}

export interface AddLinkRequest {
  Link: string
  ParentId: string | null
  CategoryId: string
}

const docReviewId = ref<string | null>(null)
async function submitForm() {
  if (!checkAvailable()) return
  const request: AddLinkRequest = {
    Link: link.value!,
    CategoryId: currentCategory.value!.Id,
    ParentId: props.parentId ? props.parentId : null,
  }

  try {
    loading.value = true
    const response = await axios.post('https://localhost:7160/api/v1/Documents/link', request)
    loading.value = false
    docReviewId.value = response.data
  } catch (error) {
    console.log(error)
  }
}

const toastRef = ref<InstanceType<typeof ToastComponent> | null>(null)
function checkAvailable() {
  let check = true

  if (!currentCategory.value) {
    toastRef.value?.addToastError('Chưa chọn chủ đề cho tài liệu')
    check = false
  }

  if (!link.value) {
    toastRef.value?.addToastError('Link không được để trống')
    check = false
  }

  const regex = /^(https?:\/\/)?([\w-]+\.)+[\w-]{2,}(\/\S*)?$/
  if (!regex.test(link.value!)) {
    toastRef.value?.addToastError('Link không đúng định dạng')
    check = false
  }
  return check
}
</script>
