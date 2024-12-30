<template>
  <TheHeader></TheHeader>
  <div class="container">
    <RouterView />
  </div>
</template>

<script setup lang="ts">
import TheHeader from './components/TheHeader.vue'
import { useStore } from 'vuex'
import { onMounted } from 'vue'

const store = useStore()

onMounted(() => {
  store.dispatch('refreshToken')

  setInterval(() => {
    store.dispatch('refreshToken')
  }, 60000)
})
</script>
