<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Quản lý phân quyền</h1>
    </div>
    <div class="content-main">
      <div class="toolbar">
        <div class="toolbar_search"></div>
        <div class="toolbar__actions"></div>
      </div>
      <div class="main-container" ref="tableContainer">
        <table class="employee-table">
          <thead>
            <tr>
              <th class="w-6">STT</th>
              <th class="w-50">UserName</th>
              <th>Email</th>
              <th class="w-10">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(account, index) in accounts" :key="account.EmployeeId">
              <td>{{ index + 1 }}</td>
              <td>{{ account.UserName }}</td>
              <td>{{ account.Email }}</td>
              <td>
                <div class="action" :ref="`action-${index}`">
                  <div class="action-buttons">
                    <button class="action-button" @click="togglePopupAction(index, $event)">
                      <img src="../../assets/icon/option.png" alt="" />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span
                        @click="updateAccountPermissions(account.Id, index)"
                        v-loading="updateLoading == index"
                        >Sửa quyền</span
                      >
                      <span
                        @click="updateAccounts(account.Id, index)"
                        v-loading="updateLoading == index"
                        >Sửa role
                      </span>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <AccountRoleForm
      :id="accountUpdateId"
      @close="closeForm"
      @stop-loading="stopLoading"
      v-if="showForm"
    />
    <AccountPermissionForm
      :id="accountUpdateId"
      @close="closeForm"
      @stop-loading="stopLoading"
      v-if="showFormPermission"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useStore } from 'vuex'
import AccountRoleForm from './AccountRoleForm.vue'
import AccountPermissionForm from './AccountPermissionForm.vue'

const store = useStore()

const showPopupAction = ref<number>(-1)
const showMenu = ref(false)
const updateLoading = ref(-1)
const accountUpdateId = ref<string>('')
const showForm = ref(false)
const showFormPermission = ref(false)
const popupPosition = ref({
  top: 0,
  right: 100,
})

async function fetchAccoutns() {
  await store.dispatch('fetchAccounts', token.value)
}

// let activeButton: HTMLElement | null = null

function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.target instanceof HTMLElement ? event.target : null
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - 2,
    right: window.innerWidth - buttonRect.left + 10,
  }

  // activeButton = target

  showPopupAction.value = showPopupAction.value === index ? -1 : index
  showMenu.value = false
}

function updateAccountPermissions(id: string, index: number) {
  updateLoading.value = index
  showFormPermission.value = true
  accountUpdateId.value = id
}

function updateAccounts(id: string, index: number) {
  updateLoading.value = index
  showForm.value = true
  accountUpdateId.value = id
}

function closeForm() {
  showForm.value = false
  showFormPermission.value = false
}

function stopLoading() {
  updateLoading.value = -1
  showPopupAction.value = -1
}

const token = computed(() => store.getters.getAccessToken)
const accounts = computed(() => store.getters.getAccounts)
onMounted(() => {
  fetchAccoutns()
})
</script>
