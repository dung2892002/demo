<template>
  <div class="content" v-if="menus">
    <div class="content-main">
      <button class="button--add" @click="addNew">Thêm</button>
      <draggable v-model="menus" @end="onDragEnd" item-key="Id">
        <template #item="{ element: menu }">
          <div @click="update(menu)" class="menu-value">
            <div>
              <span>
                <font-awesome-icon :icon="menu.MenuIcon" style="margin-right: 6px" />
                <span>{{ menu.MenuName }}</span>
              </span>
              <span>{{ menu.MenuOrder }}</span>
            </div>
            <button @click.stop="remove(menu)">
              <img src="../../assets/icon/close-48.png" alt="" />
            </button>
          </div>
        </template>
      </draggable>
      <div>
        <button class="button--add" @click="saveOrder" v-loading="saveOrderLoading">Lưu</button>
      </div>
    </div>
    <div v-if="showForm" class="form-container">
      <div class="form__content" style="margin: 20% 35%; max-width: 380px">
        <span class="error-message" v-if="error">{{ error }}</span>
        <form class="cukcuk-form" style="max-width: 360px">
          <div class="form-group">
            <div class="form__item">
              <span>Tiêu đề</span>
              <input type="text" v-model="menuValue.MenuName" />
            </div>
          </div>
          <div class="form-group">
            <div class="form__item">
              <span>Path</span>
              <input type="text" v-model="menuValue.MenuPath" />
            </div>
          </div>
          <div class="form-group">
            <div class="form__item">
              <span>Icon</span>
              <div class="custom-select">
                <div class="selected" @click="toggleSelectIcon">
                  <font-awesome-icon
                    :icon="menuValue.MenuIcon"
                    style="width: 24px; height: 24px"
                    v-if="menuValue.MenuIcon.length > 0"
                  />
                  <span>{{ menuValue.MenuIcon }}</span>
                </div>
                <div v-if="openSelectIcon" class="dropdown">
                  <div
                    v-for="(icon, index) in iconList"
                    :key="index"
                    class="dropdown-item"
                    @click="selectIcon(icon)"
                  >
                    <font-awesome-icon :icon="icon" style="width: 24px; height: 24px" />
                    <span>{{ icon }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="form-group">
            <div class="form__item">
              <span>Thứ tự</span>
              <input type="number" v-model="menuValue.MenuOrder" />
            </div>
          </div>
          <div>
            <button @click.prevent="submit" class="button--add" v-loading="saveFormLoading">
              Lưu
            </button>
            <button @click.prevent="cancel" class="button--remove">Hủy</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Menu } from '@/entities/Menu'
import draggable from 'vuedraggable'
import { onMounted, ref } from 'vue'
import { useStore } from 'vuex'
const store = useStore()
const showForm = ref(false)

const saveOrderLoading = ref(false)
const saveFormLoading = ref(false)

const menus = ref<Menu[] | null>(null)
const updateMenuId = ref<string | null>(null)
const error = ref<string | null>(null)
const openSelectIcon = ref(false)

const iconList = ref<string[]>([
  'fa-solid fa-user',
  'fa-solid fa-user-group',
  'fa-solid fa-comments',
  'fa-solid fa-chart-simple',
  'fa-solid fa-book',
])

const menuValue = ref<Menu>({
  Id: null,
  MenuName: '',
  MenuPath: '/',
  MenuIcon: '',
  MenuOrder: 0,
})

function onDragEnd() {
  menus.value?.forEach((menu, index) => {
    menu.MenuOrder = index + 1
  })
}

function toggleSelectIcon() {
  openSelectIcon.value = !openSelectIcon.value
}

function selectIcon(icon: string) {
  openSelectIcon.value = false
  menuValue.value.MenuIcon = icon
}

async function submit() {
  if (!validateData()) return
  saveFormLoading.value = true
  const response = await (updateMenuId.value === null
    ? store.dispatch('createMenu', menuValue.value)
    : store.dispatch('updateMenu', menuValue.value))

  saveFormLoading.value = false
  if (response) {
    cancel()
  }
}

function validateData(): boolean {
  menuValue.value.MenuName = menuValue.value.MenuName.trim()
  menuValue.value.MenuPath = menuValue.value.MenuPath.trim()
  if (menuValue.value.MenuName.length === 0) {
    error.value = 'Tiêu đề không được để trống'
    return false
  }
  if (menuValue.value.MenuPath.length === 0) {
    error.value = 'Path không được để trống'
    return false
  }
  if (menuValue.value.MenuPath.charAt(0) != '/') {
    error.value = 'Path phải được bắt đầu bởi "/"'
    return false
  }
  if (menuValue.value.MenuOrder <= 0) {
    error.value = 'Thứ tự menu phải là số dương'
    return false
  }
  return true
}

async function cancel() {
  updateMenuId.value = null
  showForm.value = false
  menuValue.value = {
    Id: null,
    MenuName: '',
    MenuPath: '/',
    MenuIcon: '',
    MenuOrder: 0,
  }
  await fetchMenus()
}

function addNew() {
  if (menus.value) {
    menuValue.value.MenuOrder = menus.value.length + 1
    showForm.value = true
  }
}

function update(menu: Menu) {
  showForm.value = true
  updateMenuId.value = menu.Id
  menuValue.value = menu
}

async function remove(menu: Menu) {
  if (!menus.value) return
  menus.value = menus.value?.filter((m) => m.Id != menu.Id)
  menus.value?.forEach((menu, index) => {
    menu.MenuOrder = index + 1
  })
}

async function saveOrder() {
  saveOrderLoading.value = true
  await store.dispatch('updateOrder', menus.value)
  saveOrderLoading.value = false
  fetchMenus()
}
async function fetchMenus() {
  await store.dispatch('fetchMenus')
  menus.value = store.getters.getMenus
}

onMounted(() => {
  fetchMenus()
})
</script>

<style scoped>
.custom-select {
  position: relative;
  width: 100%;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: pointer;
  background: #fff;
}

.selected {
  display: flex;
  align-items: center;
  padding: 6px;
  min-height: 36px;
}

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  width: 100%;
  background: #fff;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  z-index: 1000;
}

.dropdown-item {
  display: flex;
  align-items: center;
  padding: 8px;
  cursor: pointer;
}

.selected span,
.dropdown-item span {
  margin-left: 12px;
}

.dropdown-item:hover {
  background: #f0f0f0;
}
</style>
