using InformationSystem.Presentation.ViewModel;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
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
using InformationSystem.Application.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;

namespace InformationSystem.Presentation.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _tableName;

        private List<object> _data;

        public StatisticsWindow(string tableName, HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
            _tableName = tableName;
            LoadDataAsync();
        }

        private string GetDisplayName(string fieldName)
        {
            return fieldName switch
            {
                "Amount" => "Сумма",
                "Cost" => "Стоимость",
                "SalaryAmount" => "Зарплата",
                "Quantity" => "Количество",
                "DebtAmount" => "Сумма долга",
                "Price" => "Цена",
                "TradePointId" => "Торговая точка",
                "WorkerId" => "Сотрудник",
                "PotentialRevenue"=> "Потенциальная прибыль",
                "TotalIncome" => "Общий доход",
                "TotalExpense" => "Общие расходы",
                "ProfitOrLoss" => "Баланс",
                "PurchaseCost" => "Себестоимость покупки",
                "Revenue" => "Выручка",
                "Expenses" => "Расходы",
                "Profit" => "Прибыль",
            };
        }
        private Type GetDataTypeByTableName(string tableName)
        {
            return tableName switch
            {
                "Income" => typeof(List<IncomeDto>),
                "Expense" => typeof(List<ExpenseDto>),
                "BalanceAnalysis" => typeof(List<BalanceAnalysisDto>),
                "DebtsReceivable" => typeof(List<DebtReceivableDto>),
                "DebtsPayable" => typeof(List<DebtPayableDto>),
                "Salary" => typeof(List<SalaryDto>),
                "RawMaterialWarehouse" => typeof(List<RawMaterialWarehouseDto>),
                "FinishedGoodsWarehouse" => typeof(List<FinishedGoodsWarehouseDto>),
                "TradePointEconomics" => typeof(List<TradePointEconomicsDto>),
                "Worker" => typeof(List<WorkerDto>),
                "DebtPayable" => typeof(List<DebtPayableDto>),
                "DebtReceivable" => typeof(List<DebtReceivableDto>),

            };
        }


        private async void LoadDataAsync()
        {
            try
            {
                string query = $"api/{_tableName}/Search";

                var dateFrom = DateFromPicker.SelectedDate?.ToString("yyyy-MM-dd");
                var dateTo = DateToPicker.SelectedDate?.ToString("yyyy-MM-dd");

                if (!string.IsNullOrEmpty(dateFrom) || !string.IsNullOrEmpty(dateTo))
                {
                    query += $"?search=&dateFrom={dateFrom}&dateTo={dateTo}";
                }

                var response = await _httpClient.GetAsync(query);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var targetType = GetDataTypeByTableName(_tableName);
                    if (targetType == null)
                    {
                        MessageBox.Show("Неизвестная таблица.");
                        return;
                    }

                    var result = JsonSerializer.Deserialize(json, targetType, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _data = (result as System.Collections.IEnumerable)?.Cast<object>().ToList();

                    PopulateParameterComboBox();
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка загрузки данных.\nКод: {response.StatusCode}\nСообщение: {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке: {ex.Message}");
            }
        }


        private void PopulateParameterComboBox()
        {
            if (_data == null || !_data.Any()) return;

            var firstItem = _data.First();
            var properties = firstItem.GetType().GetProperties();

            var numericProps = properties
                .Where(p => p.PropertyType == typeof(int) || p.PropertyType == typeof(decimal) || p.PropertyType == typeof(double))
                .Select(p => new
                {
                    Display = GetDisplayName(p.Name) ?? p.Name,  // ← используем display name
                    Field = p.Name
                })
                .ToList();

            ParameterComboBox.ItemsSource = numericProps;
            ParameterComboBox.DisplayMemberPath = "Display";
            ParameterComboBox.SelectedValuePath = "Field";
        }

        private void BuildChart_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null || ParameterComboBox.SelectedValue == null || ChartTypeComboBox.SelectedItem == null) return;

            string selectedField = ParameterComboBox.SelectedValue.ToString();
            string chartType = (ChartTypeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

            MainCartesianChart.Visibility = Visibility.Collapsed;
            MainPieChart.Visibility = Visibility.Collapsed;

            switch (chartType)
            {
                case "Column":
                    BuildColumnChart(selectedField);
                    break;
                case "Line":
                    BuildLineChart(selectedField);
                    break;
                case "Pie":
                    BuildPieChart(selectedField);
                    break;
            }
        }

        public void ApplyDateFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadDataAsync();
        }

        private string GetLabelForItem(object item)
        {
            var props = item.GetType().GetProperties();

            // Попробуем найти самые понятные поля
            var preferredFields = new[] { "Name", "Description", "Title", "TradePointName", "WorkerName", "Period", "Date", "PaymentDate",
            "DueDate", "Position", "LastUpdated"};

            foreach (var field in preferredFields)
            {
                var prop = props.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    var value = prop.GetValue(item);
                    if (value != null)
                        return value.ToString();
                }
            }

            // Если ничего не подошло — используем Id или просто "X"
            var id = props.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))?.GetValue(item);
            return id?.ToString() ?? "X";
        }


        private void BuildColumnChart(string field)
        {
            var values = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var item in _data)
            {
                var value = Convert.ToDouble(item.GetType().GetProperty(field)?.GetValue(item));
                values.Add(value);
                labels.Add(GetLabelForItem(item));
            }

            var displayName = GetDisplayName(field) ?? field;

            MainCartesianChart.Series = new SeriesCollection
{
                new ColumnSeries
                {
                    Title = displayName,
                    Values = values
                }
            };

            MainCartesianChart.AxisY.Add(new Axis
            {
                Title = displayName
            });


            MainCartesianChart.AxisX.Clear();
            MainCartesianChart.AxisX.Add(new Axis
            {
                Title = "Запись",
                Labels = labels
            });

            MainCartesianChart.AxisY.Clear();
            MainCartesianChart.AxisY.Add(new Axis
            {
                Title = field
            });

            MainCartesianChart.Visibility = Visibility.Visible;
        }

        private void BuildLineChart(string field)
        {
            var values = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var item in _data)
            {
                var value = Convert.ToDouble(item.GetType().GetProperty(field)?.GetValue(item));
                values.Add(value);
                labels.Add(GetLabelForItem(item));
            }
            var displayName = GetDisplayName(field) ?? field;

            MainCartesianChart.Series = new SeriesCollection
{
                new LineSeries
                {
                    Title = displayName,
                    Values = values
                }
            };

            MainCartesianChart.AxisY.Add(new Axis
            {
                Title = displayName
            });


            MainCartesianChart.AxisY.Clear();
            MainCartesianChart.AxisY.Add(new Axis
            {
                Title = field
            });

            MainCartesianChart.Visibility = Visibility.Visible;
        }

        private void BuildPieChart(string field)
        {
            MainPieChart.Series = new SeriesCollection();

            foreach (var item in _data)
            {
                var value = Convert.ToDouble(item.GetType().GetProperty(field)?.GetValue(item));
                var label = GetLabelForItem(item);

                MainPieChart.Series.Add(new PieSeries
                {
                    Title = label,
                    Values = new ChartValues<double> { value },
                    DataLabels = true
                });
            }

            MainPieChart.Visibility = Visibility.Visible;
        }

        private void ChartTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_data == null || !_data.Any())
                return;

            string chartType = (ChartTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            var firstItem = _data.First();
            var properties = firstItem.GetType().GetProperties();

            // Определим числовые поля
            var numericProps = properties
                .Where(p => p.PropertyType == typeof(int) || p.PropertyType == typeof(double) || p.PropertyType == typeof(decimal))
                .ToList();

            // Pie-график требует категоризированных данных — например, расходов по категориям.
            bool isPieEnabled = numericProps.Count == 1 || (numericProps.Count > 0 && chartType == "Pie");

            // Обновим список параметров в ComboBox
            var displayList = numericProps.Select(p => new
            {
                Display = GetDisplayName(p.Name) ?? p.Name,  // ← человекочитаемое имя
                Field = p.Name
            }).ToList();

            ParameterComboBox.ItemsSource = displayList;
            ParameterComboBox.DisplayMemberPath = "Display";
            ParameterComboBox.SelectedValuePath = "Field";


            ParameterComboBox.ItemsSource = displayList;

            if (displayList.Any())
                ParameterComboBox.SelectedIndex = 0;

            // Скрытие/отображение PieChart по условиям
            MainPieChart.Visibility = chartType == "Pie" ? Visibility.Visible : Visibility.Collapsed;
            MainCartesianChart.Visibility = chartType != "Pie" ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
