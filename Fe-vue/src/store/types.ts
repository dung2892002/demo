import type { Customer } from '@/entities/Customer'
import type { Department } from '@/entities/Department'
import type { Employee } from '@/entities/Employee'
import type { Import } from '@/entities/Import'
import type { Position } from '@/entities/Position'
import type Menu from 'element-plus/es/components/menu/src/utils/menu-bar.mjs'

export interface State {
  employees: Employee[]
  departments: Department[]
  positions: Position[]
  employee: Employee | null
  totalPages: number
  totalRecords: number
  newEmployeeCode: string
  accessToken: string | null
  refreshToken: string | null
  username: string | null
  datasImport: []
  totalValid: number
  totalInvalid: number
  importSettings: Import[]
  customers: Customer[]
  customer: Customer | null
  customerGroups: []
  menus: Menu[]
  menu: Menu | null
  accounts: []
  roles: [] | null
  accountRoles: [] | null
  permissions: [] | null
  accountPermissions: [] | null
  onlineUsers: string[]
  listFolder: Document[]
}
