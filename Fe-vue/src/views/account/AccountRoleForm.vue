<template>
  <div class="form-container">
    <div class="form__content" v-draggable>
      <div class="form__header">
        <h2 class="form__title">Chỉnh sửa quyền</h2>
        <button class="form__button" @click="cancel">
          <img src="/src/assets/icon/close-48.png" alt="logo" />
        </button>
      </div>
      <span class="error-message" v-if="error">{{ error }}</span>
      <form class="cukcuk-form">
        <div class="form-group">
          <div class="form__item">
            <h3>Các role của account</h3>
            <div v-for="(role, index) in accountRoles" :key="index" class="multi-choice">
              <span>
                {{ role }}
              </span>
              <img src="../../assets/icon/close-48.png" alt="" @click="handleUpdateRole(role)" />
            </div>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item">
            <h3>Các role hệ thống</h3>
            <span v-for="(role, index) in roles" :key="index">
              <div v-if="checkRole(role.Name) == -1" class="multi-choice">
                <span>{{ role.Name }}</span>
                <img
                  src="../../assets/icon/success-48.png"
                  alt=""
                  @click="handleUpdateRole(role.Name)"
                />
              </div>
            </span>
          </div>
        </div>
        <div>
          <button @click.prevent="submitForm" class="button--add">Lưu</button>
          <button @click.prevent="cancel" class="button--remove">Hủy</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, toRaw } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const updatedRoles = ref<string[]>([])
const error = ref<string | boolean>(false)

const props = defineProps({
  id: {
    type: String,
    required: true,
  },
})

const emits = defineEmits(['close', 'stopLoading'])

const roles = computed(() => store.getters.getRoles)
const token = computed(() => store.getters.getAccessToken)
const accountRoles = computed(() => store.getters.getAccountRoles)

async function submitForm() {
  const result = await store.dispatch('updateRolesAccount', {
    userId: props.id,
    roleNames: updatedRoles.value,
    token: token.value,
  })

  if (result) {
    emits('close')
    error.value = false
  } else {
    error.value = 'Có lỗi xảy ra'
  }
}

function cancel() {
  emits('close')
}

async function fetchAccountRoles() {
  await store.dispatch('fetchAccountRoles', { accountId: props.id, token: token.value })
  updatedRoles.value = toRaw(accountRoles.value)
}

function checkRole(roleName) {
  const index = updatedRoles.value.indexOf(roleName)
  return index
}

function handleUpdateRole(roleName) {
  const index = checkRole(roleName)
  if (index !== -1) {
    updatedRoles.value.splice(index, 1)
  } else {
    updatedRoles.value.push(roleName)
  }
}

onMounted(() => {
  store.dispatch('fetchRoles', token.value)
  fetchAccountRoles()

  emits('stopLoading')
})
</script>
