<template>
  <div class="context-menu" :style="{ top: position.top + 'px', left: position.left + 'px' }">
    <span
      v-for="(action, index) in props.actions"
      :key="index"
      @click="emitAction(action)"
      :class="{disable: action.state === false}"
    >
      {{ action.label }}
    </span>
  </div>
  <!-- <div class="overlay" @click="emitClose" @contextmenu.prevent="emitClose"></div> -->
</template>

<script setup lang="ts">
import type { ActionMenu } from '@/entities/ActionMenu'

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


function emitAction(action: ActionMenu) {
  if (action.state === false) return
  emit('actionClick', action)
}
</script>
