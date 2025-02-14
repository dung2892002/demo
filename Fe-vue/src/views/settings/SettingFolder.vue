<template>
  <div class="content">
    <div class="content-main">
      <div v-if="isShowMenu" class="menu-list">
        <div v-for="(menu, index) in menus" :key="index" @click="fetchFolderByMenu(menu)">
          <img src="/src/assets/icon/folder.png" alt="folder" />
          <span>{{ menu.MenuName }}</span>
        </div>
      </div>
      <div class="menu-list" v-else>
        <div class="button-group">
          <button @click="goback">
            <img src="/src/assets/icon/btn-prev-page.svg" alt="" />
          </button>

          <button @click="addFolder">
            <img src="/src/assets/icon/add-folder.png" alt="" />
          </button>

          <button @click="selectFile" v-if="backFolder.length > 0" v-loading="uploadFileLoading">
            <img src="/src/assets/icon/add-file.png" alt="" />
          </button>

          <input type="file" ref="fileInput" @change="handleFileUpload" style="display: none" />
        </div>
        <div
          v-for="(folder, index) in folders"
          :key="index"
          @click="selectFolder(folder)"
          @contextmenu.prevent="showContextMenu($event, folder.Id, index)"
          :class="{ selected: targetFolderId == folder.Id }"
        >
          <img src="/src/assets/icon/folder.png" alt="" />
          <span>{{ folder.FolderName }}</span>
        </div>
        <div
          v-for="(file, index) in files"
          :key="index"
          @click="selectFileToView(file)"
          @contextmenu.prevent="showContextFileMenu($event, file.FileId, index)"
          :class="{ selected: targetFileId == file.FileId }"
        >
          <img :src="getIconUrl(file.FileName)" alt="" />
          <span>{{ file.FileName }}</span>
        </div>
        <span v-if="folders?.length === 0 && files?.length === 0" class="folder--empty">Rỗng</span>
      </div>

      <div v-if="showForm" class="form-container">
        <div class="form__content">
          <div class="form__header">
            <h2 class="form__title">Thông tin folder</h2>
            <button class="form__button" @click="handleCloseForm">
              <img src="/src/assets/icon/close-48.png" alt="logo" />
            </button>
          </div>
          <form class="hospital-form">
            <div class="form-group">
              <div class="form__item">
                <span>Tên thư mục</span>
                <input type="text" v-model="folderSubmit.FolderName" placeholder="name" />
              </div>
            </div>
            <div class="form-group">
              <div class="form__item">
                <span>Đường dẫn</span>
                <input
                  type="text"
                  v-model="folderSubmit.FolderPath"
                  placeholder="path"
                  :disabled="folderUpdateId != null"
                />
              </div>
            </div>
          </form>
          <div class="form__footer">
            <button class="button--cancel" @click="handleCloseForm">Hủy</button>
            <button class="button--complete" @click="submitForm">Lưu</button>
          </div>
        </div>
      </div>
    </div>
    <ContextMenu
      v-if="showMenu"
      :actions="contextMenuActions"
      @actionClick="handleActionClick"
      @close="closeContextMenu"
      :position="menuPosition"
    ></ContextMenu>

    <ContextMenu
      v-if="showFileMenu"
      :actions="contextMenuFileActions"
      @actionClick="handleActionFileClick"
      @close="closeContextMenu"
      :position="menuFilePosition"
    ></ContextMenu>

    <div v-show="displayOnWeb === true" class="file--view">
      <button @click="closeFile">
        <img src="/src/assets/icon/close-48.png" alt="" />
      </button>
      <iframe v-if="fileUrl" :src="fileUrl"></iframe>
    </div>
  </div>
</template>

<script setup lang="ts">
import ContextMenu from '@/components/ContextMenu.vue'
import type { ActionMenu } from '@/entities/ActionMenu'
import type { UserFile } from '@/entities/File'
import type { Folder } from '@/entities/Folder'
import type { Menu } from '@/entities/Menu'
import { getFileFormat, getIconUrl } from '@/utils'
import axios from 'axios'
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const folders = ref<Folder[] | null>(null)
const backFolder = ref<Folder[]>([])
const isShowMenu = ref(true)
const selectMenu = ref<Menu | null>(null)

