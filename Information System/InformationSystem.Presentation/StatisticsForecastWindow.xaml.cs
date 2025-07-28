using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InformationSystem.Application.DTO;
using Microsoft.Extensions.Configuration;
using Axis = LiveCharts.Wpf.Axis;
using Separator = LiveCharts.Wpf.Separator;
using InformationSystem.Presentation.ViewModel;

namespace InformationSystem.Presentation
{
    public partial class StatisticsForecastWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private List<QuarterRevenue> revenues = new();

        public StatisticsForecastWindow()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            string baseUrl = "http://localhost:5000"; // ← fallback
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

            _ = LoadDataAsync();
        }

        public class QuarterRevenue
        {
            public int Year { get; set; }
            public int QuarterNumber { get; set; }
            public bool IsPredicted { get; set; }
            public string Quarter => $"Q{QuarterNumber} {Year}" + (IsPredicted ? " (прогноз)" : "");

            public double Revenue { get; set; }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/BalanceAnalysis/GetAllBalanceAnalysiss");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Не удалось загрузить данные.");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<BalanceAnalysisDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var grouped = list!
                    .Where(b => b.Period != default)
                    .Select(b => new { b.Period, Revenue = (double)(b.TotalIncome - b.TotalExpense) })
                    .GroupBy(b => new { Quarter = ((b.Period.Month - 1) / 3) + 1, Year = b.Period.Year })
                    .Select(g => new QuarterRevenue
                    {
                        QuarterNumber = g.Key.Quarter,
                        Year = g.Key.Year,
                        Revenue = g.Sum(x => x.Revenue)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.QuarterNumber)
                    .ToList();


                var now = DateTime.Now;
                int currentQuarter = (now.Month - 1) / 3 + 1;
                int currentYear = now.Year;

                // Прошлый
                int prevQuarter = currentQuarter - 1;
                int prevYear = currentYear;
                if (prevQuarter == 0)
                {
                    prevQuarter = 4;
                    prevYear--;
                }

                // Следующий
                int nextQuarter = currentQuarter + 1;
                int nextYear = currentYear;
                if (nextQuarter > 4)
                {
                    nextQuarter = 1;
                    nextYear++;
                }

                // Получаем только нужные 2 квартала
                var filtered = grouped.Where(x =>
                    (x.Year == prevYear && x.QuarterNumber == prevQuarter) ||
                    (x.Year == currentYear && x.QuarterNumber == currentQuarter)
                ).OrderBy(x => x.Year).ThenBy(x => x.QuarterNumber).ToList();

                // Предсказание
                var prediction = PredictNextQuarterLinear(filtered.Select((r, i) => (i, r.Revenue)).ToList());
                filtered.Add(new QuarterRevenue
                {
                    Year = nextYear,
                    QuarterNumber = nextQuarter,
                    Revenue = prediction,
                    IsPredicted = true
                });

                revenues = filtered;
                BuildChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }


        private double PredictNextQuarterLinear(List<(int Index, double Value)> data)
        {
            if (data.Count < 2)
                return data.FirstOrDefault().Value; // не предсказываем

            double xAvg = data.Average(d => d.Index);
            double yAvg = data.Average(d => d.Value);

            double numerator = data.Sum(d => (d.Index - xAvg) * (d.Value - yAvg));
            double denominator = data.Sum(d => Math.Pow(d.Index - xAvg, 2));

            double slope = denominator == 0 ? 0 : numerator / denominator;
            double intercept = yAvg - slope * xAvg;

            double predicted = slope * data.Count + intercept;
            return predicted;
        }


        private void BuildChart_Click(object sender, RoutedEventArgs e) => BuildChart();

        private void BuildChart()
        {
            var selectedItem = ChartTypeComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string chartType = selectedItem.Content.ToString();

            RevenueChart.Series.Clear();
            RevenueChart.AxisX.Clear();
            RevenueChart.AxisY.Clear();

            var labels = revenues.Select(r => r.Quarter).ToArray();

            if (chartType == "Линейная")
            {
                RevenueChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Выручка",
                Values = new ChartValues<double>(revenues.Select(r => r.Revenue)),
                PointGeometry = DefaultGeometries.Circle,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                LineSmoothness = 0.3
            }
        };
            }
            else
            {
                var actualValues = new ChartValues<double>();
                var predictedValues = new ChartValues<double>();

                for (int i = 0; i < revenues.Count; i++)
                {
                    var r = revenues[i];
                    if (r.IsPredicted)
                    {
                        // Только на этой позиции добавляем прогноз
                        predictedValues.Add(r.Revenue);
                        actualValues.Add(0); // для выравнивания
                    }
                    else
                    {
                        actualValues.Add(r.Revenue);
                        predictedValues.Add(0); // ноль для пустой
                    }
                }

                RevenueChart.Series = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Факт",
                Values = actualValues,
                Fill = Brushes.SkyBlue,
                Stroke = Brushes.SteelBlue,
                DataLabels = true,
                MaxColumnWidth = 40
            },
            new ColumnSeries
            {
                Title = "Прогноз",
                Values = predictedValues,
                Fill = Brushes.LightGreen,
                Stroke = Brushes.DarkGreen,
                DataLabels = true,
                MaxColumnWidth = 40
            }
        };
            }

            RevenueChart.AxisX.Add(new Axis
            {
                Labels = labels,
                Separator = new Separator { Step = 1, IsEnabled = false }
            });

            RevenueChart.AxisY.Add(new Axis
            {
                Title = "Сумма, Руб"
            });
        }



    }
    public class ColoredRevenue
    {
        public string Label { get; set; }
        public double Value { get; set; }
        public Brush Color { get; set; }
    }

}