<template>
  <div class="file-upload-form">
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
              <input type="text" />
            </div>
          </div>
          <div class="footer">
            <button class="button--cancel" @click="handleCloseForm(false)">Hủy</button>
            <button class="button--complete">Lưu</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import ToastComponent from '@/components/ToastComponent.vue'
import type { DocumentCategory } from '@/entities/Document'
import { ref, type PropType } from 'vue'
import SelectTypeUpload from './SelectTypeUpload.vue'
import SelectCategoryAndKnowledge from './SelectCategoryAndKnowledge.vue'

const typeUpload = ref(2)

const currentCategory = ref<DocumentCategory | null>(null)

const emits = defineEmits(['closeForm', 'submitForm', 'selectKnowledType', 'selectTypeUpload'])

// eslint-disable-next-line @typescript-eslint/no-unused-vars
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
</script>
