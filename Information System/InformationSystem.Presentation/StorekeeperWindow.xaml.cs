using Domain.Models;
using ExcelDataReader;
using InformationSystem.Application.DTO;
using InformationSystem.Presentation.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <summary>
    /// Логика взаимодействия для StorekeeperWindow.xaml
    /// </summary>
    public partial class StorekeeperWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        string baseUrl;
        private string _selectedTable;
        AdminViewModel _viewModel;
        public StorekeeperWindow()
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

                // ExpandoObject (dynamic)
                if (records.First() is IDictionary<string, object>)
                {
                    var firstRecord = (IDictionary<string, object>)records.First();
                    var headers = firstRecord.Keys
                        .Where(k => !k.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var header in headers)
                        csv.WriteField(header);
                    csv.NextRecord();

                    foreach (var record in records)
                    {
                        var dict = (IDictionary<string, object>)record;
                        foreach (var header in headers)
                        {
                            dict.TryGetValue(header, out var value);

                            if (value is DateTime dateValue)
                                csv.WriteField(dateValue.ToString("yyyy-MM-dd HH:mm:ss")); // формат даты
                            else
                                csv.WriteField(value?.ToString() ?? "");
                        }
                        csv.NextRecord();
                    }
                }
                else
                {
                    // Обычные DTO
                    var type = records.First().GetType();
                    var props = type.GetProperties()
                        .Where(p => !p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var prop in props)
                        csv.WriteField(prop.Name);
                    csv.NextRecord();

                    foreach (var record in records)
                    {
                        foreach (var prop in props)
                        {
                            var value = prop.GetValue(record);
                            if (value is DateTime dateValue)
                                csv.WriteField(dateValue.ToString("yyyy-MM-dd HH:mm:ss"));
                            else
                                csv.WriteField(value?.ToString() ?? "");
                        }
                        csv.NextRecord();
                    }
                }
                await LogActionAsync(SessionManager.CurrentUserId, "Экспорт файла в" + _selectedTable);
                MessageBox.Show("Экспорт завершён успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
