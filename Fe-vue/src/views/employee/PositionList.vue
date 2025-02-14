<template>
  <div class="content__filter__item">
    <span
      v-for="position in positions"
      :key="position.PositionId"
      @click="selectPosition(position.PositionId)"
      :class="{ selected: position.PositionId === selectId }"
    >
      {{ position.PositionName }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const emit = defineEmits(['fetchData'])
const selectId = ref<string | null>(null)

function selectPosition(positionId: string) {
  selectId.value = positionId
  emit('fetchData', positionId, 'position')
}

const positions = computed(() => store.getters.getPositions)

onMounted(() => {
  store.dispatch('fetchPositions')
})
</script>
