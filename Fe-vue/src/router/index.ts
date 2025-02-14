import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginForm from '@/components/LoginForm.vue'
import ImportPage from '@/views/ImportPage.vue'
import SettingImport from '@/views/settings/SettingImport.vue'
import SettingPage from '@/views/settings/SettingPage.vue'
import SettingMenu from '@/views/settings/SettingMenu.vue'
import ListAccount from '@/views/account/ListAccount.vue'
import EmployeePage from '@/views/employee/EmployeePage.vue'
import CustomerPage from '@/views/customer/CustomerPage.vue'
import ChatRealtime from '@/views/ChatRealtime.vue'
import SettingFolder from '@/views/settings/SettingFolder.vue'
import DepartmentList from '@/views/employee/DepartmentList.vue'
import PositionList from '@/views/employee/PositionList.vue'
import GroupList from '@/views/customer/GroupList.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true },
      children: [
        {
          path: '/employees',
          name: 'employees',
          component: EmployeePage,
          meta: { requiresAuth: true },
          children: [
            {
              path: '/employees/department',
              component: DepartmentList,
              meta: { requiresAuth: true },
            },
            {
              path: '/employees/position',
              component: PositionList,
              meta: { requiresAuth: true },
            },
          ],
        },
        {
          path: '/import/:data',
          name: 'importPage',
          component: ImportPage,
          meta: { requiresAuth: true },
        },
        {
          path: '/setting',
          name: 'setting',
          component: SettingPage,
          meta: { requiresAuth: true },
          children: [
            {
              path: '/setting/import',
              name: 'setting-import',
              component: SettingImport,
              meta: { requiresAuth: true },
            },
            {
              path: '/setting/menu',
              name: 'setting-menu',
              component: SettingMenu,
              meta: { requiresAuth: true },
            },
            {
              path: '/setting/folder',
              name: 'setting-folder',
              component: SettingFolder,
              meta: { requiresAuth: true },
            },
          ],
        },
        {
          path: '/accounts',
          name: 'accountList',
          component: ListAccount,
          meta: { requiresAuth: true },
        },
        {
          path: '/customers',
          name: 'customers',
          component: CustomerPage,
          meta: { requiresAuth: true },
          children: [
            {
              path: '/customers/customer-group',
              component: GroupList,
              meta: { requiresAuth: true },
            },
          ],
        },
        {
          path: '/chats',
          name: 'chats',
          component: ChatRealtime,
          meta: { requiresAuth: true },
        },
      ],
    },
    {
      path: '/login',
      name: 'login',
      component: LoginForm,
    },
  ],
})

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('accessToken')
  if (to.meta.requiresAuth && isAuthenticated === null) {
    next({
      name: 'login',
    })
  } else {
    next()
  }
})

export default router
