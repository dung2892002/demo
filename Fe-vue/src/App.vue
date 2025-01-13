<template>
  <TheHeader></TheHeader>
  <div class="container">
    <RouterView />
  </div>
</template>

<script setup lang="ts">
import TheHeader from './components/TheHeader.vue'
import Cookies from 'js-cookie'
import { useStore } from 'vuex'
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const store = useStore()
const router = useRouter()
if (window.location.pathname === '/') {
  const params = new URLSearchParams(window.location.search)

  const accessToken = params.get('access_token')
  const refreshToken = params.get('refresh_token')
  const username = params.get('username')
  const expiration = params.get('expiration')

  if (accessToken && refreshToken && username && expiration) {
    localStorage.setItem('accessToken', accessToken)
    localStorage.setItem('username', username)
    localStorage.setItem('expirationTime', expiration)

    store.commit('setAccessToken', accessToken)
    store.commit('setUsername', username)

    Cookies.set('refreshToken', refreshToken, {
      expires: 7,
      secure: true,
      sameSite: 'Strict',
    })

    router.push({ name: 'home' })
  }
}

onMounted(() => {
  store.dispatch('refreshToken')

  setInterval(() => {
    store.dispatch('refreshToken')
  }, 60000)
})
</script>
