<template>
  <div class="form-container">
    <div class="form__content" style="margin: 10% 35%; max-width: 380px">
      <span class="error-message" v-if="error">{{ error }}</span>
      <form class="hospital-form" style="max-width: 360px">
        <div class="form-group">
          <div class="form__item">
            <h3>Các quyền của tài khoản</h3>
            <div v-for="(permission, index) in accountPermissions" :key="index">
              <span>
                {{ permission.Permission.PermissionName }}
              </span>
              <img
                src="../../assets/icon/close-48.png"
                alt=""
                @click="handleDeletePermission(permission)"
              />
            </div>
          </div>
        </div>
        <div class="form-group">
          <div class="form__item">
            <h3>Các quyền của hệ thống</h3>
            <div v-for="(permission, index) in filteredPermissions" :key="index">
              <span>{{ permission.PermissionName }}</span>
              <img
                src="../../assets/icon/success-48.png"
                alt=""
                @click="handleAddPermission(permission)"
              />
            </div>
          </div>
        </div>
        <div>
          <button @click.prevent="cancel" class="button--add">Đóng</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const error = ref<string | boolean>(false)

async function handleAddPermission(permission) {
  const permissionAdd = {
    UserId: props.id,
    PermissionId: permission.PermissionId,
  }

  const result = await store.dispatch('addPermissionToUser', {
    permission: permissionAdd,
    token: token.value,
  })

  if (!result) error.value = 'co loi xay ra'
  else fetchAccountPermissions()
}

async function handleDeletePermission(permission) {
  const result = await store.dispatch('deletePermissionToUser', {
    permission: permission,
    token: token.value,
  })

  if (!result) error.value = 'co loi xay ra'
  else fetchAccountPermissions()
}
const props = defineProps({
  id: {
    type: String,
    required: true,
  },
})

const emits = defineEmits(['close', 'stopLoading'])

function cancel() {
  emits('close')
}

async function fetchAccountPermissions() {
  await store.dispatch('fetchAccountPermissions', { accountId: props.id, token: token.value })
}

const token = computed(() => store.getters.getAccessToken)
const accountPermissions = computed(() => store.getters.getAccountPermissions)
const permissions = computed(() => store.getters.getPermissions)

const filteredPermissions = computed(() => {
  if (permissions.value && accountPermissions.value) {
    return permissions.value.filter(
      (permission) =>
        !accountPermissions.value.some(
          (accPerm) => accPerm.Permission.PermissionName === permission.PermissionName,
        ),
    )
  }
  return []
})

onMounted(() => {
  store.dispatch('fetchPermissions', {
    name: ' ',
    token: token.value,
  })
  fetchAccountPermissions()

  emits('stopLoading')
})
</script>
