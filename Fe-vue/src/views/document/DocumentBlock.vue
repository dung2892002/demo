<template>
  <div>
    <div v-for="(block, index) in props.blocks" :key="index">
      <span>{{ block.Content }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import type { PropType } from 'vue'

const props = defineProps({
  blocks: {
    type: Array as PropType<DocumentBlock[]>,
    required: true,
  },
})
</script>
