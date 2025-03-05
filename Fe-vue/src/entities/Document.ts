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
