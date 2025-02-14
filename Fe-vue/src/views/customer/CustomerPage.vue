<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Quản lý khách hàng</h1>
      <button class="content__button button--add" @click="addNew" v-loading="addLoading">
        <div class="content__button button--add">
          <img src="/src/assets/icon/add.png" alt="logo" class="button--add-logo" />
          <span class="button--add-text">Thêm mới</span>
        </div>
      </button>
    </div>
    <FolderList @selectFile="handleSelectFile" :loadingFileId="loadingFileId" />
    <div class="content-main">
      <div class="toolbar">
        <div class="toolbar_search">
          <input
            type="text"
            id="search-employee"
            placeholder="Tìm kiếm theo mã, họ tên"
            v-model="keyword"
          />
          <button @click="handleSearch()" v-loading="searchLoading">
            <img src="/src/assets/icon/search.png" alt="logo" />
          </button>
        </div>
        <div class="toolbar__actions">
          <button class="toolbar-action" @click="importFile()">
            <img src="/src/assets/icon/import.png" alt="logo" />
          </button>

          <button class="toolbar-action" @click="handleExportFile()" v-loading="exportLoading">
            <img src="/src/assets/icon/export-excel-50.png" alt="logo" />
          </button>
          <button class="toolbar-action" @click="handleRefresh()" v-loading="refreshLoading">
            <img src="/src/assets/icon/refresh.png" alt="logo" />
          </button>
        </div>
      </div>
      <div class="main-container" ref="tableContainer">
        <table class="employee-table">
          <thead>
            <tr>
              <th class="w-6">STT</th>
              <th class="w-10">Mã KH</th>
              <th class="w-20">Họ và tên</th>
              <th class="w-10">Giới tính</th>
              <th class="w-12">Ngày sinh</th>
              <th class="w-30">Email</th>
              <th>Địa chỉ</th>
              <th class="w-14">Nhóm KH</th>
              <th class="w-10">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(customer, index) in customers"
              :key="index"
              @contextmenu.prevent="showContextMenu($event, customer.CustomerId, index)"
            >
              <td>{{ index + 1 }}</td>
              <td>{{ customer.CustomerCode }}</td>
              <td>{{ customer.Fullname }}</td>
              <td>{{ customer.GenderName }}</td>
              <td>{{ formatDate(customer.DateOfBirth) }}</td>
              <td>{{ customer.Email }}</td>
              <td>{{ customer.Address }}</td>
              <td>{{ customer.GroupName }}</td>
              <td>
                <div class="action" :ref="`action-${index}`">
                  <div class="action-buttons">
                    <button class="action-button" @click="togglePopupAction(index, $event)">
                      <img src="/src/assets/icon/option.png" alt="" />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span
                        @click="deleteCustomer(customer.CustomerId, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span @click="deleteCustomer(customer.CustomerId, index)">Xóa</span>
                      <span
                        @click="updateCustomer(customer.CustomerId, index)"
                        v-loading="updateLoading == index"
                        >Sửa
                      </span>
                      <span
                        @click="updateCustomer(customer.CustomerId, index)"
                        v-loading="updateLoading == index"
                        >Sửa
                      </span>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <ThePagnigation
        :pageNumber="pageNumber"
        :pageLoading="pageLoading"
        @pageChange="handlePageChange"
        @pageSizeChange="handlePageSizeChange"
      />
    </div>
    <ContextMenu
      v-if="showMenu"
      :actions="contextMenuActions"
      @actionClick="handleActionClick"
      @close="closeContextMenu"
      :position="menuPosition"
    ></ContextMenu>
    <CustomerForm
      v-if="showForm"
      :id="customerUpdateId"
      @closeForm="closeForm"
      @stopLoading="stopLoading"
    />

    <div class="toast-list">
      <VToast
        v-for="(toast, index) in toasts"
        :key="index"
        :type="toast.type"
        :message="toast.message"
        :index="index"
        @delete="deleteToast(index)"
      ></VToast>
    </div>
  </div>
</template>

<script setup lang="ts">
import '/src/styles/component/button.scss'
import '/src/styles/component/select.css'
import '/src/styles/component/input.css'
import '/src/styles/layout/toolbar.css'
import '/src/styles/layout/table.scss'
import '/src/styles/utils.css'
import ThePagnigation from '@/components/ThePagnigation.vue'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useStore } from 'vuex'
import ContextMenu from '@/components/ContextMenu.vue'
import type { ActionMenu } from '@/entities/ActionMenu'
import VToast from '@/components/VToast.vue'
import type { Toast } from '@/entities/Toast'
import router from '@/router'
import CustomerForm from './CustomerForm.vue'
import FolderList from '../FolderList.vue'
import { formatDate } from '@/utils'
import type { UserFile } from '@/entities/File'

const showForm = ref(false)
const pageLoading = ref(false)
const refreshLoading = ref(false)
const searchLoading = ref(false)
const addLoading = ref(false)
const updateLoading = ref(-1)
const deleteLoading = ref(-1)
const exportLoading = ref(false)
const errorMessage = ref<string | null>(null)
const tableContainer = ref<HTMLDivElement | null>(null)

