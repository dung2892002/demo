<template>
  <div class="content">
    <div class="content__header">
      <h1 class="content__title">Quản lý tài khoản</h1>
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
            <tr
              v-for="(account, index) in accounts"
              :key="account.Id"
              @contextmenu="showContextMenu($event, account.Id, index)"
            >
              <td>{{ index + 1 }}</td>
              <td>{{ account.UserName }}</td>
              <td>{{ account.Email }}</td>
              <td>
                <div class="action" :ref="`action-${index}`">
                  <div class="action-buttons">
                    <button class="action-button" @click="togglePopupAction(index, $event)">
                      <font-awesome-icon
                        :icon="['fas', 'ellipsis-vertical']"
                        class="button__icon"
                        :class="{ selected: showPopupAction === index }"
                      />
                    </button>
                    <div
                      class="popup-action"
                      v-if="showPopupAction == index"
                      :style="{ top: popupPosition.top + 'px', right: popupPosition.right + 'px' }"
                    >
                      <span
                        @click="updateAccountPermissions(account.Id, index)"
                        v-loading="updatePermissionLoading == index"
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
    <ContextMenu
      v-if="showMenu"
      :actions="contextMenuActions"
      @actionClick="handleActionClick"
      @close="closeContextMenu"
      :position="menuPosition"
    ></ContextMenu>
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
import type { ActionMenu } from '@/entities/ActionMenu'
import ContextMenu from '@/components/ContextMenu.vue'

const store = useStore()

const showPopupAction = ref<number>(-1)
const showMenu = ref(false)
const updateLoading = ref(-1)
const updatePermissionLoading = ref(-1)
const accountUpdateId = ref<string>('')
const showForm = ref(false)
const showFormPermission = ref(false)

const targetId = ref<string>('')
const targetIndex = ref(-1)

const contextMenuActions = ref<ActionMenu[]>([
  { label: 'Sửa quyền', action: 'updatPermission' },
  { label: 'Sửa role', action: 'updateRole' },
])

const menuPosition = ref({
  top: 0,
  left: 0,
})

function handleActionClick(action: ActionMenu) {
  if (action.action === 'updateRole') {
    updateAccounts(targetId.value, targetIndex.value)
  } else {
    updateAccountPermissions(targetId.value, targetIndex.value)
  }
  showMenu.value = false
}

function showContextMenu(event: MouseEvent, Id: string, index: number) {
  event.preventDefault()
  menuPosition.value.top = event.clientY
  menuPosition.value.left = event.clientX
  showMenu.value = true
  showPopupAction.value = -1
  targetId.value = Id
  targetIndex.value = index
}

function closeContextMenu() {
  showMenu.value = false
}

const popupPosition = ref({
  top: 0,
  right: 100,
})

async function fetchAccoutns() {
  await store.dispatch('fetchAccounts', token.value)
}

function togglePopupAction(index: number, event: MouseEvent): void {
  const target = event.currentTarget as HTMLElement
  if (!target) return

  const buttonRect = target.getBoundingClientRect()

  popupPosition.value = {
    top: buttonRect.top - buttonRect.height,
    right: window.innerWidth - buttonRect.left,
  }

  showPopupAction.value = showPopupAction.value === index ? -1 : index
}

function updateAccountPermissions(id: string, index: number) {
  updatePermissionLoading.value = index
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
  showMenu.value = false
}

function stopLoading() {
  updateLoading.value = -1
  updatePermissionLoading.value = -1
  showPopupAction.value = -1
}

const token = computed(() => store.getters.getAccessToken)
const accounts = computed(() => store.getters.getAccounts)
onMounted(() => {
  fetchAccoutns()
})
</script>
