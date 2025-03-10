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
            disabled:
              document.Id === selectedDocuments[0]?.Id ||
              (selectedDocuments[0]?.ParentId == null && document.Id == 'null') ||
              document.Id === selectedDocuments[0]?.ParentId,
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
            disabled:
              document.Id === selectedDocuments[0]?.ParentId ||
              document.Id === selectedDocuments[0]?.Id ||
              (selectedDocuments[0]?.ParentId == null && document.Id == 'null'),
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
import { ref, type PropType, onMounted } from 'vue'
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

function checkDocument() {
  const folderPath = props.selectedDocuments[0].FolderPath.split('/')
  const currentPath = [...props.document.FolderPath.split('/'), props.document.Name]

  return folderPath.slice(0, currentPath.length).join('/') === currentPath.join('/')
}

const loading = ref(false)

function handleClickDocument() {
  if (props.document.Id == props.selectedDocuments[0]?.Id) return
  if (props.document.Id == props.selectedDocuments[0]?.ParentId) return
  if (props.document.Id == 'null' && props.selectedDocuments[0]?.ParentId == null) return
  emit('select', props.document.Id)
}

const emit = defineEmits(['select'])

async function toggleExpand() {
  if (props.document.Type !== 'Folder') return

  if (
    !props.document.IsExpend &&
    props.document.Id != props.selectedDocuments[0]?.Id &&
    props.document.Id != props.selectedDocuments[0]?.ParentId
  ) {
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

onMounted(() => {
  if (checkDocument() && !props.document.IsLoaded) {
    console.log('open folder:', props.document.Name)
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
