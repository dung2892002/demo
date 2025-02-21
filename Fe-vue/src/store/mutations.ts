import type { Department } from '@/entities/Department'
import type { Employee } from '@/entities/Employee'
import type { Position } from '@/entities/Position'
import type { MutationTree } from 'vuex/types/index.js'
import type { State } from './types'
import type { Customer } from '@/entities/Customer'
import type Menu from 'element-plus/es/components/menu/src/utils/menu-bar.mjs'

export const mutations: MutationTree<State> = {
  setEmployees(state, employees: Employee[]) {
    state.employees = employees
  },
  setEmployee(state, employee: Employee) {
    state.employee = employee
  },
  setDepartments(state, departments: Department[]) {
    state.departments = departments
  },
  setPositions(state, positions: Position[]) {
    state.positions = positions
  },
  setTotalRecords(state, totalRecords: number) {
    state.totalRecords = totalRecords
  },
  setTotalPages(state, totalPages: number) {
    state.totalPages = totalPages
  },
  setNewEmployeeCode(state, newEmployeeCode: string) {
    state.newEmployeeCode = newEmployeeCode
  },
  setAccessToken(state, token: string) {
    state.accessToken = token
  },
  setRefreshToken(state, token: string) {
    state.refreshToken = token
  },
  setUsername(state, username: string) {
    state.username = username
  },
  setDatasImport(state, datas) {
    state.datasImport = datas
  },
  setTotalValid(state, value) {
    state.totalValid = value
  },
  setTotalInValid(state, value) {
    state.totalInvalid = value
  },
  setImportSettings(state, settings) {
    state.importSettings = settings
  },
  setCustomers(state, customers: Customer[]) {
    state.customers = customers
  },
  setMenus(state, menus: Menu[]) {
    state.menus = menus
  },
  setMenu(state, menu: Menu) {
    state.menu = menu
  },
  setAccounts(state, accounts) {
    state.accounts = accounts
  },
  setRoles(state, roles) {
    state.roles = roles
  },
  setAccountRoles(state, roles) {
    state.accountRoles = roles
  },
  setPermissions(state, permissions) {
    state.permissions = permissions
  },
  setAccountPermissions(state, permissions) {
    state.accountPermissions = permissions
  },
  setCustomer(state, customer: Customer) {
    state.customer = customer
  },
  setcustomerGroups(state, groups) {
    state.customerGroups = groups
  },
  setOnlineUsers(state, users) {
    state.onlineUsers = users
  },
}
