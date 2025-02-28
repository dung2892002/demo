import type { Employee } from './Employee'

export interface EmployeeFolder {
  Id: string
  Name: string
  ParentId: string | null
  EmployeeId: string | null
  Type: boolean
  CreatedAt: string | null
  Employee: Employee | null
}
