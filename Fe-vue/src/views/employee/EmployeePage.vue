<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Quản lý nhân viên</h1>
      <div class="header-btn">
        <div class="btn btn--blue" @click="addNew">
          <img src="/src/assets/icon/add.png" alt="logo" class="button--add-logo" />
          <span class="button--add-text">Thêm mới</span>
        </div>
      </div>
    </div>

    <span v-if="errorMessage" class="error-message">{{ errorMessage }}</span>
    <div class="content-main">
      <div class="toolbar">
        <div style="display: flex; flex-direction: row; gap: 20px">
          <div class="toolbar_search">
            <input
              type="text"
              id="search-employee"
              placeholder="Tìm kiếm theo mã, họ tên"
              v-model="keyword"
              @input="handleSearch"
            />
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
          <span @click="goStartFolder" v-if="listFolder.length > 0">Nhân viên</span>
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
              v-for="(folder, index) in employeeFolders"
              :key="index"
              @contextmenu.prevent="showContextMenu($event, folder.EmployeeId, index)"
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
              <td>{{ folder.Employee?.EmployeeCode }}</td>
              <td>{{ folder.Employee?.GenderName }}</td>
              <td>{{ folder.Employee ? formatDate(folder.Employee?.DateOfBirth) : '' }}</td>
              <td>{{ folder.Employee?.Email }}</td>
              <td>{{ folder.Employee?.Address }}</td>
              <td>
                <div class="action" :ref="`action-${index}`" v-if="folder.Type == false">
                  <div class="action-buttons">
                    <button class="action-button" @click="togglePopupAction(index, $event)">
                      <font-awesome-icon
                        :icon="['fas', 'ellipsis-vertical']"
                        class="button__icon"
                        :class="{ selected: showPopupAction === index }"
                      />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span
                        @click="deleteEmployee(folder.EmployeeId, index)"
                        v-loading="deleteLoading == index"
                        ><font-awesome-icon :icon="['fas', 'trash']" /><span>Xóa</span></span
                      >

                      <span
                        @click="deleteEmployee(folder.EmployeeId, index)"
                        v-loading="deleteLoading == index"
                        ><font-awesome-icon :icon="['fas', 'trash']" /><span>Xóa</span></span
                      >
                      <span
                        @click="updateEmployee(folder.EmployeeId, index)"
                        v-loading="updateLoading == index"
                      >
                        <font-awesome-icon icon="pen-to-square" />
                        <span>Sửa</span>
                      </span>
                      <span
                        @click="updateEmployee(folder.EmployeeId, index)"
                        v-loading="updateLoading == index"
                      >
                        <font-awesome-icon icon="pen-to-square" />
                        <span>Sửa</span>
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

    <EmployeeForm
      v-if="showForm"
      :id="employeeUpdateId"
      @closeForm="closeForm"
      @stopLoading="stopLoading"
    />
  </div>
</template>

<script setup lang="ts">
import '/src/styles/component/button.scss'
import '/src/styles/component/select.css'
import '/src/styles/component/input.scss'
import '/src/styles/layout/toolbar.scss'
import '/src/styles/layout/table.scss'
import '/src/styles/utils.css'
import ThePagnigation from '@/components/ThePagnigation.vue'
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'
import ContextMenu from '@/components/ContextMenu.vue'
import type { ActionMenu } from '@/entities/ActionMenu'
import router from '@/router'
import EmployeeForm from './EmployeeForm.vue'
import { formatDate } from '@/utils'
import axios from 'axios'
import type { EmployeeFolder } from '../../entities/EmployeeFolder'

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
      data: 'Employees',
    },
  })
}

function handleActionClick(action: ActionMenu) {
  if (action.action === 'update') {
    updateEmployee(targetEmployeeId.value, targetIndex.value)
  } else {
    deleteEmployee(targetEmployeeId.value, targetIndex.value)
    closeContextMenu()
  }
}

const targetEmployeeId = ref<string>('')
const targetIndex = ref(-1)

const pageSize = ref(10)
const pageNumber = ref(1)
const keyword = ref<string | null>()
const employeeUpdateId = ref<string>('')
const store = useStore()

const showPopupAction = ref<number>(-1)
const showMenu = ref(false)

const listFolder = ref<EmployeeFolder[]>([])

