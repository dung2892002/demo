<template>
  <div style="width: 100%; padding: 20px">
    <h1>Cài đặt</h1>
    <div style="display: flex; gap: 10px">
      <button @click="goToExcelSetting">Import</button>
      <button @click="goToMenuSetting">Menu</button>
    </div>
    <RouterView />
  </div>
</template>
<script setup lang="ts">
import { RouterView, useRouter } from 'vue-router'

const router = useRouter()

function goToExcelSetting() {
  router.push({
    name: 'setting-excel',
  })
}

function goToMenuSetting() {
  router.push({
    name: 'setting-menu',
  })
}
</script>
