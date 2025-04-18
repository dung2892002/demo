<template>
  <div class="form-container" id="employeeForm" v-if="employee.EmployeeCode.length > 0">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Thông tin nhân viên</h2>
        <button class="form__button" @click="handleCloseForm">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form">
        <span v-if="error" class="error-message">{{ error }}</span>
        <div class="form-group">
          <div class="form__item form__item--3">
            <label for="employee-code" class="form__label"
              >Mã nhân viên <span class="required">*</span></label
            >
            <input
              type="text"
              id="employee-code"
              class="form__input"
              name="employee-code"
              v-model="employee.EmployeeCode"
              readonly
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="fullname" class="form__label"
              >Họ và tên <span class="required">*</span></label
            >
            <input
              type="text"
              id="fullname"
              class="form__input"
              name="fullname"
              v-model="employee.Fullname"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="date-of-birth" class="form__label"
              >Ngày sinh <span class="required">*</span></label
            >
            <input
              type="date"
              id="date-of-birth"
              class="form__input form__input--date"
              name="date-of-birth"
              v-model="employee.DateOfBirth"
              required
            />
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--3">
            <label for="phone-number" class="form__label"
              >Số điện thoại <span class="required">*</span></label
            >
            <input
              type="text"
              id="phone-number"
              class="form__input"
              name="phone-number"
              v-model="employee.MobileNumber"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="email" class="form__label">Email <span class="required">*</span></label>
            <input
              type="email"
              id="email"
              class="form__input"
              name="email"
              v-model="employee.Email"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="male" class="form__label">Giới tính <span class="required">*</span></label>
            <div class="form__checkbox-group">
              <div class="form__checkbox">
                <input
                  type="radio"
                  id="female"
                  name="gender"
                  :value="0"
                  class="form__input form__input--radio"
                  v-model="employee.Gender"
                />
                <span>Nữ</span>
              </div>
              <div class="form__checkbox">
                <input
                  type="radio"
                  id="male"
                  name="gender"
                  :value="1"
                  class="form__input form__input--radio"
                  v-model="employee.Gender"
                />
                <span>Nam</span>
              </div>
              <div class="form__checkbox">
                <input
                  type="radio"
                  id="other"
                  name="gender"
                  :value="2"
                  class="form__input form__input--radio"
                  v-model="employee.Gender"
                />
                <span>Khác</span>
              </div>
            </div>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--2">
            <label for="landlineNumber" class="form__label"
              >Sdt cố định<span class="required">*</span></label
            >
            <input
              type="text"
              id="landlineNumber"
              class="form__input"
              name="address"
              v-model="employee.LandlineNumber"
              required
            />
          </div>
          <div class="form__item form__item--2">
            <label for="address" class="form__label">Địa chỉ <span class="required">*</span></label>
            <input
              type="text"
              id="address"
              class="form__input"
              name="address"
              v-model="employee.Address"
              required
            />
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--3">
            <label for="identity-number" class="form__label"
              >Số CCCD<span class="required">*</span></label
            >
            <input
              type="text"
              id="identity-number"
              class="form__input"
              name="identity-number"
              v-model="employee.IdentityNumber"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="identity-date" class="form__label"
              >Ngày cấp <span class="required">*</span></label
            >
            <input
              type="date"
              id="identity-date"
              class="form__input form__input--date"
              name="identity-date"
              v-model="employee.IdentityDate"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="identity-place" class="form__label"
              >Nơi cấp<span class="required">*</span></label
            >
            <input
              type="text"
              id="identity-place"
              class="form__input"
              name="identity-place"
              v-model="employee.IdentityPlace"
              required
            />
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--3">
            <label for="bank-number" class="form__label"
              >Số tài khoản <span class="required">*</span></label
            >
            <input
              type="text"
              id="bank-number"
              class="form__input"
              name="bank-number"
              v-model="employee.BankNumber"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="bank-name" class="form__label"
              >Ngân hàng<span class="required">*</span></label
            >
            <input
              type="text"
              id="bank-name"
              class="form__input"
              name="bank-name"
              v-model="employee.BankName"
              required
            />
          </div>
          <div class="form__item form__item--3">
            <label for="salary" class="form__label"
              >Chi nhánh <span class="required">*</span></label
            >
            <input
              type="text"
              id="salary"
              class="form__input"
              name="salary"
              v-model="employee.BankBranch"
              required
            />
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--2">
            <label for="department" class="form__label">Phòng ban</label>
            <div class="form-select" style="width: 100%;">
              <div class="select-value" @click="toggleSelectDepartment">
                <div
                  v-for="(department, index) in employee.DepartmentName.slice(0, showMaxSelect)"
                  :key="index"
                >
                  <span>{{ department }}</span>
                  <span @click.stop="removeDepartmentByIndex(index)">x</span>
                </div>
                <div v-if="employee.DepartmentId.length > showMaxSelect" class="select--overlength">
                  {{ employee.DepartmentId.length - showMaxSelect }}+
                  <div class="select--overlength-value">
                    <div
                      v-for="(department, index) in employee.DepartmentName.slice(showMaxSelect)"
                      :key="index"
                    >
                      <span>{{ department }}</span>
                      <span @click.stop="removeDepartmentByIndex(index + showMaxSelect)">x</span>
                    </div>
                  </div>
                </div>
              </div>
              <div v-if="showSelectDepartment == true" class="select-option">
                <span
                  v-for="department in departments"
                  :key="department.DepartmentId"
                  class="select-item"
                  :class="{ 'select-item--selected': checkContainDepartment(department) }"
                  @click="
                    checkContainDepartment(department)
                      ? removeDepartment(department)
                      : selectDepartment(department)
                  "
                >
                  <img
                    src="/src/assets/icon/icons8-yes-50.png"
                    alt="logo"
                    v-if="checkContainDepartment(department)"
                  />
                  <span>{{ department.DepartmentName }}</span>
                </span>
              </div>
            </div>
          </div>
          <div class="form__item form__item--2">
            <label for="position" class="form__label">Vị trí</label>
            <div class="form-select" style="width: 100%;">
              <div class="select-value" @click="toggleSelectPosition">
                <div
                  v-for="(position, index) in employee.PositionName.slice(0, showMaxSelect)"
                  :key="index"
                >
                  <span>{{ position }}</span>
                  <span @click.stop="removePositionByIndex(index)">x</span>
                </div>
                <div v-if="employee.PositionId.length > 3" class="select--overlength">
                  {{ employee.PositionId.length - 3 }}+
                  <div class="select--overlength-value">
                    <div
                      v-for="(position, index) in employee.PositionName.slice(showMaxSelect)"
                      :key="index"
                    >
                      <span>{{ position }}</span>
                      <span @click.stop="removePositionByIndex(index + showMaxSelect)">x</span>
                    </div>
                  </div>
                </div>
              </div>
              <div v-if="showSelectPosition == true" class="select-option">
                <span
                  v-for="position in positions"
                  :key="position.Id"
                  class="select-item"
                  :class="{ 'select-item--selected': checkContainPosition(position) }"
                  @click="
                    checkContainPosition(position)
                      ? removePosition(position)
                      : selectPosition(position)
                  "
                >
                  <img
                    src="/src/assets/icon/icons8-yes-50.png"
                    alt="logo"
                    v-if="checkContainPosition(position)"
                  />
                  <span>{{ position.PositionName }}</span>
                </span>
              </div>
            </div>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="handleCloseForm">Hủy</button>
        <button
          class="button--complete"
          id="submitButton"
          @click="handleSubmitForm"
          v-loading="loading"
        >
          <span src="/src/assets/icon/refresh.png" alt="logo">Lưu</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import '/src/styles/layout/form.scss'
