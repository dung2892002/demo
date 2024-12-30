<template>
  <div class="sidebar" id="sidebar" :class="{ 'sidebar--less': isLess }">
    <div class="sidebar__buttons">
      <button class="sidebar__button" data-text="Trang chủ" @click="goHomePage">
        <img src="../assets/icon/dashboard.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text">Trang chủ</span>
      </button>
      <button class="sidebar__button" data-text="Báo cáo">
        <img src="../assets/icon/report.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text">Báo cáo</span>
      </button>
      <button class="sidebar__button" data-text="Nhân viên" @click="goEmployeePage">
        <img src="../assets/icon/dic-employee.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text">Nhân viên</span>
      </button>
      <button class="sidebar__button" data-text="Khách hàng" @click="goCustomerPage">
        <img src="../assets/icon/dic-employee.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text">Khách hàng</span>
      </button>
      <button class="sidebar__button" data-text="Cài đặt" @click="goSettingPage">
        <img src="../assets/icon/setting.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text" data-text="Cài đặt">Cài đặt</span>
      </button>
    </div>
    <div>
      <div class="sidebar__separator"></div>
      <button
        class="sidebar__button sidebar__button--toggle"
        id="toggleButton"
        data-text="Phóng to"
        @click="toggleSidebar"
      >
        <img
          src="../assets/icon/btn-prev-page.svg"
          alt="logo"
          class="sidebar__button-logo"
          v-if="!isLess"
        />
        <img
          src="../assets/icon/btn-next-page.svg"
          alt="logo"
          class="sidebar__button-logo"
          v-else
        />
        <span class="sidebar__button-text" @click="toggleSidebar">Thu gọn</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import '../styles/layout/sidebar.css'
import { ref } from 'vue'

const router = useRouter()
const isLess = ref(false)
function toggleSidebar() {
  isLess.value = !isLess.value
}

function goEmployeePage() {
  router.push({
    name: 'employees',
  })
}

function goCustomerPage() {
  router.push({
    name: 'customers',
  })
}

function goHomePage() {
  router.push({
    name: 'home',
  })
}

function goSettingPage() {
  router.push({
    name: 'setting',
  })
}
</script>
