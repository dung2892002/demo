<template>
  <div class="context-menu" :style="{ top: position.top + 'px', left: position.left + 'px' }">
    <span
      v-for="(action, index) in props.actions"
      :key="index"
      @click="emitAction(action, index)"
      v-loading="loading == index"
    >
      {{ action.label }}
    </span>
  </div>
  <!-- <div class="overlay" @click="emitClose" @contextmenu.prevent="emitClose"></div> -->
</template>

<script setup lang="ts">
import type { ActionMenu } from '@/entities/ActionMenu'
import { ref } from 'vue'

const props = defineProps({
  actions: {
    type: Array<ActionMenu>,
    required: true,
  },
  position: {
    type: Object,
    required: true,
  },
})
const emit = defineEmits(['actionClick', 'close'])
const loading = ref(-1)


function emitAction(action: ActionMenu, index: number) {
  loading.value = index
  emit('actionClick', action)
}
</script>
