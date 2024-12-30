<template>
  <div class="header">
    <div class="header__left">
      <button class="header__toggle">
        <img src="../assets/img/toggle.png" alt="toggle" class="header__toggle-img" />
      </button>
      <div class="header__logo" @click="goHomePage">
        <img src="../assets/img/cukcuk-logo.png" alt="toggle" class="header__logo-img" />
      </div>
    </div>
    <div class="header__right" v-if="username">
      <select id="locations" name="locations" class="header__branch-select">
        <option value="Ha noi">Chi nhánh Hà Nội</option>
        <option value="Da Nang">Chi nhánh Đà Nẵng</option>
        <option value="Ho Chi Minh">Chi nhánh TP.Hồ Chí Minh</option>
      </select>
      <div class="header__account">
        <img
          src="../assets/icon/avatar-default.png"
          alt="account avatar"
          class="header__account-avatar"
        />
        <span class="header__account-name">{{ username }}</span>
        <button class="header__account-option" @click="logout" v-loading="logoutLoading">
          Logout
        </button>
      </div>
    </div>
    <div class="header__right" v-else>
      <div></div>
      <RouterLink :to="{ name: 'login' }">Đăng nhập</RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useStore } from 'vuex'
import '../styles/layout/header.css'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

const store = useStore()
const logoutLoading = ref(false)
const router = useRouter()

const username = computed(() => store.getters.getUsername)
const token = computed(() => store.getters.getAccessToken)

function goHomePage() {
  router.push({
    name: 'home',
  })
}

async function logout() {
  logoutLoading.value = true
  const request = {
    username: username.value,
    token: token.value,
  }
  await store.dispatch('logout', request)
  logoutLoading.value = false
  router.push({
    name: 'login',
  })
}
</script>
