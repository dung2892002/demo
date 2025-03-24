<template>
  <div class="form__select-filetype">
    <button :class="{ selected: typeUpload === 1 }" @click="selectTypeUpload(1)">
      Tải lên tệp
    </button>
    <button :class="{ selected: typeUpload === 2 }" @click="selectTypeUpload(2)">Liên kết</button>
    <button :class="{ selected: typeUpload === 3 }" @click="selectTypeUpload(3)">Nội dung</button>
  </div>
</template>

<script setup lang="ts">
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  typeUpload: {
    type: Number,
    required: true,
  },
})

const emits = defineEmits(['selectTypeUpload'])

function selectTypeUpload(type: number) {
  emits('selectTypeUpload', type)
}
</script>
