<template>
  <div class="content__filter__item">
    <span
      v-for="group in groups"
      :key="group.GroupId"
      @click="selectDepartment(group.GroupId)"
      :class="{ selected: group.GroupId === selectId }"
    >
      {{ group.GroupName }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const emit = defineEmits(['fetchData'])

const selectId = ref<string | null>(null)

function selectDepartment(groupId: string) {
  selectId.value = groupId
  emit('fetchData', groupId, 'group')
}

const groups = computed(() => store.getters.getCustomerGroups)

onMounted(() => {
  store.dispatch('fetchCustomerGroups')
})
</script>
