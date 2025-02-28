export interface Document {
  Id: string | null
  Name: string
  Path?: string
  DocumentPath: string
  CreatedAt: string | null
  ParentId?: string | null
  CategoryId?: string | null
  Category?: DocumentCategory
  Type: DocumentType
  Children?: Document[]
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
