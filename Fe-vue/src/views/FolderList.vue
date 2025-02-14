<template>
  <div class="content__filter">
    <div>
      <div @click="goback" v-if="backMenu">
        <img src="/src/assets/icon/btn-prev-page.svg" alt="" />
      </div>
      <div v-for="(folder, index) in folders" :key="index" @click="selectFolder(folder)">
        <img src="/src/assets/icon/folder.png" alt="" />
        <span>{{ folder.FolderName }}</span>
      </div>
    </div>
    <div>
      <div
        v-for="(file, index) in files"
        :key="index"
        :class="{ selected: fileSelectId === file.FileId }"
        @click="selectFile(file)"
        v-loading="props.loadingFileId === file.FileId"
      >
        <img :src="getIconUrl(file.FileName)" alt="" />
        <span>{{ file.FileName }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { UserFile } from '@/entities/File'
import type { Folder } from '@/entities/Folder'
import { getIconUrl } from '@/utils'
import axios from 'axios'
import { onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const folders = ref<Folder[] | null>(null)
const files = ref<UserFile[] | null>(null)

const fileSelectId = ref<string | null>(null)

const backFolderId = ref<string[]>([])
const backMenu = ref(false)

const emits = defineEmits(['selectFile'])

const props = defineProps({
  loadingFileId: {
    type: String,
  },
})

async function goback() {
  if (backFolderId.value.length > 0) {
    const response = await axios.get('https://localhost:7160/api/v1/Folders/subs', {
      params: {
        parentId: backFolderId.value[backFolderId.value.length - 1],
      },
    })

    await fetchFiles(backFolderId.value[backFolderId.value.length - 1])
    backFolderId.value.pop()
    folders.value = response.data
  } else {
    backMenu.value = false
    files.value = null
    fetchFolderByMenu()
  }
}

async function selectFolder(folder: Folder) {
  const response = await axios.get('https://localhost:7160/api/v1/Folders/subs', {
    params: {
      parentId: folder.Id,
    },
  })
  fileSelectId.value = null
  await fetchFiles(folder.Id ?? '')
  if (folder.ParentId) backFolderId.value.push(folder.ParentId)
  backMenu.value = true
  folders.value = response.data
}

async function fetchFolderByMenu() {
  const menuId = store.getters.getCurrentMenuId

  const response = await axios.get('https://localhost:7160/api/v1/Folders', {
    params: {
      menuId: menuId,
    },
  })

  folders.value = response.data
}

async function fetchFiles(folderId: string) {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Files', {
      params: {
        folderId: folderId,
      },
    })

    files.value = response.data
  } catch (error) {
    console.error('Lỗi khi cập nhật folder:', error)
  }
}

function selectFile(file: UserFile) {
  fileSelectId.value = file.FileId
  emits('selectFile', file)
}

onMounted(() => {
  fetchFolderByMenu()
})
</script>
