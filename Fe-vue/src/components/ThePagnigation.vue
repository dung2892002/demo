<template>
  <div class="pagination" v-loading="pageLoading">
    <div class="pagination-section">
      <span>Tổng: {{ totalRecords }}</span>
    </div>
    <div class="pagination-section">
      <div class="content--row" style="align-items: center">
        <label for="page-size">Số bản ghi/trang: </label>
        <select v-model="pageSize" id="page-size" @change="emitPageSizeChange">
          <option v-for="size in pageSizes" :key="size" :value="size">
            {{ size }}
          </option>
        </select>
      </div>
      <div class="control_page">
        <button id="prev-page" class="button--prev" @click="previousPage">
          <img src="/src/assets/icon/btn-prev-page.svg" alt="logo" />
        </button>
        <div class="current-page">
          {{ props.pageNumber }}
        </div>
        <button id="next-page" class="button--next" @click="nextPage">
          <img src="/src/assets/icon/btn-next-page.svg" alt="logo" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import '../styles/layout/pagination.css'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

const pageSize = ref(10)

const pageSizes = [10, 20, 30, 50, 100]

const store = useStore()

const totalRecords = computed(() => store.getters.getTotalRecords)
const totalPages = computed(() => store.getters.getTotalPages)

const emits = defineEmits(['pageSizeChange', 'pageChange'])

const props = defineProps({
  pageNumber: {
    type: Number,
    required: true,
  },
  pageLoading: {
    type: Boolean,
    required: true,
  },
})

function nextPage() {
  if (props.pageNumber < totalPages.value) {
    emits('pageChange', props.pageNumber + 1)
  }
}

function previousPage() {
  if (props.pageNumber > 1) {
    emits('pageChange', props.pageNumber - 1)
  }
}

function emitPageSizeChange() {
  emits('pageSizeChange', pageSize.value)
}
</script>