const files = ref<UserFile[] | null>([])

const showForm = ref(false)

const targetFolderId = ref<string>('')
const targetIndex = ref(-1)

const targetFileIndex = ref(-1)
const targetFileId = ref<string>('')

const showMenu = ref(false)
const showFileMenu = ref(false)

const fileInput = ref<HTMLInputElement | null>(null)

const uploadFileLoading = ref(false)

const fileUrl = ref<string | null>(null)

const displayOnWeb = ref(false)

function checkDisplayOnWeb(name: string): void {
  const formatFile = getFileFormat(name)
  if (formatFile === 'excel' || formatFile === 'word') displayOnWeb.value = false
  else displayOnWeb.value = true
}

function selectFile() {
  fileInput.value?.click()
}

const handleFileUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement
  if (!target.files || target.files.length === 0) return

  const file = target.files[0]
  const formData = new FormData()
  formData.append('folderId', backFolder.value[backFolder.value.length - 1].Id ?? '')
  formData.append('files', file)

  try {
    uploadFileLoading.value = true
    const response = await axios.post('https://localhost:7160/api/v1/Files', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    uploadFileLoading.value = false
    files.value?.push(response.data)
  } catch (error) {
    console.error('Upload failed:', error)

    uploadFileLoading.value = false
  }
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

const folderUpdateId = ref<string | null>(null)
const menuPosition = ref({
  top: 0,
  left: 0,
})

const menuFilePosition = ref({
  top: 0,
  left: 0,
})

function showContextFileMenu(event: MouseEvent, fileId: string | null, index: number) {
  menuFilePosition.value.top = event.clientY
  menuFilePosition.value.left = event.clientX

  showMenu.value = false
  showFileMenu.value = true

  targetFolderId.value = ''
  targetIndex.value = -1

  if (fileId) targetFileId.value = fileId
  targetFileIndex.value = index
}
function showContextMenu(event: MouseEvent, folderId: string | null, index: number) {
  menuPosition.value.top = event.clientY
  menuPosition.value.left = event.clientX

  showMenu.value = true
  showFileMenu.value = false

  targetFileId.value = ''
  targetFileIndex.value = -1

  if (folderId) targetFolderId.value = folderId
  targetIndex.value = index
}

function closeContextMenu() {
  showMenu.value = false
  showFileMenu.value = false
}

const contextMenuActions = ref<ActionMenu[]>([
  { label: 'Xoá', action: 'delete' },
  { label: 'Sửa', action: 'update' },
])

const contextMenuFileActions = ref<ActionMenu[]>([{ label: 'Xoá', action: 'delete' }])

function handleActionClick(action: ActionMenu) {
  if (action.action === 'update') {
    updateFolder(targetFolderId.value)
  } else {
    deleteFolder(targetFolderId.value, targetIndex.value)
    closeContextMenu()
  }
}

function handleActionFileClick(action: ActionMenu) {
  if (action.action === 'delete') {
    deleteFile(targetFileId.value, targetFileIndex.value)
  }
}

async function updateFolder(id: string) {
  const response = await axios.get(`https://localhost:7160/api/v1/Folders/${id}`)
  folderSubmit.value = response.data

  showForm.value = true
  showMenu.value = false
  folderUpdateId.value = id
}

async function deleteFile(id: string, index: number) {
  const response = await axios.delete('https://localhost:7160/api/v1/Files', {
    params: {
      id: id,
    },
  })
  closeContextMenu()
  if (response.status === 200) files.value?.splice(index, 1)
  else console.log('error')
}

async function deleteFolder(id: string, index: number) {
  const response = await axios.delete('https://localhost:7160/api/v1/Folders', {
    params: {
      id: id,
    },
  })
  if (response.status === 200) folders.value?.splice(index, 1)
  else console.log('error')
}

const folderSubmit = ref<Folder>({
  Id: null,
  FolderName: '',
  FolderPath: '',
  MenuId: null,
  ParentId: null,
})

async function goback() {
  const len = backFolder.value.length
  if (len > 0) {
    if (backFolder.value[len - 1].ParentId) {
      const response = await axios.get('https://localhost:7160/api/v1/Folders/subs', {
        params: {
          parentId: backFolder.value[len - 1].ParentId,
        },
      })
      folders.value = response.data
      await fetchFiles(backFolder.value[len - 1].ParentId ?? '')
    } else {
      if (selectMenu.value) {
        fetchFolderByMenu(selectMenu.value)
        files.value = []
      }
    }
    backFolder.value.pop()
  } else {
    fetchMenus()
  }
}

function closeFile() {
  fileUrl.value = null
  displayOnWeb.value = false
}

async function selectFileToView(file: UserFile) {
  try {
    const response = await axios.get(`https://localhost:7160/api/v1/Files/url/${file.FileId}`)
    fileUrl.value = response.data
    checkDisplayOnWeb(file.FileName)
  } catch (error) {
    console.error('Lỗi khi xem file:', error)
  }
}

async function selectFolder(folder: Folder) {
  const response = await axios.get('https://localhost:7160/api/v1/Folders/subs', {
    params: {
      parentId: folder.Id,
    },
  })

  targetFolderId.value = folder.Id ?? ''
  backFolder.value.push(folder)
  folders.value = response.data

  targetFileId.value = ''
  targetFileIndex.value = -1

  await fetchFiles(folder.Id ?? '')
}

async function fetchFolderByMenu(menu: Menu) {
  const response = await axios.get('https://localhost:7160/api/v1/Folders', {
    params: {
      menuId: menu.Id,
    },
  })
  isShowMenu.value = false
  selectMenu.value = menu
  folders.value = response.data
}

async function fetchMenus() {
  isShowMenu.value = true
  selectMenu.value = null
  await store.dispatch('fetchMenus')
}

function addFolder() {
  showForm.value = true
  folderUpdateId.value = null
}

async function submitForm() {
  if (folderUpdateId.value === null) {
    if (backFolder.value.length === 0) {
      folderSubmit.value.MenuId = selectMenu.value?.Id ?? null
      folderSubmit.value.FolderPath =
        selectMenu.value?.MenuPath + '/' + folderSubmit.value.FolderPath
    } else {
      folderSubmit.value.ParentId = backFolder.value[backFolder.value.length - 1].Id
      folderSubmit.value.FolderPath =
        backFolder.value[backFolder.value.length - 1].FolderPath +
        '/' +
        folderSubmit.value.FolderPath
    }

    try {
      const response = await axios.post('https://localhost:7160/api/v1/Folders', folderSubmit.value)

      if (response.status === 201) {
        folders.value?.push(response.data)
      }
    } catch (error) {
      console.error('Lỗi khi cập nhật folder:', error)
    }
  } else {
    try {
      const response = await axios.put('https://localhost:7160/api/v1/Folders', folderSubmit.value)
      if (response.status === 200 && folders.value && targetIndex.value !== -1) {
        folders.value[targetIndex.value] = folderSubmit.value
      }
    } catch (error) {
      console.error('Lỗi khi cập nhật folder:', error)
    }
  }

  showForm.value = false
  folderUpdateId.value = null
  folderSubmit.value = {
    Id: null,
    FolderName: '',
    FolderPath: '',
    MenuId: null,
    ParentId: null,
  }
}

function handleCloseForm() {
  showForm.value = false
  showMenu.value = false
  folderUpdateId.value = null
  folderSubmit.value = {
    Id: null,
    FolderName: '',
    FolderPath: '',
    MenuId: null,
    ParentId: null,
  }
}

const menus = computed(() => store.getters.getMenus)

onMounted(() => {
  fetchMenus()
})
</script>
