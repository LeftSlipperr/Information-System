using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Domain.Models;
using ExcelDataReader;
using InformationSystem.Application.DTO;
using InformationSystem.Presentation.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InformationSystem.Presentation
{
    public partial class AccountantWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        string baseUrl;
        private string _selectedTable;
        AdminViewModel _viewModel;
        public AccountantWindow()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            string baseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5000"; // ← fallback

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _viewModel = new AdminViewModel(_httpClient, baseUrl);
            DataContext = _viewModel;
        }

        private async void TablesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablesListView.SelectedItem is ListBoxItem selectedItem)
            {
                // Получаем техническое имя таблицы (для логики)
                _selectedTable = selectedItem.Tag?.ToString();

                // Получаем пользовательское название (для отображения)
                if (selectedItem.Content is StackPanel panel)
                {
                    var textBlock = panel.Children.OfType<TextBlock>().FirstOrDefault();
                    if (textBlock != null)
                    {
                        TableTitle.Text = textBlock.Text;
                    }
                }

                await LoadTableData();
            }
        }
        private void MainDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var property = e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor;
            if (property != null)
            {
                var displayNameAttr = property.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
                if (displayNameAttr != null && !string.IsNullOrWhiteSpace(displayNameAttr.DisplayName))
                {
                    e.Column.Header = displayNameAttr.DisplayName;
                }
            }

            // Скрываем все поля, которые заканчиваются на "Id", но НЕ скрываем составные объекты
            if (e.PropertyName.EndsWith("Id") && e.PropertyName != "UserId") // можно добавить исключения по необходимости
            {
                e.Cancel = true;
                return;
            }

            // Кастомный вывод связанных сущностей (например, TradePoint.Name)
            if (e.PropertyName == "TradePoint")
            {
                e.Column = new DataGridTextColumn
                {
                    Header = "Торговая точка",
                    Binding = new Binding("TradePoint.Name")
                };
            }
            else if (e.PropertyName == "TradePointEconomics")
            {
                e.Column = new DataGridTextColumn
                {
                    Header = "Профит",
                    Binding = new Binding("TradePointEconomics.Profit")
                };
            }

            // Можно добавить кастомный вывод для других объектов, если нужно
        }


        private async Task LoadTableData()
        {
            try
            {
                await _viewModel.LoadTableAsync(_selectedTable);

                MainDataGrid.AutoGenerateColumns = false; // Сначала выключим
                MainDataGrid.Columns.Clear();             // Очистим
                MainDataGrid.AutoGenerateColumns = true;  // А теперь снова включим

                MainDataGrid.ItemsSource = null;          // Сброс источника
                MainDataGrid.ItemsSource = _viewModel.TableData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }


        private async void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTable == "Salary")
            {
                MessageBox.Show("Добавление записей в таблицу 'Зарплаты' отключено.", "Ограничение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_viewModel._tableTypeMap.TryGetValue(_selectedTable, out var modelType))
            {
                var addWindow = new AddRecordWindow(modelType, _selectedTable, _httpClient, baseUrl);
                if (addWindow.ShowDialog() == true)
                {
                    await LogActionAsync(SessionManager.CurrentUserId, "Добавление записи в " + _selectedTable);
                    await LoadTableData(); // Обновление таблицы
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить тип модели.");
            }
        }

        private async void EditRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null || MainDataGrid.SelectedItem == null)
                return;

            var selectedItem = MainDataGrid.SelectedItem;

            if (_viewModel._tableTypeMap.TryGetValue(_selectedTable, out var modelType))
            {
                var editWindow = new AddRecordWindow(modelType, _selectedTable, _httpClient, baseUrl, selectedItem);
                if (editWindow.ShowDialog() == true)
                {
                    await LogActionAsync(SessionManager.CurrentUserId, "Редактирование записи в " + _selectedTable);
                    await LoadTableData(); // обновить данные
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить тип модели.");
            }
        }

        private async void DeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null || MainDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана таблица или запись.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранную запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                if (_viewModel._tableTypeMap.TryGetValue(_selectedTable, out var modelType))
                {
                    await _viewModel.DeleteRecordAsync(MainDataGrid.SelectedItem, _selectedTable);
                    await LogActionAsync(SessionManager.CurrentUserId, "Удаление записи в " + _selectedTable);
                    await LoadTableData(); // Обновляем после удаления
                }
                else
                {
                    MessageBox.Show("Не удалось определить тип модели.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
            }
        }


        private async void RefreshTable_Click(object sender, RoutedEventArgs e)
        {
            await LoadTableData();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();
            DateTime? dateFrom = DateFromPicker.SelectedDate;
            DateTime? dateTo = DateToPicker.SelectedDate;

            try
            {
                await _viewModel.SearchTableAsync(_selectedTable, searchText, dateFrom, dateTo);
                MainDataGrid.ItemsSource = null;
                MainDataGrid.ItemsSource = _viewModel.TableData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}");
            }

        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {

            if (_selectedTable == null || _selectedTable == "User" || _selectedTable == "TradePoint" || _selectedTable == "Log")
            {
                MessageBox.Show("Для данной таблицы построение статистики невозможно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (TablesListView.SelectedItem is ListViewItem selectedItem)
            {
                string tableName = selectedItem.Tag.ToString();
                var statsWindow = new StatisticsWindow(tableName, _httpClient);
                statsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите таблицу для анализа.");
            }
        }

        private void Balance_Click(object sender, RoutedEventArgs e)
        {
            var staticticsWindow = new StatisticsForecastWindow();
            staticticsWindow.ShowDialog();
        }

        private string GetDisplayName(PropertyDescriptor prop)
        {
            var displayNameAttr = prop.Attributes[typeof(System.ComponentModel.DisplayNameAttribute)]
                                  as System.ComponentModel.DisplayNameAttribute;

            return displayNameAttr?.DisplayName ?? prop.Name;
        }


        public async void ImportFromGoogleSheets_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedTable))
            {
                MessageBox.Show("Сначала выберите таблицу для импорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv",
                Title = "Выберите файл для импорта"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                List<object> importedRecords;

                if (dialog.FileName.EndsWith(".xlsx"))
                {
                    importedRecords = ReadExcelFile(dialog.FileName, _selectedTable);
                }
                else // CSV
                {
                    importedRecords = ReadCsvFile(dialog.FileName, _selectedTable);
                }

                if (importedRecords.Count == 0)
                {
                    MessageBox.Show("Файл не содержит данных или формат не поддерживается.");
                    return;
                }

                foreach (var record in importedRecords)
                {
                    await _viewModel.AddRecordAsync(record);
                }

                await LoadTableData();
                await LogActionAsync(SessionManager.CurrentUserId, "Импорт файла из " + _selectedTable);
                MessageBox.Show("Импорт завершен успешно.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}");
            }
        }

        private List<object> ReadExcelFile(string filePath, string tableName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var records = new List<object>();

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();

            var table = result.Tables[0];
            var columns = table.Rows[0].ItemArray.Select(x => x.ToString()).ToArray();

            if (!_viewModel._tableTypeMap.TryGetValue(tableName, out var modelType))
                throw new Exception("Не удалось определить тип модели.");

            for (int i = 1; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i].ItemArray;
                var instance = Activator.CreateInstance(modelType);

                for (int j = 0; j < columns.Length && j < row.Length; j++)
                {
                    var prop = modelType.GetProperty(columns[j]);
                    if (prop != null && row[j] != DBNull.Value)
                    {
                        var value = Convert.ChangeType(row[j], Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        prop.SetValue(instance, value);
                    }
                }

                records.Add(instance);
            }
            return records;
        }
        private List<object> ReadCsvFile(string filePath, string tableName)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

            if (!_viewModel._tableTypeMap.TryGetValue(tableName, out var modelType))
                throw new Exception("Не удалось определить тип модели.");

            var list = new List<object>();
            var records = csv.GetRecords(modelType);
            foreach (var record in records)
                list.Add(record);

            return list;
        }
        public async void ExportToCsv_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedTable) || _viewModel.TableData == null || !_viewModel.TableData.Any())
            {
                MessageBox.Show("Нет данных для экспорта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currencyWindow = new CurrencySelectionWindow();
            if (currencyWindow.ShowDialog() != true)
                return; // пользователь отменил выбор

            string selectedCurrency = currencyWindow.SelectedCurrency;
            decimal selectedRate = currencyWindow.SelectedRate;

            var dialog = new SaveFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv",
                Title = "Сохранить экспорт",
                FileName = $"{_selectedTable}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                using var writer = new StreamWriter(dialog.FileName, false, Encoding.UTF8);
                using var csv = new CsvHelper.CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                });

                var records = _viewModel.TableData;

                if (records.First() is IDictionary<string, object>) // dynamic
                {
                    var first = (IDictionary<string, object>)records.First();
                    var headers = first.Keys.Where(k => !k.EndsWith("Id")).ToList();

                    foreach (var h in headers)
                        csv.WriteField(h);
                    csv.WriteField($"Сумма в {selectedCurrency}");
                    csv.NextRecord();

                    string[] sumKeywords = { "Сумма", "Чистая прибыль/убыток", "Прибыль", "Стоимость покупки", "Прибыль/Убыток" };

                    foreach (var record in records)
                    {
                        var dict = (IDictionary<string, object>)record;
                        foreach (var h in headers)
                        {
                            dict.TryGetValue(h, out var value);
                            csv.WriteField(value is DateTime dt ? dt.ToString("yyyy-MM-dd") : value?.ToString() ?? "");
                        }


                        object sumValue = null;
                        foreach (var key in dict.Keys)
                        {
                            if (sumKeywords.Any(k => key.ToLower().Contains(k.ToLower())))
                            {
                                sumValue = dict[key];
                                break;
                            }
                        }

                        if (sumValue is decimal dec)
                            csv.WriteField((dec / selectedRate).ToString("F2"));
                        else if (decimal.TryParse(sumValue?.ToString(), out var parsed))
                            csv.WriteField((parsed / selectedRate).ToString("F2"));
                        else
                            csv.WriteField("");

                        csv.NextRecord();
                    }

                }
                else
                {
                    var type = records.First().GetType();
                    var props = TypeDescriptor.GetProperties(type)
                        .Cast<PropertyDescriptor>()
                        .Where(p => !p.Name.EndsWith("Id"))
                        .ToList();

                    foreach (var prop in props)
                        csv.WriteField(GetDisplayName(prop));
                    csv.WriteField($"Сумма в {selectedCurrency}");
                    csv.NextRecord();

                    foreach (var record in records)
                    {
                        string[] sumKeywords = { "Сумма", "Чистая прибыль/убыток", "Прибыль", "Стоимость покупки", "Прибыль/Убыток" };
                        decimal? originalSum = null;

                        foreach (var prop in props)
                        {
                            var value = prop.GetValue(record);
                            csv.WriteField(value is DateTime dt ? dt.ToString("yyyy-MM-dd") : value?.ToString() ?? "");

                            if (originalSum == null && (prop.Name.ToLower().Contains("amount") || prop.Name.ToLower().Contains("sum") || prop.Name.ToLower().Contains("price")))
                            {
                                if (value is decimal dec)
                                    originalSum = dec;
                                else if (decimal.TryParse(value?.ToString(), out var parsed))
                                    originalSum = parsed;
                            }
                        }

                        csv.WriteField(originalSum.HasValue ? (originalSum.Value / selectedRate).ToString("F2") : "");
                        csv.NextRecord();
                    }
                }



            }
            catch
            {
                MessageBox.Show("Ошибка импорта");
            }
        }

        public void ExportInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.TableData == null || !_viewModel.TableData.Any())
            {
                MessageBox.Show("Нет данных для экспорта.");
                return;
            }

            var currencyWindow = new CurrencySelectionWindow();
            if (currencyWindow.ShowDialog() != true)
                return;

            string selectedCurrency = currencyWindow.SelectedCurrency;
            decimal selectedRate = currencyWindow.SelectedRate;


            var selected = _viewModel.SelectedRecord;
            var allData = _viewModel.TableData.ToList();

            try
            {
                switch (_selectedTable)
                {
                    case "FinishedGoodsWarehouse":
                        var products = selected != null
                            ? new List<FinishedGoodsWarehouseDto> { (FinishedGoodsWarehouseDto)selected }
                            : allData.Cast<FinishedGoodsWarehouseDto>().ToList();
                        ExportFinishedGoodsInvoice(products, selectedCurrency, selectedRate);
                        break;

                    case "RawMaterialWarehouse":
                        var raws = selected != null
                            ? new List<RawMaterialWarehouseDto> { (RawMaterialWarehouseDto)selected }
                            : allData.Cast<RawMaterialWarehouseDto>().ToList();
                        ExportRawMaterialInvoice(raws, selectedCurrency, selectedRate);
                        break;

                    case "Salary":
                        var salaries = selected != null
                            ? new List<SalaryDto> { (SalaryDto)selected }
                            : allData.Cast<SalaryDto>().ToList();
                        ExportSalaryInvoice(salaries, selectedCurrency, selectedRate);
                        break;
                    case "TradePointEconomics":
                        var economics = selected != null
                            ? new List<TradePointEconomicsDto> { (TradePointEconomicsDto)selected }
                            : allData.Cast<TradePointEconomicsDto>().ToList();
                        ExportTradePointEconomicsInvoice(economics, selectedCurrency, selectedRate);
                        break;
                    default:
                        MessageBox.Show("Экспорт для этой таблицы пока не реализован.");
                        break;
                }
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Ошибка преобразования типов при экспорте. Проверьте соответствие модели.");
            }
        }

        public void ExportFinishedGoodsInvoice(List<FinishedGoodsWarehouseDto> products, string currency, decimal rate)
        {
            string templatePath = "Накладная_шаблон_склад_готовой_продукции.docx";

            if (!File.Exists(templatePath))
            {
                MessageBox.Show($"Файл шаблона не найден по пути:\n{templatePath}");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Накладная_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                DefaultExt = ".docx",
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Сохранить накладную"
            };

            if (dialog.ShowDialog() != true)
                return;

            string savePath = dialog.FileName;

            File.Copy(templatePath, savePath, true);

            using (var doc = WordprocessingDocument.Open(savePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                ReplacePlaceholderText(body, "InvoiceNumber", $"НК-{DateTime.Today:yyyyMMdd}-{new Random().Next(100, 999)}");
                ReplacePlaceholderText(body, "InvoiceDate", DateTime.Today.ToString("dd.MM.yyyy"));
                ReplacePlaceholderText(body, "TotalAmount", (Math.Floor(products.Sum(p => p.PotentialRevenue) / rate * 100) / 100).ToString("F2") + " " + currency);


                var table = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().FirstOrDefault();
                if (table == null)
                {
                    MessageBox.Show("В шаблоне не найдена таблица.");
                    return;
                }

                var templateRow = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .FirstOrDefault(tr => tr.InnerText.Contains("Number") && tr.InnerText.Contains("ProductName"));

                if (templateRow == null)
                {
                    MessageBox.Show("В шаблоне не найдена строка-шаблон с плейсхолдерами.");
                    return;
                }

                foreach (var (item, index) in products.Select((p, i) => (p, i + 1)))
                {
                    var newRow = (DocumentFormat.OpenXml.Wordprocessing.TableRow)templateRow.CloneNode(true);

                    ReplacePlaceholderText(newRow, "Number", index.ToString());
                    ReplacePlaceholderText(newRow, "ProductName", item.Name);
                    ReplacePlaceholderText(newRow, "Quantity", item.Quantity.ToString());
                    ReplacePlaceholderText(newRow, "Total", (Math.Floor(item.PotentialRevenue / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Price", item.LastUpdated.ToString("dd.MM.yyyy"));

                    table.AppendChild(newRow);
                }

                templateRow.Remove();
                doc.MainDocumentPart.Document.Save();
            }

            MessageBox.Show($"Накладная успешно экспортирована:\n{savePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ExportRawMaterialInvoice(List<RawMaterialWarehouseDto> products, string currency, decimal rate)
        {
            string templatePath = "Накладная_шаблон_расходы_на_сырье.docx";

            if (!File.Exists(templatePath))
            {
                MessageBox.Show($"Файл шаблона не найден по пути:\n{templatePath}");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Накладная_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                DefaultExt = ".docx",
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Сохранить накладную"
            };

            if (dialog.ShowDialog() != true)
                return;

            string savePath = dialog.FileName;

            File.Copy(templatePath, savePath, true);

            using (var doc = WordprocessingDocument.Open(savePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                ReplacePlaceholderText(body, "InvoiceNumber", $"НК-{DateTime.Today:yyyyMMdd}-{new Random().Next(100, 999)}");
                ReplacePlaceholderText(body, "InvoiceDate", DateTime.Today.ToString("dd.MM.yyyy"));
                ReplacePlaceholderText(body, "TotalAmount", (Math.Floor(products.Sum(p => p.PurchaseCost) / rate * 100) / 100).ToString("F2") + " " + currency);


                var table = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().FirstOrDefault();
                if (table == null)
                {
                    MessageBox.Show("В шаблоне не найдена таблица.");
                    return;
                }

                var templateRow = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .FirstOrDefault(tr => tr.InnerText.Contains("Number") && tr.InnerText.Contains("ProductName"));

                if (templateRow == null)
                {
                    MessageBox.Show("В шаблоне не найдена строка-шаблон с плейсхолдерами.");
                    return;
                }

                foreach (var (item, index) in products.Select((p, i) => (p, i + 1)))
                {
                    var newRow = (DocumentFormat.OpenXml.Wordprocessing.TableRow)templateRow.CloneNode(true);

                    ReplacePlaceholderText(newRow, "Number", index.ToString());
                    ReplacePlaceholderText(newRow, "ProductName", item.MaterialName);
                    ReplacePlaceholderText(newRow, "Quantity", item.Quantity.ToString());
                    ReplacePlaceholderText(newRow, "Total", (Math.Floor(item.PurchaseCost / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Price", item.LastUpdated.ToString("dd.MM.yyyy"));

                    table.AppendChild(newRow);
                }

                templateRow.Remove();
                doc.MainDocumentPart.Document.Save();
            }

            MessageBox.Show($"Накладная успешно экспортирована:\n{savePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ExportSalaryInvoice(List<SalaryDto> products, string currency, decimal rate)
        {
            string templatePath = "Накладная_шаблон_расходы_на_зарплаты.docx";

            if (!File.Exists(templatePath))
            {
                MessageBox.Show($"Файл шаблона не найден по пути:\n{templatePath}");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Накладная_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                DefaultExt = ".docx",
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Сохранить накладную"
            };

            if (dialog.ShowDialog() != true)
                return;

            string savePath = dialog.FileName;

            File.Copy(templatePath, savePath, true);

            using (var doc = WordprocessingDocument.Open(savePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                ReplacePlaceholderText(body, "InvoiceNumber", $"НК-{DateTime.Today:yyyyMMdd}-{new Random().Next(100, 999)}");
                ReplacePlaceholderText(body, "InvoiceDate", DateTime.Today.ToString("dd.MM.yyyy"));
                ReplacePlaceholderText(body, "TotalAmount", (Math.Floor(products.Sum(p => p.Amount) / rate * 100) / 100).ToString("F2") + " " + currency);

                var table = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().FirstOrDefault();
                if (table == null)
                {
                    MessageBox.Show("В шаблоне не найдена таблица.");
                    return;
                }

                var templateRow = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .FirstOrDefault(tr => tr.InnerText.Contains("Number"));

                if (templateRow == null)
                {
                    MessageBox.Show("В шаблоне не найдена строка-шаблон с плейсхолдерами.");
                    return;
                }

                foreach (var (item, index) in products.Select((p, i) => (p, i + 1)))
                {
                    var newRow = (DocumentFormat.OpenXml.Wordprocessing.TableRow)templateRow.CloneNode(true);

                    ReplacePlaceholderText(newRow, "Number", index.ToString());
                    ReplacePlaceholderText(newRow, "Total", (Math.Floor(item.Amount / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Price", item.PaymentDate.ToString("dd.MM.yyyy"));

                    table.AppendChild(newRow);
                }

                templateRow.Remove();
                doc.MainDocumentPart.Document.Save();
            }

            MessageBox.Show($"Накладная успешно экспортирована:\n{savePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ExportTradePointEconomicsInvoice(List<TradePointEconomicsDto> records, string currency, decimal rate)
        {
            string templatePath = "Накладная_шаблон_экономика_торговой_точки.docx";

            if (!File.Exists(templatePath))
            {
                MessageBox.Show($"Файл шаблона не найден по пути:\n{templatePath}");
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Экономика_ТТ_{DateTime.Now:yyyyMMdd_HHmmss}.docx",
                DefaultExt = ".docx",
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Сохранить накладную"
            };

            if (dialog.ShowDialog() != true)
                return;

            string savePath = dialog.FileName;

            File.Copy(templatePath, savePath, true);

            using (var doc = WordprocessingDocument.Open(savePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                ReplacePlaceholderText(body, "InvoiceNumber", $"ТТ-{DateTime.Today:yyyyMMdd}-{new Random().Next(100, 999)}");
                ReplacePlaceholderText(body, "InvoiceDate", DateTime.Today.ToString("dd.MM.yyyy"));

                decimal totalProfit = records.Sum(r => r.Revenue);
                decimal totalExpenses = records.Sum(r => r.Expenses);
                decimal totalNet = totalProfit - totalExpenses;

                ReplacePlaceholderText(body, "TotalIncome", (Math.Floor(totalProfit / rate * 100) / 100).ToString("F2") + " " + currency);
                ReplacePlaceholderText(body, "TotalExpense", (Math.Floor(totalExpenses / rate * 100) / 100).ToString("F2") + " " + currency);
                ReplacePlaceholderText(body, "TotalNet", Math.Floor((totalNet / rate * 100) / 100).ToString("F2") + " " + currency);

                var table = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().FirstOrDefault();
                if (table == null)
                {
                    MessageBox.Show("В шаблоне не найдена таблица.");
                    return;
                }

                var templateRow = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>()
                    .FirstOrDefault(tr => tr.InnerText.Contains("TradePointName"));

                if (templateRow == null)
                {
                    MessageBox.Show("В шаблоне не найдена строка-шаблон с плейсхолдерами.");
                    return;
                }

                foreach (var (item, index) in records.Select((r, i) => (r, i + 1)))
                {
                    var newRow = (DocumentFormat.OpenXml.Wordprocessing.TableRow)templateRow.CloneNode(true);

                    ReplacePlaceholderText(newRow, "Number", index.ToString());
                    ReplacePlaceholderText(newRow, "TradePointName", item.TradePointName);
                    ReplacePlaceholderText(newRow, "Income", (Math.Floor(item.Revenue / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Expenses", (Math.Floor(item.Expenses / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Net", (Math.Floor((item.Profit - item.Expenses) / rate * 100) / 100).ToString("F2") + " " + currency);
                    ReplacePlaceholderText(newRow, "Date", item.Period.ToString("dd.MM.yyyy"));

                    table.AppendChild(newRow);
                }

                templateRow.Remove();
                doc.MainDocumentPart.Document.Save();
            }

            MessageBox.Show($"Накладная успешно экспортирована:\n{savePath}", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void ReplacePlaceholderText(OpenXmlElement element, string placeholder, string value)
        {
            var runs = element.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>().ToList();

            for (int i = 0; i < runs.Count; i++)
            {
                var text = runs[i].Elements<DocumentFormat.OpenXml.Wordprocessing.Text>().FirstOrDefault();
                if (text == null) continue;

                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, value);
                }
                else if (text.Text.StartsWith("{{") || text.Text.Contains("{"))
                {
                    string combined = text.Text;
                    int j = i + 1;

                    while (j < runs.Count && !combined.Contains("}}"))
                    {
                        var nextText = runs[j].Elements<DocumentFormat.OpenXml.Wordprocessing.Text>().FirstOrDefault();
                        if (nextText != null)
                            combined += nextText.Text;
                        j++;
                    }

                    if (combined.Contains(placeholder))
                    {
                        string newText = combined.Replace(placeholder, value);

                        for (int k = i; k < j; k++)
                        {
                            var t = runs[k].Elements<DocumentFormat.OpenXml.Wordprocessing.Text>().FirstOrDefault();
                            if (t != null)
                                t.Text = string.Empty;
                        }

                        var firstText = runs[i].Elements<DocumentFormat.OpenXml.Wordprocessing.Text>().FirstOrDefault();
                        if (firstText != null)
                            firstText.Text = newText;
                    }
                }
            }
        }



        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            var window = new ExchangeWindow(); // создадим окно
            window.ShowDialog();
        }


        public void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }


        private async Task LogActionAsync(Guid userID, string action)
        {
            var logDto = new LogDto
            {
                Timestamp = DateTime.Now,
                Action = action,
                UserId = userID
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(logDto, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                await _httpClient.PostAsync("api/Log/AddLog", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка логирования: {ex.Message}");
            }
        }
    }
}