const popupPosition = ref({
  top: 0,
  right: 100,
})

const menuPosition = ref({
  top: 0,
  left: 0,
})

function showContextMenu(event: MouseEvent, employeeId: string | null, index: number) {
  menuPosition.value.top = event.clientY
  menuPosition.value.left = event.clientX
  showMenu.value = true
  showPopupAction.value = -1
  targetIndex.value = index
  targetEmployeeId.value = employeeId || ''
}

function closeContextMenu() {
  showMenu.value = false
}

function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - buttonRect.height,
    right: window.innerWidth - buttonRect.left,
  }

  showPopupAction.value = showPopupAction.value === index ? -1 : index
}

async function handleRefresh() {
  refreshLoading.value = true
  await fetchEmployeeFolders()
  refreshLoading.value = false
}

function closeForm() {
  showForm.value = false
  fetchEmployeeFolders()
}

function addNew() {
  addLoading.value = true
  showForm.value = true
  employeeUpdateId.value = ''
}

function updateEmployee(id: string | null, index: number) {
  updateLoading.value = index
  showForm.value = true
  if (id) employeeUpdateId.value = id
}

async function deleteEmployee(id: string | null, index: number) {
  deleteLoading.value = index
  await store.dispatch('deleteEmployee', {
    id: id,
    token: accessToken.value,
  })
  await fetchEmployeeFolders()
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
  await fetchEmployeeFolders()
  pageLoading.value = false
}

async function handlePageChange(newPageNumber: number) {
  pageLoading.value = true
  pageNumber.value = newPageNumber
  await fetchEmployeeFolders()
  pageLoading.value = false
}

let timeout: ReturnType<typeof setTimeout> | null = null

function handleSearch() {
  if (timeout) {
    clearTimeout(timeout)
  }

  timeout = setTimeout(() => {
    searchLoading.value = true
    pageNumber.value = 1
    fetchEmployeeFolders()
    searchLoading.value = false
  }, 500)
}

async function handleExportFile() {
  exportLoading.value = true
  await store.dispatch('downloadExcel', {
    datas: employees.value,
    token: accessToken.value,
    object: 'Employees',
  })
  exportLoading.value = false
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
const sortType = ref<boolean | null>(false)

const sortNameLoading = ref(false)
const sortDateLoading = ref(false)
const sortTypeLoading = ref(false)
const employeeFolders = ref<EmployeeFolder[]>([])

async function fetchEmployeeFolders() {
  try {
    const response = await axios.get('https://localhost:7160/api/v1/Employees/folder', {
      params: {
        parentId: parentId.value,
        keyword: keyword.value,
        pageNumber: pageNumber.value,
        pageSize: pageSize.value,
        sortName: sortName.value,
        sortDate: sortDate.value,
        sortType: sortType.value,
      },
    })

    employeeFolders.value = response.data.Items
    store.commit('setTotalRecords', response.data.TotalItems)
    store.commit('setTotalPages', response.data.TotalPages)
    scrollTable()
  } catch (error) {
    console.log(error)
  }
}

function resetQuery() {
  parentId.value = null
  keyword.value = null
  sortName.value = null
  sortDate.value = null
  sortType.value = false
  pageNumber.value = 1
}

function selectFolder(folder: EmployeeFolder) {
  if (folder.Type) {
    resetQuery()
    parentId.value = folder.Id
    fetchEmployeeFolders()
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
  await fetchEmployeeFolders()
  sortNameLoading.value = false
}

async function handleSortByType() {
  sortType.value = !sortType.value
  sortName.value = null
  sortDate.value = null

  sortTypeLoading.value = true
  await fetchEmployeeFolders()
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
  await fetchEmployeeFolders()
  sortDateLoading.value = false
}

function goStartFolder() {
  resetQuery()
  fetchEmployeeFolders()
  listFolder.value = []
}

function routeFolder(folder: EmployeeFolder, index: number) {
  resetQuery()
  parentId.value = folder.Id
  fetchEmployeeFolders()
  listFolder.value = listFolder.value.slice(0, index + 1)
}

const employees = computed(() => store.getters.getEmployees)
const accessToken = computed(() => store.getters.getAccessToken)

onMounted(() => {
  store.dispatch('fetchDepartments')
  store.dispatch('fetchPositions')
  fetchEmployeeFolders()
})
</script>
