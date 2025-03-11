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

  <div class="form-container" v-if="showForm">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Thêm tệp</h2>
        <button class="form__button" @click="handleCloseForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <label class="form__label">Đối tượng <span class="required">*</span></label>
            <select name="" id="" v-model="importValue.TableName">
              <option v-for="(table, index) in tables" :key="index" :value="table">
                {{ table }}
              </option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item">
            <label class="form__label">Tên thuộc tính <span class="required">*</span></label>
            <select name="" id="" v-model="importValue.PropertyName">
              <option
                v-for="(property, index) in filteredProperties"
                :key="index"
                :value="property"
              >
                {{ property }}
              </option>
            </select>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item">
            <label class="form__label">Tên cột file excel <span class="required">*</span></label>
            <input type="text" v-model="importValue.ColumnName" />
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm(false)">Hủy bỏ</button>
        <button class="button--complete" id="submitButton" @click="submit">
          <span src="/src/assets/icon/refresh.png" alt="logo">Thêm</span>
        </button>
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

function handleCloseForm(state: boolean) {
  showForm.value = false
  if (state) fetchSettings()
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
