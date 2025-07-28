using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InformationSystem.Presentation
{
    public class ExcelImportService
    {
        public async Task<List<T>> LoadFromExcelAsync<T>(string filePath)
        {
            var result = new List<T>();
            var properties = typeof(T).GetProperties();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheets.First();
                var headers = worksheet.Row(1).Cells().Select(c => c.Value.ToString()).ToList();

                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    var item = Activator.CreateInstance<T>();
                    var currentRow = worksheet.Row(row);

                    foreach (var prop in properties)
                    {
                        var headerIndex = headers.FindIndex(h => h.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                        if (headerIndex >= 0)
                        {
                            try
                            {
                                var cellValue = currentRow.Cell(headerIndex + 1).Value;
                                var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                var safeValue = Convert.ChangeType(cellValue, targetType);
                                prop.SetValue(item, safeValue);
                            }
                            catch
                            {
                                throw new Exception($"Ошибка при парсинге столбца {prop.Name} в строке {row}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Не найден заголовок {prop.Name} в Excel");
                        }
                    }

                    result.Add(item);
                }
            }

            return result;
        }
    }

}
