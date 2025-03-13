export interface Document {
  Id: string | null
  Name: string
  Path?: string
  FolderPath: string
  CreatedAt: string | null
  ParentId?: string | null
  CategoryId?: string | null
  Category?: DocumentCategory
  Type: DocumentType
  Children?: Document[]
  IsLoaded?: boolean
  IsExpend?: boolean

  DocumentBlocks: DocumentBlock[]
  Issuer: string | null
  IssueDate: string | null
  DocumentNo: string | null
  SignerName: string | null
  IsLaw: boolean
}

export interface DocumentCategory {
  Id: string
  Name: string
}

export enum DocumentType {
  Folder = 'Folder',
  Word = 'Word',
  Pdf = 'Pdf',
  Excel = 'Excel',
  Ppt = 'Ppt',
  Image = 'Image',
  Unknown = 'Unknown',
}

export interface DocumentBlock {
  Id: string | null
  ParentId: string | null
  DocumentId: string
  Title: string
  Content: string
  Level: number
  ContentType: number
  Order: number
  IsExpend?: boolean
}
