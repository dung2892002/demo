<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Thông tin khách hàng</h2>
        <button class="form__button" @click="handleCloseForm">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="hospital-form" id="form">
        <span v-if="error" class="error-message">{{ error }}</span>
        <div class="form-group">
          <div class="form__item form__item--3" v-if="props.id">
            <label for="employee-code" class="form__label"
              >Mã khách hàng <span class="required">*</span></label
            >
            <input
              type="text"
              id="employee-code"
              class="form__input"
              name="employee-code"
              v-model="customer.CustomerCode"
              readonly
              required
            />
          </div>
          <div class="form__item form__item--2">
            <label for="fullname" class="form__label"
              >Họ và tên <span class="required">*</span></label
            >
            <input
              type="text"
              id="fullname"
              class="form__input"
              name="fullname"
              v-model="customer.Fullname"
              required
            />
          </div>
          <div class="form__item form__item--2">
            <label for="date-of-birth" class="form__label"
              >Ngày sinh <span class="required">*</span></label
            >
            <input
              type="date"
              id="date-of-birth"
              class="form__input form__input--date"
              name="date-of-birth"
              v-model="customer.DateOfBirth"
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
              v-model="customer.MobileNumber"
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
              v-model="customer.Email"
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
                  v-model="customer.Gender"
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
                  v-model="customer.Gender"
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
                  v-model="customer.Gender"
                />
                <span>Khác</span>
              </div>
            </div>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item form__item--2">
            <label for="address" class="form__label">Địa chỉ <span class="required">*</span></label>
            <input
              type="text"
              id="address"
              class="form__input"
              name="address"
              v-model="customer.Address"
              required
            />
          </div>
          <div class="form__item form__item--2">
            <label for="customer-group" class="form__label">Nhóm khách hàng</label>
            <select v-model="customer.GroupId" class="form__input">
              <option :value="group.GroupId" v-for="group in customerGroups" :key="group.GroupId">
                {{ group.GroupName }}
              </option>
            </select>
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
import { useStore } from 'vuex'
import type { Customer } from '@/entities/Customer'
const store = useStore()
const loading = ref(false)

const error = ref<string | null>(null)

const customer = ref<Customer>({
  CustomerId: undefined,
  CustomerCode: '',
  Fullname: '',
  DateOfBirth: '',
  Gender: 1,
  GenderName: 'Nam',
  Address: '',
  MobileNumber: '',
  Email: '',
  GroupId: '',
  GroupName: '',
  Amount: 0,
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

async function handleSubmitForm() {
  loading.value = true
  customer.value.GenderName =
    customer.value.Gender === 0 ? 'Nữ' : customer.value.Gender === 1 ? 'Nam' : 'Không rõ'

  if (props.id) {
    const response = await store.dispatch('updateCustomer', {
      id: props.id,
      customer: customer.value,
      token: token.value,
    })
    if (response.success) handleCloseForm()
    else error.value = response.message
  } else {
    const response = await store.dispatch('createCustomer', {
      customer: customer.value,
      token: token.value,
    })
    if (response.success) handleCloseForm()
    else error.value = response.message
  }
  loading.value = false
}

async function fetchCustomerData() {
  if (props.id) {
    await store.dispatch('fetchCustomer', props.id)
    const empStore = store.getters.getCustomer
    customer.value = empStore
    customer.value.DateOfBirth = formatDateForm(customer.value.DateOfBirth)
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

const customerGroups = computed(() => store.getters.getCustomerGroups)
const token = computed(() => store.getters.getAccessToken)
onMounted(() => {
  fetchCustomerData()
  store.dispatch('fetchCustomerGroups')
})
</script>
