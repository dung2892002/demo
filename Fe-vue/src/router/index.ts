import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginForm from '@/components/LoginForm.vue'
import ImportPage from '@/views/ImportPage.vue'
import SettingPage from '@/views/settings/SettingPage.vue'
import SettingMenu from '@/views/settings/SettingMenu.vue'
import ListAccount from '@/views/account/ListAccount.vue'
import EmployeePage from '@/views/employee/EmployeePage.vue'
import CustomerPage from '@/views/customer/CustomerPage.vue'
import ChatRealtime from '@/views/ChatRealtime.vue'
import SettingExcel from '@/views/settings/SettingExcel.vue'
import StatisticalPage from '@/views/statistical/StatisticalPage.vue'
import DocumentPage from '@/views/document/DocumentPage.vue'

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
          children: [],
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
              path: '/setting/excel',
              name: 'setting-excel',
              component: SettingExcel,
              meta: { requiresAuth: true },
            },
            {
              path: '/setting/menu',
              name: 'setting-menu',
              component: SettingMenu,
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
          children: [],
        },
        {
          path: '/chats',
          name: 'chats',
          component: ChatRealtime,
          meta: { requiresAuth: true },
        },
        {
          path: '/statistical',
          name: 'statisticals',
          component: StatisticalPage,
          meta: { requiresAuth: true },
        },
        {
          path: '/documents',
          name: 'documents',
          component: DocumentPage,
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
