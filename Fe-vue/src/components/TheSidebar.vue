<template>
  <div class="sidebar" id="sidebar" :class="{ 'sidebar--less': isLess }">
    <div class="sidebar__buttons">
      <button class="sidebar__button" data-text="Trang chủ" @click="goHomePage">
        <img src="../assets/icon/dashboard.png" alt="logo" class="sidebar__button-logo" />
        <span class="sidebar__button-text">Trang chủ</span>
      </button>
      <button
        v-for="menu in menus"
        :key="menu.Id"
        class="sidebar__button"
        :data-text="menu.MenuName"
        @click="handleSelectMenu(menu)"
      >
        <font-awesome-icon :icon="menu.MenuIcon" />
        <span class="sidebar__button-text">{{ menu.MenuName }}</span>
      </button>
      <button class="sidebar__button" data-text="Cài đặt" @click="goSettingPage">
        <font-awesome-icon icon="fa-solid fa-gear" />
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
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'
import type { Menu } from '@/entities/Menu'

const store = useStore()

const router = useRouter()
const isLess = ref(false)
function toggleSidebar() {
  isLess.value = !isLess.value
}

function handleSelectMenu(menu: Menu) {
  router.push(`${menu.MenuPath}`)
}

const menus = computed(() => store.getters.getMenus)

function goHomePage() {
  router.push({
    name: 'home',
  })
}

function goSettingPage() {
  router.push({
    name: 'setting-excel',
  })
}

onMounted(() => {
  store.dispatch('fetchMenus')
})
</script>