import type { Employee } from '@/entities/Employee'
import { useStore } from 'vuex'
import type { Position } from '@/entities/Position'
import type { Department } from '@/entities/Department'

const store = useStore()
const loading = ref(false)
const error = ref<string | null>('')
const showMaxSelect = 3

const showSelectPosition = ref(false)
const showSelectDepartment = ref(false)

const employee = ref<Employee>({
  EmployeeId: undefined,
  Fullname: '',
  EmployeeCode: '',
  DateOfBirth: '',
  Gender: 1,
  GenderName: 'Nam',
  IdentityNumber: '',
  IdentityPlace: '',
  IdentityDate: '',
  Address: '',
  MobileNumber: '',
  LandlineNumber: '',
  Email: '',
  BankNumber: '',
  BankName: '',
  BankBranch: '',
  PositionId: [],
  PositionCode: '',
  PositionName: [],
  DepartmentId: [],
  DepartmentCode: '',
  DepartmentName: [],
  CreatedDate: new Date(),
  CreatedBy: '',
  ModifiedDate: new Date(),
  ModifiedBy: '',
})

const props = defineProps({
  id: {
    type: String,
    default: null,
  },
})

const emits = defineEmits(['closeForm', 'stopLoading'])

function handleCloseForm() {
  emits('closeForm')
}

