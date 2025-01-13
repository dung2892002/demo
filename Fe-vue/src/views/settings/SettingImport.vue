<template>
  <div class="content">
    <div class="content-main">
      <div class="toolbar">
        <div class="toolbar_search content--row">
          <button class="content__button button--add">
            <div class="content__button button--add" @click="showForm = true">
              <span class="button--add-text">Thêm mới</span>
            </div>
          </button>
        </div>
        <div class="toolbar__actions">
          <button class="toolbar-action" @click="handleRefresh()" v-loading="refreshLoading">
            <img src="/src/assets/icon/refresh.png" alt="logo" />
          </button>
        </div>
      </div>
      <div class="form" v-if="showForm">
        <div>
          <span>Đối tượng</span>
          <select name="" id="" v-model="importValue.TableName">
            <option v-for="(table, index) in tables" :key="index" :value="table">
              {{ table }}
            </option>
          </select>
        </div>
        <div>
          <span>Tên thuộc tính</span>
          <select name="" id="" v-model="importValue.PropertyName">
            <option v-for="(property, index) in filteredProperties" :key="index" :value="property">
              {{ property }}
            </option>
          </select>
        </div>
        <div>
          <span>Tên cột file excel</span>
          <input type="text" v-model="importValue.ColumnName" />
        </div>
        <div>
          <button @click="submit" class="button--add">Lưu</button>
          <button @click="cancel" class="button--remove">Hủy</button>
        </div>
      </div>
      <div class="main-container" ref="tableContainer">
        <table class="employee-table">
          <thead>
            <tr>
              <th class="w-6">STT</th>
              <th class="w-10">Tên cột</th>
              <th class="w-20">Tên thuộc tính</th>
              <th class="w-10">Đối tượng</th>
              <th class="w-10">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(setting, index) in settings" :key="setting.Id">
              <td>{{ index + 1 }}</td>
              <td>{{ setting.ColumnName }}</td>
              <td>{{ setting.PropertyName }}</td>
              <td>{{ setting.TableName }}</td>
              <td>
                <div style="display: flex; flex-direction: row; justify-content: center">
                  <button @click="update(setting)" class="button--add">Sửa</button>
                  <button
                    @click="deleteSetting(setting)"
                    style="margin-left: 10px; background-color: red"
                  >
                    xóa
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Import } from '@/entities/Import'
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const refreshLoading = ref(false)
const showForm = ref(false)

const importValue = ref<Import>({
  Id: 0,
  TableName: 'Employee',
  ColumnName: '',
  PropertyName: '',
})

const properties = {
  Employee: [
    'EmployeeCode',
    'Fullname',
    'DateOfBirth',
    'GenderName',
    'IdentityNumber',
    'IdentityDate',
    'IdentityPlace',
    'Address',
    'MobileNumber',
    'LandlineNumber',
    'Email',
    'BankNumber',
    'BankName',
    'BankBranch',
    'PositionName',
    'DepartmentName',
  ],
  Customer: [
    'CustomerCode',
    'Fullname',
    'DateOfBirth',
    'GenderName',
    'Address',
    'MobileNumber',
    'Email',
    'Amount',
    'GroupName',
  ],
}

const filteredProperties = computed(() => {
  const tableName = importValue.value.TableName
  return properties[tableName as keyof typeof properties] || []
})

const tables = ['Employee', 'Customer']

async function submit() {
  if (importValue.value.Id === 0) {
    const response = await store.dispatch('createImport', importValue.value)
    if (response) showForm.value = false
  } else {
    const response = await store.dispatch('updateImport', importValue.value)
    if (response) showForm.value = false
  }
  fetchSettings()
}

function cancel() {
  showForm.value = false
}

function update(importSetting: Import) {
  importValue.value = importSetting
  showForm.value = true
}

async function deleteSetting(importSetting: Import) {
  const response = await store.dispatch('deleteImport', importSetting)
  if (response) fetchSettings()
}

async function handleRefresh() {
  refreshLoading.value = true
  await fetchSettings()
  refreshLoading.value = false
}

async function fetchSettings() {
  await store.dispatch('fecthImportSetting')
}

const settings = computed(() => store.getters.getImportSettings)
onMounted(() => {
  fetchSettings()
})
</script>
