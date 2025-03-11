<template>
  <div class="import">
    <div class="import-step">Bước {{ stepStatus + 1 }}: {{ steps[stepStatus] }}</div>
    <div class="import-main" style="width: 100%; background-color:">
      <div class="import-step">
        <span
          class="step"
          v-for="(step, index) in steps"
          :key="index"
          :class="{ '--selected': stepStatus == index }"
          >{{ index + 1 }}. {{ step }}</span
        >
      </div>
      <div class="import-content">
        <div v-if="stepStatus === 0" class="step--1">
          <span>Chọn dữ liệu đã chuẩn bị để nhập vào phần mềm</span>
          <div class="content--row">
            <input type="file" ref="fileInput" style="display: none" @change="handleFileUpload" />
            <span>{{ uploadedFile?.name }}</span>
            <button @click="triggerFileInput" style="padding: 8px 24px">Chọn</button>
          </div>
          <div v-if="error" class="error-message">{{ error }}</div>
          <span
            >Chưa có tệp mẫu để chuẩn bị dữ liệu? Tải tệp excel mẫu mà phần mềm cung cấp để chuẩn bị
            dữ liệu nhập <a href="">Tại đây</a></span
          >
        </div>
        <div v-if="stepStatus === 1" class="step--2">
          <div>
            <span>{{ totalValid }} dòng hợp lệ</span>
            <span>{{ totalInvalid }} dòng không hợp lệ</span>
          </div>
          <span class="error-message" v-if="error">{{ error }}</span>
          <div class="main-container">
            <table class="employee-table">
              <thead>
                <tr>
                  <th class="w-6">STT</th>
                  <th class="w-10">
                    {{ objectImport === 'Employees' ? 'Mã NV' : 'Mã KH' }}
                  </th>
                  <th class="w-14">Họ và tên</th>
                  <th class="w-12">Ngày sinh</th>
                  <th class="w-12">Số điện thoại</th>
                  <th class="w-20">Địa chỉ</th>
                  <th>Tình trạng</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(data, index) in datas" :key="index">
                  <td>{{ index + 1 }}</td>
                  <td>
                    {{ objectImport === 'Employees' ? data.EmployeeCode : data.CustomerCode }}
                  </td>
                  <td>{{ data.Fullname }}</td>
                  <td>{{ formatDate(data.DateOfBirth) }}</td>
                  <td>{{ data.MobileNumber }}</td>
                  <td>{{ data.Address }}</td>
                  <td>
                    <span v-if="data.Status">Hợp lệ</span>

                    <div class="error-message" v-for="(error, index) in data.Errors" :key="index">
                      - {{ error }}
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div>
            Tải về tập tin chứa các dòng nhập khẩu không hợp lệ
            <span @click="downloadFileImport(0)" style="cursor: pointer">Tại đây</span>
          </div>
        </div>
        <div v-if="stepStatus === 2" class="step--3">
          <span class="label">Kết quả nhập khẩu</span>
          <div class="link">
            Tải về tập tin chứa kết quả nhập khẩu
            <span @click="downloadFileImport(1)" style="cursor: pointer">Tại đây</span>
          </div>
          <div class="result">
            <span>+ Số dòng nhập khẩu thành công: {{ totalValid }}</span>
            <span>+ Số dòng nhập khẩu không thành công: {{ totalInvalid }}</span>
          </div>
        </div>
      </div>
    </div>
    <div class="footer">
      <button class="footer-button">Giúp</button>
      <div v-if="stepStatus != 2">
        <button class="footer-button" :disabled="stepStatus === 0" @click="handlePreStep">
          Quay lại
        </button>
        <button
          class="footer-button"
          :disabled="stepStatus === 2"
          @click="handleNextStep"
          v-loading="loading"
        >
          Tiếp tục
        </button>
        <button class="footer-button" @click="closeImport">Hủy bỏ</button>
      </div>
      <div v-else>
        <button class="footer-button" @click="closeImport">Đóng</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

import '../styles/layout/table.scss'
import { useRoute, useRouter } from 'vue-router'

const steps = ref(['Chọn tệp nguồn', 'Kiểm tra dữ liệu', 'Kết quả nhập khẩu'])

const stepStatus = ref(0)
const error = ref<string | null>(null)
const uploadedFile = ref<File | null>(null)
const fileInput = ref<HTMLInputElement | null>(null)
const loading = ref(false)
const store = useStore()

const router = useRouter()
const route = useRoute()

const objectImport = route.params.data.toString()

function closeImport() {
  router.push({
    name: objectImport.toLowerCase(),
  })
}

function triggerFileInput() {
  fileInput.value?.click()
}

function handleFileUpload(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files ? target.files[0] : null
  if (file) {
    if (file.name.endsWith('.xlsx')) {
      uploadedFile.value = file
      error.value = null
    } else {
      error.value = 'Chỉ hỗ trợ file có định dạng .xlsx'
    }
  }
}

function handlePreStep() {
  stepStatus.value--
  error.value = null
}

async function handleNextStep() {
  if (stepStatus.value == 0) {
    if (!uploadedFile.value) {
      error.value = 'Chưa chọn file'
    } else {
      loading.value = true
      const response = await importFile()
      loading.value = false
      if (response.success) {
        stepStatus.value++
        error.value = null
      } else {
        error.value = response.message
      }
    }
  } else {
    if (totalValid.value > 0) {
      loading.value = true
      const cacheId = localStorage.getItem('ValidDataCacheIdImport')
      const response = await store.dispatch('addDataImport', {
        cacheId: cacheId,
        token: token.value,
        object: objectImport,
      })
      loading.value = false
      if (response.success) {
        stepStatus.value++
        error.value = null
      } else {
        error.value = response.message
      }
    } else {
      error.value = 'Không có dòng nào hợp lệ'
    }
  }
}

async function downloadFileImport(state: number) {
  const validDataCacheId = state != 0 ? localStorage.getItem('ValidDataCacheIdImport') : null
  const invalidDataCacheId = localStorage.getItem('InvalidDataCacheIdImport')
  console.log(validDataCacheId)
  await store.dispatch('downloadFileImportData', {
    validCacheId: validDataCacheId,
    invalidCacheId: invalidDataCacheId,
    token: token.value,
    object: objectImport,
  })
}

async function importFile() {
  if (!uploadedFile.value) return
  const formData = new FormData()
  formData.append('file', uploadedFile.value)

  const response = await store.dispatch('importExcel', {
    formData: formData,
    token: token.value,
    object: objectImport,
  })

  return response
}

const token = computed(() => store.getters.getAccessToken)
const datas = computed(() => store.getters.getDatasImport)
const totalInvalid = computed(() => store.getters.getTotalInvalid)
const totalValid = computed(() => store.getters.getTotalValid)

function formatDate(inputDate: string | null) {
  if (inputDate == null) return ''
  const date = new Date(inputDate)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}
</script>
