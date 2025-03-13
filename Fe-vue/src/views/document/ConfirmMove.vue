<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Xác nhận</h2>
        <button class="form__button" @click="closeForm(false)">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <span>
              Di chuyển tới thư mục mới
              <!-- <span style="font-weight: bold">{{ toDocumentId }}</span> -->
            </span>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--cancel" @click="closeForm(false)">Hủy bỏ</button>
        <button class="button--complete" id="submitButton" @click="submit">
          <span src="/src/assets/icon/refresh.png" alt="logo">Xác nhận</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  toDocumentId: {
    type: String,
    required: true,
  },
})

const emits = defineEmits(['close', 'submit'])

function closeForm(state: boolean) {
  emits('close', state)
}

function submit() {
  emits('submit')
}
</script>
