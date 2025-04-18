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
  MarkdownContent?: string

  DocumentBlocks: DocumentBlock[]
  Issuer: string | null
  IssueDate: string | null
  DocumentNo: string | null
  DocumentName: string | null
  SignerName: string | null
  EffectiveDate: string | null
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
  Link = 'Link',
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
  IsExpand?: boolean
  IsShow?: boolean
  State?: number /*0: khong thay doi, 1: cap nhat, 2: them moi, 3: xoa bo, 4: xoa sau khi them */
}
