<template>
  <div class="toast">
    <img src="../assets/icon/error-48.png" alt="toast" v-if="props.type === 0" />
    <img src="../assets/icon/success-48.png" alt="toast" v-if="props.type === 1" />
    <img src="../assets/icon/warning-48.png" alt="toast" v-if="props.type === 2" />

    <div
      class="message"
      :class="{
        '--error': props.type === 0,
        '--success': props.type === 1,
        '--warning': props.type === 2,
      }"
    >
      {{ props.message }}
    </div>
    <img
      src="../assets/icon/delete-48.png"
      alt="close-toast"
      @click="handleDelete"
      class="action"
    />
  </div>
</template>

<script setup lang="ts">
import '../styles/component/toast.scss'

const emits = defineEmits(['delete'])

function handleDelete() {
  emits('delete', props.index)
}

const props = defineProps({
  type: {
    type: Number,
    required: true,
  },
  message: {
    type: String,
    required: true,
  },
  index: {
    type: Number,
    required: true,
  },
})
</script>
