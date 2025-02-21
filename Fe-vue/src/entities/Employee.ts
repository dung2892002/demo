export interface Employee {
  EmployeeId?: string
  EmployeeCode: string
  Fullname: string
  DateOfBirth: string
  Gender: number
  GenderName: string
  IdentityNumber: string
  IdentityDate: string
  IdentityPlace: string
  Address: string
  MobileNumber: string
  LandlineNumber: string
  Email: string
  BankNumber: string
  BankName: string
  BankBranch: string
  PositionId: string[]
  PositionCode: string
  PositionName: string[]
  DepartmentId: string[]
  DepartmentCode: string
  DepartmentName: string[]
  CreatedDate: Date
  CreatedBy: string
  ModifiedDate: Date
  ModifiedBy: string
}
