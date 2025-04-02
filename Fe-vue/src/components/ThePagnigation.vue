<template>
  <div class="pagination" v-loading="pageLoading">
    <div class="pagination-section">
      <span>Tổng: {{ totalRecords }}</span>
    </div>
    <div class="pagination-section">
      <div class="pagesize" style="align-items: center">
        <span>Số dòng/trang: </span>
        <div class="pagesize">
          <div class="current-pagesize" @click="togglePagesize">
            <span>{{ pageSize }}</span>

            <font-awesome-icon :icon="['fas', 'chevron-down']" v-if="showSelectPageSize" />
            <font-awesome-icon :icon="['fas', 'chevron-up']" v-else />
          </div>
          <div class="pagesize-select" v-if="showSelectPageSize">
            <div
              v-for="(value, index) in pageSizes"
              :key="index"
              class="pagesize-data"
              :class="{ selected: value === pageSize }"
              @click="changePageSize(value)"
            >
              {{ value }}
              <font-awesome-icon
                :icon="['fas', 'check']"
                style="color: #078cf8; width: 20px; height: 20px"
                v-if="value === pageSize"
              />
            </div>
          </div>
        </div>
      </div>
      <div class="pagenumber">
        <button
          id="prev-page"
          class="button--prev"
          @click="previousPage"
          :class="{ disable: pageNumber === 1 }"
        >
          <img src="/src/assets/icon/btn-prev-page.svg" alt="logo" />
        </button>
        <div class="current-page">
          {{ props.pageNumber }}
        </div>

        <button
          id="next-page"
          class="button--next"
          @click="nextPage"
          :class="{ disable: pageNumber === totalPages }"
        >
          <img src="/src/assets/icon/btn-next-page.svg" alt="logo" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import '../styles/layout/pagination.scss'
import { computed, ref } from 'vue'
import { useStore } from 'vuex'

const pageSize = ref(10)

const pageSizes = [10, 20, 30, 50, 100]

const store = useStore()

const showSelectPageSize = ref(false)

const totalRecords = computed(() => store.getters.getTotalRecords)
const totalPages = computed(() => store.getters.getTotalPages)

const emits = defineEmits(['pageSizeChange', 'pageChange'])

function togglePagesize() {
  showSelectPageSize.value = !showSelectPageSize.value
}

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

function changePageSize(value: number) {
  pageSize.value = value
  showSelectPageSize.value = false
  emits('pageSizeChange', value)
}
</script>
