import type { State } from './types'

export const state: State = {
  employees: [],
  departments: [],
  positions: [],
  totalPages: 0,
  totalRecords: 0,
  newEmployeeCode: '',
  employee: null,
  accessToken: localStorage.getItem('accessToken') || null,
  refreshToken: null,
  username: localStorage.getItem('username') || null,
  datasImport: [],
  totalInvalid: 0,
  totalValid: 0,
  importSettings: [],
  customers: [],
  menus: [],
  menu: null,
  accounts: [],
  roles: null,
  accountRoles: null,
  permissions: null,
  accountPermissions: null,
  customer: null,
  customerGroups: [],
  onlineUsers: [],
  listFolder: [],
}
