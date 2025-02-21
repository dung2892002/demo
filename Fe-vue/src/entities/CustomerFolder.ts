import type { Customer } from './Customer'

export type CustomerFolder = {
  Id: string
  Name: string
  ParentId: string | null
  CustomerId: string | null
  Type: boolean
  CreatedAt: string | null
  Customer: Customer | null
}
