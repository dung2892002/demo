import type { Customer } from '@/entities/Customer'
import type { Department } from '@/entities/Department'
import type { Employee } from '@/entities/Employee'
import type { Import } from '@/entities/Import'
import type { Position } from '@/entities/Position'

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
}
