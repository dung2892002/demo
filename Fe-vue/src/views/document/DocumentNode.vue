<template>
  <div class="tree-node">
    <div
      :class="[
        'folder-item',
        { selected: document.Id === parentId },
        {
          disabled:
            document.Id === selectedDocument?.Id ||
            (selectedDocument?.ParentId == null && document.Id == 'null'),
        },
      ]"
      @click="handleClickDocument"
    >
      <div style="display: flex; flex-direction: row; align-items: center; gap: 6px">
        <span
          v-if="document.Type === DocumentType.Folder"
          @click.stop="toggleExpand"
          v-loading="loading"
        >
          <img
            src="/src/assets/icon/substraction.png"
            style="width: 16px; height: 16px"
            v-if="isExpanded"
          />
          <img src="/src/assets/icon/add-xx.png" style="width: 16px; height: 16px" v-else />
        </span>
        <span
          style="display: flex; align-items: center; gap: 10px"
          :class="{
            disabled:
              document.Id === selectedDocument?.ParentId || document.Id === selectedDocument?.Id,
          }"
        >
          <img src="/src/assets/icon/folder.png" style="width: 24px; height: 24px" />
          {{ document.Name }}
        </span>
      </div>
      <img
        src="/src/assets/icon/yes.png"
        style="width: 16px; height: 16px"
        v-if="document.Id === parentId"
      />
    </div>

    <div v-if="isExpanded" class="children">
      <template v-if="!loading">
        <DocumentNode
          v-for="child in document.Children"
          :key="child.Id!"
          :document="child"
          :selectedDocument="selectedDocument"
          :parentId="parentId"
          @select="$emit('select', $event)"
        />
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { DocumentType, type Document } from '@/entities/Document'
import axios from 'axios'
import { ref, type PropType, onMounted, computed } from 'vue'
import { useStore } from 'vuex'

const props = defineProps({
  document: {
    type: Object as PropType<Document>,
    required: true,
  },
  selectedDocument: {
    type: Object as PropType<Document>,
  },
  parentId: {
    type: String as PropType<string | null>,
  },
})

const store = useStore()
const listDocuments = computed(() => store.getters.getListDocuments)

function checkDocument() {
  return listDocuments.value.some((doc) => doc.Id === props.document.Id)
}

const loading = ref(false)

function handleClickDocument() {
  if (props.document.Id == props.selectedDocument?.Id) return
  if (props.document.Id == props.selectedDocument?.ParentId) return
  if (props.document.Id == 'null' && props.selectedDocument?.ParentId == null) return
  emit('select', props.document.Id)
}

const emit = defineEmits(['select'])

const isExpanded = ref(false)

async function toggleExpand() {
  if (props.document.Type !== 'Folder') return

  if (!isExpanded.value && props.document.Id != props.selectedDocument?.Id) {
    isExpanded.value = true
    if (!props.document.Children?.length) {
      loading.value = true
      await fetchChildren()
      loading.value = false
    }
  } else {
    isExpanded.value = false
  }
}

async function fetchChildren() {
  const response = await axios.get('https://localhost:7160/api/v1/Documents/subs', {
    params: { id: props.document.Id },
  })

  props.document.Children! = response.data
}

onMounted(() => {
  if (checkDocument()) {
    fetchChildren()
    isExpanded.value = true
  }
})
</script>

<style scoped>
.folder-item {
  padding: 5px 10px;
  cursor: pointer;
  border-radius: 4px;
  display: flex;
  flex-direction: row;
  justify-content: space-between;
}
.folder-item.selected {
  background: #e0f2ff;
}
.children {
  margin-left: 20px;
}
.loading {
  font-size: 12px;
  color: gray;
}

.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
