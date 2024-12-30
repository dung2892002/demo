import type { GetterTree } from 'vuex/types/index.js'
import type { State } from './types'

export const getters: GetterTree<State, State> = {
  getEmployees: (state) => state.employees,
  getEmployee: (state) => state.employee,
  getPositions: (state) => state.positions,
  getDepartments: (state) => state.departments,
  getNewEmployeeCode: (state) => state.newEmployeeCode,
  getTotalRecords: (state) => state.totalRecords,
  getTotalPages: (state) => state.totalPages,
  getAccessToken: (state) => state.accessToken,
  getRefreshToken: (state) => state.refreshToken,
  getUsername: (state) => state.username,
  getDatasImport: (state) => state.datasImport,
  getTotalValid: (state) => state.totalValid,
  getTotalInvalid: (state) => state.totalInvalid,
  getImportSettings: (state) => state.importSettings,
  getCustomers: (state) => state.customers,
}
