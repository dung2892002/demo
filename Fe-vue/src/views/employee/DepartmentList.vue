<template>
  <div class="content__filter__item">
    <span
      v-for="department in departments"
      :key="department.DepartmentId"
      @click="selectDepartment(department.DepartmentId)"
      :class="{ selected: department.DepartmentId === selectId }"
    >
      {{ department.DepartmentName }}
    </span>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()

const emit = defineEmits(['fetchData'])
const selectId = ref<string | null>(null)

function selectDepartment(departmentId: string) {
  selectId.value = departmentId
  emit('fetchData', departmentId, 'department')
}

const departments = computed(() => store.getters.getDepartments)

onMounted(() => {
  store.dispatch('fetchDepartments')
})
</script>
