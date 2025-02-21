using Cukcuk.Core.Entities;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cukcuk.Core.Helper
{
    public class ExcelHelper
    {
        public static byte[] CreateFile<T>(List<T> datas)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Sheet");
            var header = sheet.CreateRow(0);

            var properties = typeof(T).GetProperties();

            var font = workbook.CreateFont();
            font.IsBold = true;
            var style = workbook.CreateCellStyle();
            style.SetFont(font);

            var cellIndex = 0;

            foreach (var property in properties)
            {
                var cell = header.CreateCell(cellIndex++);
                cell.SetCellValue(property.Name);
                cell.CellStyle = style;
            }

            var rowIndex = 1;

            foreach (var item in datas)
            {
                cellIndex = 0;
                var row = sheet.CreateRow(rowIndex++);
                var cell = row.CreateCell(0);
                cell.SetCellValue(rowIndex-1);
                foreach (var property in properties)
                {
                    var value = property.GetValue(item, null);
                    cell = row.CreateCell(cellIndex++);
                    if (value != null)
                    {
                        if (value.GetType() == typeof(int) || value.GetType() == typeof(int?))
                        {
                            cell.SetCellValue((int)value);
                        }
                        else if (value.GetType() == typeof(decimal) || value.GetType() == typeof(decimal?))
                        {
                            cell.SetCellValue(Convert.ToDouble(value));
                        }
                        else if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
                        {
                            cell.SetCellValue(((DateTime)value).ToString("dd/MM/yyyy"));
                        }
                        else if (value is List<string> stringList)
                        {

                            string cellContent = string.Join("\n- ", stringList);
                            if (stringList.Count > 0)
                            {
                                cellContent = "- " + cellContent;
                            }
                            cell.SetCellValue(cellContent);

                            ICellStyle celStyle = workbook.CreateCellStyle();
                            IFont cellFont = workbook.CreateFont();
                            cellFont.Color = IndexedColors.Red.Index;
                            celStyle.SetFont(cellFont);
                            celStyle.WrapText = true;
                            cell.CellStyle = celStyle;
                        }
                        else
                        {
                            cell.SetCellValue(value.ToString());
                        }
                    }
                    else
                    {
                        cell.SetCellValue("");
                    }
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using var stream = new MemoryStream();
            workbook.Write(stream);
            workbook.Close();
            return stream.ToArray();
        }

        public static async Task<List<T>> ReadFile<T>(IFormFile file, IEnumerable<Import> imports) where T : new()
        {
            if (!file.FileName.EndsWith(".xlsx"))
            {
                throw new InvalidOperationException("Chỉ chấp nhận file có định dạng xlsx");
            }

            var datas = new List<T>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;


            var workbook = new XSSFWorkbook(stream);

            var sheet = workbook.GetSheetAt(0);
            var header = sheet.GetRow(0);

            var columns = new List<string>();

            for (int i = 0; i < header.Cells.Count; i++)
            {
                var columnName = header.GetCell(i).ToString().Trim();
                var import = imports.Where(i => i.ColumnName == columnName).FirstOrDefault() ?? throw new ArgumentException($"Cột {columnName} trong file chưa được cấu hình phù hợp");
                columns.Add(import.PropertyName); 
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row == null || row.Cells.All(cell => string.IsNullOrEmpty(cell?.ToString().Trim())))
                {
                    continue; 
                }
                var data = new T();
                var errors = new List<string>();
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var columnName = header.GetCell(j).ToString().Trim();
                    var property = typeof(T).GetProperty(columns[j]);
                    if (property == null)
                    {
                        continue;
                    }

                    var value = row.GetCell(j).ToString() ?? string.Empty; 

                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        if (Int32.TryParse(value, out var intValue))
                        {
                            property.SetValue(data, intValue);
                        }
                        else
                        {
                            errors.Add($"Giá trị {value} cho cột {columnName} không hợp lệ");
                        }
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                    {
                        if (decimal.TryParse(value, out var decimalValue))
                        {
                            property.SetValue(data, decimalValue);
                        }
                        else
                        {
                            errors.Add($"Giá trị {value} cho cột {columnName} không hợp lệ");
                        }

                    }
                    else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        if (Regex.IsMatch(value, @"^\d{4}$"))
                        {
                            value = $"01/01/{value}";
                        }
                        else if (Regex.IsMatch(value, @"^\d{1,2}/\d{4}$"))
                        {
                            value = $"01/{value}";
                        }

                        if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                        {
                            property.SetValue(data, parsedDate);
                        }
                        else
                        {
                            property.SetValue(data, null);
                            errors.Add($"Giá trị {value} cho cột {columnName} không hợp lệ");
                        }
                    }
                    else
                    {
                        property.SetValue(data, value);
                    }
                }

                var errorProperty = typeof(T).GetProperty("Errors");
                if (errorProperty != null)
                {
                    errorProperty.SetValue(data, errors);
                }
                datas.Add(data);
            }
            return datas;
        }
    }
}
