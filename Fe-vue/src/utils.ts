import { DocumentType } from './entities/Document'

export function formatDate(inputDate: string) {
  const date = new Date(inputDate)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

export function getSrcIconDocument(type: DocumentType) {
  switch (type) {
    case DocumentType.Folder:
      return 'folder.png'
    case DocumentType.Word:
      return 'word.png'
    case DocumentType.Pdf:
      return 'pdf.png'
    case DocumentType.Excel:
      return 'excel.png'
    case DocumentType.Ppt:
      return 'ppt.png'
    case DocumentType.Image:
      return 'image.png'
    default:
      return 'file.png'
  }
}

export function formatDateForm(date: string) {
  const d = new Date(date)
  let month = '' + (d.getMonth() + 1)
  let day = '' + d.getDate()
  const year = d.getFullYear()

  if (month.length < 2) month = '0' + month
  if (day.length < 2) day = '0' + day

  return [year, month, day].join('-')
}
