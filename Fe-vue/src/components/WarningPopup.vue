<template>
  <div class="form-container">
    <div class="form__content">
      <div class="form__header">
        <h2 class="form__title">{{ title }}</h2>
        <button class="form__button" @click="closePopup">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <form class="cukcuk-form" id="form">
        <div class="form-group">
          <div class="form__item">
            <span>
              {{ content }}
            </span>
          </div>
        </div>
      </form>
      <div class="form__footer">
        <button class="button--complete" id="submitButton" @click="closePopup">
          <span src="/src/assets/icon/refresh.png" alt="logo">Xác nhận</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  title: {
    type: String,
    required: true,
  },
  content: {
    type: String,
    required: true,
  },
});

const emit = defineEmits(['close']);

function closePopup() {
  emit('close');
}
</script>