const toasts = ref<Toast[]>([])

const loadingFileId = ref('')
async function handleSelectFile(file: UserFile) {
  loadingFileId.value = file.FileId ?? ''
  const result = await store.dispatch('readFileCustomer', file.FileId)
  loadingFileId.value = ''
  if (result.success === false) errorMessage.value = result.message
  else errorMessage.value = null
}

const contextMenuActions = ref<ActionMenu[]>([
  { label: 'Sửa', action: 'update' },
  { label: 'Sửa', action: 'update' },
  { label: 'Xoá', action: 'delete' },
  { label: 'Xoá', action: 'delete' },
  { label: 'Xoá', action: 'delete' },
])

function importFile() {
  router.push({
    name: 'importPage',
    params: {
      data: 'Customers',
    },
  })
}

function deleteToast(index: number) {
  toasts.value.splice(index, 1)
}

function handleActionClick(action: ActionMenu) {
  if (action.action === 'update') {
    updateCustomer(targetCustomerId.value, targetIndex.value)
  } else {
    deleteCustomer(targetCustomerId.value, targetIndex.value)
    closeContextMenu()
  }
}

const targetCustomerId = ref<string>('')
const targetIndex = ref(-1)

const pageSize = ref(10)
const pageNumber = ref(1)
const keyword = ref<string>()
const customerUpdateId = ref<string>('')
const store = useStore()

const showPopupAction = ref<number>(-1)
const showMenu = ref(false)
const groupId = ref<string | null>(null)

const popupPosition = ref({
  top: 0,
  right: 100,
})

const menuPosition = ref({
  top: 0,
  left: 0,
})

function showContextMenu(event: MouseEvent, employeeId: string, index: number) {
  event.preventDefault()
  menuPosition.value.top = event.clientY
  menuPosition.value.left = event.clientX
  showMenu.value = true
  showPopupAction.value = -1
  targetCustomerId.value = employeeId
  targetIndex.value = index
}

function closeContextMenu() {
  showMenu.value = false
}

let activeButton: HTMLElement | null = null
function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.target instanceof HTMLElement ? event.target : null
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - 2 * buttonRect.height,
    right: window.innerWidth - buttonRect.left,
  }

  activeButton = target

  showPopupAction.value = showPopupAction.value === index ? -1 : index
  showMenu.value = false
}

async function handleRefresh() {
  refreshLoading.value = true
  await fetchCustomers()
  refreshLoading.value = false
}

function updatePopupPosition() {
  if (!activeButton) return
  const buttonRect = activeButton.getBoundingClientRect()
  popupPosition.value = {
    top: buttonRect.top,
    right: window.innerWidth - buttonRect.left,
  }
}

function closeForm() {
  showForm.value = false
  fetchCustomers()
}

function addNew() {
  addLoading.value = true
  showForm.value = true
  customerUpdateId.value = ''
}

function updateCustomer(id: string, index: number) {
  updateLoading.value = index
  showForm.value = true
  customerUpdateId.value = id
}

async function deleteCustomer(id: string, index: number) {
  deleteLoading.value = index
  await store.dispatch('deleteCustomer', {
    id: id,
    token: accessToken.value,
  })
  await fetchCustomers()
  deleteLoading.value = -1
  showPopupAction.value = -1
}

function stopLoading() {
  addLoading.value = false
  showMenu.value = false
  updateLoading.value = -1
  showPopupAction.value = -1
}

async function handlePageSizeChange(newPageSize: number) {
  pageLoading.value = true
  pageSize.value = newPageSize
  pageNumber.value = 1
  await fetchCustomers()
  pageLoading.value = false
}

async function handlePageChange(newPageNumber: number) {
  pageLoading.value = true
  pageNumber.value = newPageNumber
  await fetchCustomers()
  pageLoading.value = false
}

async function handleSearch() {
  searchLoading.value = true
  pageNumber.value = 1
  await fetchCustomers()
  searchLoading.value = false
}

async function handleExportFile() {
  exportLoading.value = true
  await store.dispatch('downloadExcel', {
    datas: customers.value,
    token: accessToken.value,
    object: 'Customers',
  })
  exportLoading.value = false
}

async function fetchCustomers() {
  await store.dispatch('fetchCustomers', {
    pageSize: pageSize.value,
    pageNumber: pageNumber.value,
    keyword: keyword.value,
    token: accessToken.value,
    groupId: groupId.value,
  })
  scrollTable()
}

function scrollTable() {
  if (tableContainer.value) {
    tableContainer.value.scrollTo({
      top: 0,
      behavior: 'smooth',
    })
  }
}

const customers = computed(() => store.getters.getCustomers)
const accessToken = computed(() => store.getters.getAccessToken)

onMounted(() => {
  fetchCustomers()
  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.addEventListener('scroll', updatePopupPosition)
})

onUnmounted(() => {
  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.removeEventListener('scroll', updatePopupPosition)
})
</script>
