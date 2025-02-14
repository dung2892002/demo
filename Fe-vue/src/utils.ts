export function formatDate(inputDate: string) {
  const date = new Date(inputDate)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const year = date.getFullYear()
  return `${day}/${month}/${year}`
}

export function getFileFormat(name: string): string {
  const format = name.split('.').pop()?.toLowerCase()

  if (format === 'xlsx' || format === 'xls') return 'excel'
  if (format === 'pdf') return 'pdf'
  if (format === 'ppt' || format === 'pptx') return 'power-point'
  if (format === 'doc' || format === 'docx') return 'word'
  return ''
}

export function getIconUrl(name: string): string {
  const formatFile = getFileFormat(name)
  return `/src/assets/icon/${formatFile}.png`
}
