<template>
  <div class="toast-container">
    <div v-for="toast in toasts" :key="toast.id" class="toast" :class="toast.type">
      {{ toast.message }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onUnmounted } from 'vue'

export interface Toast {
  id: number
  message: string
  type?: 'error' | 'success' | 'info'
}

const toasts = ref<Toast[]>([])
let toastId = 0

function addToastError(message: string, type: Toast['type'] = 'error') {
  const id = toastId++
  toasts.value.push({ id, message, type })

  setTimeout(() => {
    toasts.value = toasts.value.filter((toast) => toast.id !== id)
  }, 3000)
}

function addToastSuccess(message: string, type: Toast['type'] = 'success') {
  const id = toastId++
  toasts.value.push({ id, message, type })

  setTimeout(() => {
    toasts.value = toasts.value.filter((toast) => toast.id !== id)
  }, 3000)
}

function addToastInfo(message: string, type: Toast['type'] = 'info') {
  const id = toastId++
  toasts.value.push({ id, message, type })

  setTimeout(() => {
    toasts.value = toasts.value.filter((toast) => toast.id !== id)
  }, 3000)
}

const removeToast = (id: number) => {
  toasts.value = toasts.value.filter((toast) => toast.id !== id)
}

defineExpose({
  addToastError,
  addToastSuccess,
  addToastInfo,
  removeToast,
})

onUnmounted(() => {
  toasts.value = []
})
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 60px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 1000;
}

.toast {
  padding: 10px 20px;
  margin-bottom: 10px;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
  animation: slideIn 0.3s ease-out;
  color: white;
}

.toast.error {
  background-color: #f44336;
}

.toast.success {
  background-color: #4caf50;
}

.toast.info {
  background-color: #2196f3;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-100%);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
