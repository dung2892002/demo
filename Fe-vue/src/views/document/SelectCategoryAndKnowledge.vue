<template>
  <div
    style="
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      align-items: center;
      gap: 10px;
    "
  >
    <div class="data-select">
      <span class="form__label">Chủ đề <span class="required">*</span></span>
      <div class="value--current" @click.stop="toggleShowSelectCategory">
        <span>{{ currentCategory ? currentCategory.Name : 'Chọn chủ đề' }}</span>
        <div>
          <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectCategory" />
          <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
        </div>
        <div class="value-data more" v-if="showSelectCategory">
          <div
            v-for="category in categories"
            :key="category.Id"
            @click.stop="selectCategory(category)"
            :class="{ selected: currentCategory?.Id === category.Id }"
            class="data"
          >
            <span> {{ category.Name }}</span>
            <font-awesome-icon
              :icon="['fas', 'check']"
              v-if="currentCategory?.Id === category.Id"
            />
          </div>
        </div>
      </div>
    </div>
    <div class="data-select">
      <span class="form__label">Loại tri thức <span class="required">*</span></span>
      <div class="value--current" @click.stop="toggleShowSelectKnowledgeType">
        <span>{{ isLaw ? 'Văn bản quy phạm pháp luật' : 'Tri thức nghiệp vụ khác' }}</span>
        <div>
          <font-awesome-icon :icon="['fas', 'chevron-up']" v-if="showSelectKnowledgeType" />
          <font-awesome-icon :icon="['fas', 'chevron-down']" v-else />
        </div>
        <div class="value-data" v-if="showSelectKnowledgeType">
          <div @click.stop="selectLaw(true)" :class="{ selected: isLaw }" class="data">
            <span> Văn bản quy phạm pháp luật</span>
            <font-awesome-icon :icon="['fas', 'check']" v-if="isLaw" />
          </div>
          <div @click.stop="selectLaw(false)" :class="{ selected: !isLaw }" class="data">
            <span> Tri thức nghiệp vụ khác</span>
            <font-awesome-icon :icon="['fas', 'check']" v-if="!isLaw" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentCategory } from '@/entities/Document'
import { onBeforeUnmount, onMounted, ref, type PropType } from 'vue'
import '../../styles/component/select.scss'

const currentCategory = ref<DocumentCategory | null>(null)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  categories: {
    type: Array as PropType<DocumentCategory[]>,
    required: true,
  },
  isLaw: {
    type: Boolean,
    required: true,
  },
})

const emits = defineEmits(['selectKnowledType', 'selectCategory'])

const showSelectCategory = ref(false)
const showSelectKnowledgeType = ref(false)

function toggleShowSelectCategory() {
  showSelectKnowledgeType.value = false
  showSelectCategory.value = !showSelectCategory.value
}

function toggleShowSelectKnowledgeType() {
  showSelectCategory.value = false
  showSelectKnowledgeType.value = !showSelectKnowledgeType.value
}

function selectCategory(category: DocumentCategory) {
  currentCategory.value = category
  emits('selectCategory', category)
  showSelectCategory.value = false
}

function selectLaw(state: boolean) {
  showSelectKnowledgeType.value = false
  emits('selectKnowledType', state)
}

onMounted(() => {
  document.addEventListener('click', () => {
    showSelectCategory.value = false
    showSelectKnowledgeType.value = false
  })
})

onBeforeUnmount(() => {
  document.removeEventListener('click', () => {
    showSelectCategory.value = false
    showSelectKnowledgeType.value = false
  })
})
</script>
