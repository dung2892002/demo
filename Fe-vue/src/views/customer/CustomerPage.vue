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
    <div class="content-main">
      <div class="toolbar">
        <div style="display: flex; flex-direction: row; gap: 20px">
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

          <div class="toolbar_sort">
            <span v-loading="sortNameLoading" @click="handleSortByName()">Theo tên</span>
            <span v-loading="sortDateLoading" @click="handleSortByDate()">Theo ngày tạo</span>
            <span v-loading="sortTypeLoading" @click="handleSortByType()">Theo loại</span>
          </div>
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
      <div class="content__filter">
        <div class="folder-route">
          <span @click="goStartFolder" v-if="listFolder.length > 0">Khách hàng</span>
          <div
            v-for="(folder, index) in listFolder"
            :key="index"
            @click="routeFolder(folder, index)"
          >
            ><span> {{ folder.Name }}</span>
          </div>
        </div>
      </div>
      <div class="main-container" ref="tableContainer">
        <table class="employee-table">
          <thead>
            <tr>
              <th class="w-6">STT</th>
              <th class="w-20">Tên</th>
              <th class="w-10">Mã KH</th>
              <th class="w-10">Giới tính</th>
              <th class="w-12">Ngày sinh</th>
              <th class="w-30">Email</th>
              <th>Địa chỉ</th>
              <th class="w-10">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(folder, index) in customerFolders"
              :key="index"
              @contextmenu.prevent="showContextMenu($event, folder.CustomerId, index)"
              style="cursor: pointer"
              @click="selectFolder(folder)"
            >
              <td>{{ index + 1 }}</td>
              <td>
                <img
                  src="/src/assets/icon/folder.png"
                  v-if="folder.Type"
                  style="width: 24px; height: 24px; margin-right: 6px; vertical-align: middle"
                />
                <span style="vertical-align: middle">{{ folder.Name }}</span>
              </td>
              <td>{{ folder.Customer?.CustomerCode }}</td>
              <td>{{ folder.Customer?.GenderName }}</td>
              <td>{{ folder.Customer ? formatDate(folder.Customer?.DateOfBirth) : '' }}</td>
              <td>{{ folder.Customer?.Email }}</td>
              <td>{{ folder.Customer?.Address }}</td>
              <td>
                <div class="action" :ref="`action-${index}`" v-if="folder.Type == false">
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
                        @click="deleteCustomer(folder.CustomerId, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span @click="deleteCustomer(folder.CustomerId, index)">Xóa</span>
                      <span
                        @click="updateCustomer(folder.CustomerId, index)"
                        v-loading="updateLoading == index"
                        >Sửa
                      </span>
                      <span
                        @click="updateCustomer(folder.CustomerId, index)"
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
        <ThePagnigation
          :pageNumber="pageNumber"
          :pageLoading="pageLoading"
          @pageChange="handlePageChange"
          @pageSizeChange="handlePageSizeChange"
        />
      </div>
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
  </div>
</template>

<script setup lang="ts">
import '/src/styles/component/button.scss'
import '/src/styles/component/select.css'
import '/src/styles/component/input.css'
import '/src/styles/layout/toolbar.scss'
import '/src/styles/layout/table.scss'
import '/src/styles/utils.css'

import ThePagnigation from '@/components/ThePagnigation.vue'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useStore } from 'vuex'
import ContextMenu from '@/components/ContextMenu.vue'
import type { ActionMenu } from '@/entities/ActionMenu'
import router from '@/router'
import CustomerForm from './CustomerForm.vue'
import { formatDate } from '@/utils'
import axios from 'axios'
import type { CustomerFolder } from '@/entities/CustomerFolder'

const showForm = ref(false)
const pageLoading = ref(false)
const refreshLoading = ref(false)
const searchLoading = ref(false)
const addLoading = ref(false)
const updateLoading = ref(-1)
const deleteLoading = ref(-1)
const exportLoading = ref(false)
const tableContainer = ref<HTMLDivElement | null>(null)
const contextMenuActions = ref<ActionMenu[]>([
  { label: 'Sửa', action: 'update' },
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

const listFolder = ref<CustomerFolder[]>([])

const popupPosition = ref({
  top: 0,
  right: 100,
})

const menuPosition = ref({
  top: 0,
  left: 0,
})

function showContextMenu(event: MouseEvent, customerId: string | null, index: number) {
  if (customerId) {
    menuPosition.value.top = event.clientY
    menuPosition.value.left = event.clientX
    showMenu.value = true
    showPopupAction.value = -1
    targetCustomerId.value = customerId
    targetIndex.value = index
  }
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
  fetchCustomerFolder()
}

function addNew() {
  addLoading.value = true
  showForm.value = true
  customerUpdateId.value = ''
}

function updateCustomer(id: string | null, index: number) {
  updateLoading.value = index
  showForm.value = true
  if (id) customerUpdateId.value = id
}

async function deleteCustomer(id: string | null, index: number) {
  deleteLoading.value = index
  await store.dispatch('deleteCustomer', {
    id: id,
    token: accessToken.value,
  })
  await fetchCustomerFolder()
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

const parentId = ref<string | null>(null)
const sortName = ref<boolean | null>(null)
const sortDate = ref<boolean | null>(null)
const sortType = ref<boolean | null>(true)

const sortNameLoading = ref(false)
const sortDateLoading = ref(false)
const sortTypeLoading = ref(false)

const customerFolders = ref<CustomerFolder[]>([])

async function fetchCustomerFolder() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Customers/folder', {
      params: {
        parentId: parentId.value,
        sortName: sortName.value,
        sortDate: sortDate.value,
        sortType: sortType.value,
      },
    })

    customerFolders.value = response.data
  } catch (error) {
    console.log(error)
  }
}

function selectFolder(folder: CustomerFolder) {
  if (folder.Type) {
    parentId.value = folder.Id
    sortName.value = null
    sortDate.value = null
    sortType.value = true
    fetchCustomerFolder()
    listFolder.value.push(folder)
  }
}

async function handleSortByName() {
  if (sortName.value === null) {
    sortName.value = true
  } else {
    sortName.value = !sortName.value
  }
  sortType.value = null
  sortDate.value = null

  sortNameLoading.value = true
  await fetchCustomerFolder()
  sortNameLoading.value = false
}

async function handleSortByType() {
  sortType.value = !sortType.value
  sortName.value = null
  sortDate.value = null

  sortTypeLoading.value = true
  await fetchCustomerFolder()
  sortTypeLoading.value = false
}

async function handleSortByDate() {
  if (sortDate.value === null) {
    sortDate.value = true
  } else {
    sortDate.value = !sortDate.value
  }
  sortName.value = null
  sortType.value = null

  sortDateLoading.value = true
  await fetchCustomerFolder()
  sortDateLoading.value = false
}

function goStartFolder() {
  parentId.value = null
  sortName.value = null
  sortDate.value = null
  sortType.value = null
  fetchCustomerFolder()
  listFolder.value = []
}

function routeFolder(folder: CustomerFolder, index: number) {
  parentId.value = folder.Id
  sortName.value = null
  sortDate.value = null
  sortType.value = true
  fetchCustomerFolder()
  listFolder.value = listFolder.value.slice(0, index + 1)
}

const customers = computed(() => store.getters.getCustomers)
const accessToken = computed(() => store.getters.getAccessToken)

onMounted(() => {
  fetchCustomerFolder()
  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.addEventListener('scroll', updatePopupPosition)
})

onUnmounted(() => {
  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.removeEventListener('scroll', updatePopupPosition)
})
</script>