function toggleSelectPosition() {
  showSelectPosition.value = !showSelectPosition.value
  showSelectDepartment.value = false
}

function toggleSelectDepartment() {
  showSelectDepartment.value = !showSelectDepartment.value
  showSelectPosition.value = false
}

function checkContainPosition(position: Position) {
  if (employee.value.PositionId.includes(position.PositionId)) return true
  return false
}

function selectPosition(position: Position) {
  if (!checkContainPosition(position)) {
    employee.value.PositionId = [...employee.value.PositionId, position.PositionId]
    employee.value.PositionName = [...employee.value.PositionName, position.PositionName]
  }
}

function removePositionByIndex(index: number) {
  employee.value.PositionId.splice(index, 1)
  employee.value.PositionName.splice(index, 1)
}

function removePosition(position: Position) {
  const index = employee.value.PositionId.indexOf(position.PositionId)
  removePositionByIndex(index)
}

function checkContainDepartment(department: Department) {
  if (employee.value.DepartmentId.includes(department.DepartmentId)) return true
  return false
}

function selectDepartment(department: Department) {
  if (!checkContainDepartment(department)) {
    employee.value.DepartmentId = [...employee.value.DepartmentId, department.DepartmentId]
    employee.value.DepartmentName = [...employee.value.DepartmentName, department.DepartmentName]
  }
}

function removeDepartmentByIndex(index: number) {
  employee.value.DepartmentId.splice(index, 1)
  employee.value.DepartmentName.splice(index, 1)
}

function removeDepartment(department: Department) {
  const index = employee.value.DepartmentId.indexOf(department.DepartmentId)
  removeDepartmentByIndex(index)
}

async function handleSubmitForm() {
  loading.value = true

  employee.value.GenderName =
    employee.value.Gender === 0 ? 'Nữ' : employee.value.Gender === 1 ? 'Nam' : 'Không rõ'
  if (props.id) {
    const response = await store.dispatch('updateEmployee', {
      id: props.id,
      employee: employee.value,
      token: token.value,
    })
    if (response.success) handleCloseForm()
    else error.value = response.message
  } else {
    const response = await store.dispatch('createEmployee', {
      employee: employee.value,
      token: token.value,
    })
    if (response.success) handleCloseForm()
    else error.value = response.message
  }
  loading.value = false
}

async function fetchEmployeeData() {
  if (props.id) {
    await store.dispatch('fetchEmployee', props.id)
    const empStore = store.getters.getEmployee
    employee.value = empStore
    employee.value.DateOfBirth = formatDateForm(employee.value.DateOfBirth)
    employee.value.IdentityDate = formatDateForm(employee.value.IdentityDate)
    employee.value.PositionId = [empStore.PositionId]
    employee.value.PositionName = [empStore.PositionName]
    employee.value.DepartmentId = [empStore.DepartmentId]
    employee.value.DepartmentName = [empStore.DepartmentName]
  } else {
    await store.dispatch('fetchNewEmployeeCode')
    employee.value.EmployeeCode = store.getters.getNewEmployeeCode
  }
  emits('stopLoading')
}

function formatDateForm(date: string) {
  const d = new Date(date)
  let month = '' + (d.getMonth() + 1)
  let day = '' + d.getDate()
  const year = d.getFullYear()

  if (month.length < 2) month = '0' + month
  if (day.length < 2) day = '0' + day

  return [year, month, day].join('-')
}

const positions = computed(() => store.getters.getPositions)
const departments = computed(() => store.getters.getDepartments)
const token = computed(() => store.getters.getAccessToken)
onMounted(() => {
  fetchEmployeeData()
})
</script>
