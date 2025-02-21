<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Quản lý nhân viên</h1>
      <button class="content__button button--add" @click="addNew" v-loading="addLoading">
        <div class="content__button button--add">
          <img src="/src/assets/icon/add.png" alt="logo" class="button--add-logo" />
          <span class="button--add-text">Thêm mới</span>
        </div>
      </button>
    </div>

    <span v-if="errorMessage" class="error-message">{{ errorMessage }}</span>
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
              <th class="w-10">Mã NV</th>
              <th class="w-20">Họ và tên</th>
              <th class="w-10">Giới tính</th>
              <th class="w-12">Ngày sinh</th>
              <th class="w-30">Email</th>
              <th>Địa chỉ</th>
              <th class="w-10">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(employee, index) in employees"
              :key="index"
              @contextmenu.prevent="showContextMenu($event, employee.EmployeeId, index)"
            >
              <td>{{ index + 1 }}</td>
              <td>{{ employee.EmployeeCode }}</td>
              <td>{{ employee.Fullname }}</td>
              <td>{{ employee.GenderName }}</td>
              <td>{{ formatDate(employee.DateOfBirth) }}</td>
              <td>{{ employee.Email }}</td>
              <td>{{ employee.Address }}</td>
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
                        @click="deleteEmployee(employee.EmployeeId, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span
                        @click="deleteEmployee(employee.EmployeeId, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span
                        @click="deleteEmployee(employee.EmployeeId, index)"
                        v-loading="deleteLoading == index"
                        >Xóa</span
                      >
                      <span
                        @click="updateEmployee(employee.EmployeeId, index)"
                        v-loading="updateLoading == index"
                        >Sửa
                      </span>
                      <span
                        @click="updateEmployee(employee.EmployeeId, index)"
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
import EmployeeForm from './EmployeeForm.vue'
import { formatDate } from '@/utils'

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
const keyword = ref<string>()
const employeeUpdateId = ref<string>('')
const store = useStore()

const showPopupAction = ref<number>(-1)
const showMenu = ref(false)

const departmentId = ref<string | null>(null)
const positionId = ref<string | null>(null)

const popupPosition = ref({
  top: 0,
  right: 100,
})

const menuPosition = ref({
  top: 0,
  left: 0,
})

function showContextMenu(event: MouseEvent, employeeId: string, index: number) {
  menuPosition.value.top = event.clientY
  menuPosition.value.left = event.clientX
  showMenu.value = true
  showPopupAction.value = -1
  targetEmployeeId.value = employeeId
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
  await fetchEmployees()
  refreshLoading.value = false
}

function updatePopupPosition() {
  if (!activeButton) return
  showPopupAction.value = -1
}

function closeForm() {
  showForm.value = false
  fetchEmployees()
}

function addNew() {
  addLoading.value = true
  showForm.value = true
  employeeUpdateId.value = ''
}

function updateEmployee(id: string, index: number) {
  updateLoading.value = index
  showForm.value = true
  employeeUpdateId.value = id
}

async function deleteEmployee(id: string, index: number) {
  deleteLoading.value = index
  await store.dispatch('deleteEmployee', {
    id: id,
    token: accessToken.value,
  })
  await fetchEmployees()
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
  await fetchEmployees()
  pageLoading.value = false
}

async function handlePageChange(newPageNumber: number) {
  pageLoading.value = true
  pageNumber.value = newPageNumber
  await fetchEmployees()
  pageLoading.value = false
}

async function handleSearch() {
  searchLoading.value = true
  pageNumber.value = 1
  await fetchEmployees()
  searchLoading.value = false
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

async function fetchEmployees() {
  await store.dispatch('fetchEmployees', {
    pageSize: pageSize.value,
    pageNumber: pageNumber.value,
    keyword: keyword.value,
    departmentId: departmentId.value,
    positionId: positionId.value,
    token: accessToken.value,
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

const employees = computed(() => store.getters.getEmployees)
const accessToken = computed(() => store.getters.getAccessToken)

onMounted(() => {
  store.dispatch('fetchDepartments')
  store.dispatch('fetchPositions')
  fetchEmployees()

  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.addEventListener('scroll', updatePopupPosition)
})

onUnmounted(() => {
  const scrollContainer = document.querySelector('.main-container')
  scrollContainer?.removeEventListener('scroll', updatePopupPosition)
})
</script>
