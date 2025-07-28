using CsvHelper;
using OfficeOpenXml;
using System.Collections;
using System.Formats.Asn1;
using System.Globalization;

public class FileParser
{
    public List<Dictionary<string, string>> ParseFile(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLower();
        return ext switch
        {
            ".xlsx" => ParseExcel(filePath),
            ".csv" => ParseCsv(filePath),
            _ => throw new NotSupportedException("Тип файла не поддерживается")
        };
    }

    private List<Dictionary<string, string>> ParseExcel(string path)
    {
        using var package = new ExcelPackage(new FileInfo(path));
        var worksheet = package.Workbook.Worksheets[0];
        var headers = new List<string>();
        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            headers.Add(worksheet.Cells[1, col].Text.Trim());

        var data = new List<Dictionary<string, string>>();
        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var rowData = new Dictionary<string, string>();
            for (int col = 1; col <= headers.Count; col++)
                rowData[headers[col - 1]] = worksheet.Cells[row, col].Text.Trim();
            data.Add(rowData);
        }
        return data;
    }

    private List<Dictionary<string, string>> ParseCsv(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<dynamic>().ToList();
        return records.Select(r => ((IDictionary<string, object>)r).ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "")).ToList();
    }

    public bool CanMapTo(Type type, List<Dictionary<string, string>> data)
    {
        var props = type.GetProperties().Select(p => p.Name.ToLower()).ToHashSet();
        var firstRowKeys = data.FirstOrDefault()?.Keys.Select(k => k.ToLower()).ToHashSet();
        return firstRowKeys != null && firstRowKeys.All(k => props.Contains(k));
    }

    public IList MapDataToType(List<Dictionary<string, string>> data, Type type)
    {
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
        foreach (var row in data)
        {
            var obj = Activator.CreateInstance(type);
            foreach (var prop in type.GetProperties())
            {
                if (row.TryGetValue(prop.Name, out string value) && !string.IsNullOrEmpty(value))
                {
                    var converted = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(obj, converted);
                }
            }
            list.Add(obj);
        }
        return list;
    }
}
