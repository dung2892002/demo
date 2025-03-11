<template>
  <div class="tree-node">
    <div
      :class="['folder-item', { selected: document.Id === parentId }]"
      @click="handleClickDocument"
    >
      <div style="display: flex; flex-direction: row; align-items: center">
        <span
          v-if="document.Type === DocumentType.Folder"
          @click.stop="toggleExpand"
          v-loading="loading"
          :class="{
            disabled: !checkAvailableSelectFolder(),
          }"
        >
          <img
            src="/src/assets/icon/substraction.png"
            style="width: 16px; height: 16px"
            v-if="document.IsExpend"
          />
          <img src="/src/assets/icon/add-xx.png" style="width: 16px; height: 16px" v-else />
        </span>
        <span
          style="display: flex; align-items: center; gap: 10px; padding: 0 6px"
          :class="{
            disabled: !checkAvailableSelectFolder(),
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

    <div v-if="document.IsExpend" class="children">
      <template v-if="!loading">
        <DocumentNode
          v-for="child in document.Children"
          :key="child.Id!"
          :document="child"
          :selectedDocuments="selectedDocuments"
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
const store = useStore()
const props = defineProps({
  document: {
    type: Object as PropType<Document>,
    required: true,
  },
  selectedDocuments: {
    type: Array as PropType<Document[]>,
    required: true,
  },
  parentId: {
    type: String as PropType<string | null>,
  },
})

function checkAvailableSelectFolder() {
  return !(
    props.selectedDocuments.some((doc) => doc.Id === props.document.Id) ||
    props.selectedDocuments.some((doc) => doc.ParentId === props.document.Id) ||
    (props.selectedDocuments.some((doc) => doc.ParentId === null) && props.document.Id === 'null')
  )
}

function checkDocumentIsParent() {
  return listParent.value.some((folder) => folder.Id === props.document.Id)
}

const loading = ref(false)

function handleClickDocument() {
  if (!checkAvailableSelectFolder()) return
  emit('select', props.document.Id)
}

const emit = defineEmits(['select'])

async function toggleExpand() {
  if (props.document.Type !== 'Folder') return
  if (!checkAvailableSelectFolder()) return
  if (!props.document.IsExpend) {
    props.document.IsExpend! = true
    if (!props.document.IsLoaded) {
      loading.value = true
      await fetchChildren()
      loading.value = false
    }
  } else {
    props.document.IsExpend! = false
  }
}

async function fetchChildren() {
  props.document.Children! = props.document.Children || []
  const response = await axios.get('https://localhost:7160/api/v1/Documents/subs', {
    params: { id: props.document.Id },
  })
  props.document.IsExpend! = true
  props.document.IsLoaded! = true
  props.document.Children! = response.data.map((doc) => ({
    ...doc,
    isExpand: false,
    isLoaded: false,
  }))
}

const listParent = computed(() => store.getters.getListFolder)

onMounted(() => {
  if (checkDocumentIsParent() && !props.document.IsLoaded) {
    fetchChildren()
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
