<template>
  <div class="header">
    <div class="header__left">
      <button class="header__toggle">
        <img src="../assets/img/toggle.png" alt="toggle" class="header__toggle-img" />
      </button>
      <div class="header__logo" @click="goHomePage">
        <img src="../assets/img/logo.svg" alt="logo app" class="header__logo-img" />
      </div>
    </div>
    <div class="header__right" v-if="username">
      <div></div>
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
import '../styles/layout/header.scss'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { connection } from '@/ts/signalR'
import * as signalR from '@microsoft/signalr'
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
  if (connection.state === signalR.HubConnectionState.Connected) {
    await connection.stop()
    console.log('Disconnected from SignalR.')
  }
  logoutLoading.value = false
  router.push({
    name: 'login',
  })
}
</script>
