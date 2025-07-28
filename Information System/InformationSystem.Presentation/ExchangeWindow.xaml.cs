using HtmlAgilityPack;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Domain.Models;

namespace InformationSystem.Presentation
{
    /// <summary>
    /// Логика взаимодействия для ExchangeWindow.xaml
    /// </summary>
    public partial class ExchangeWindow : Window
    {
        private readonly HttpClient _httpClient = new();

        public ExchangeWindow()
        {
            InitializeComponent();
            LoadCurrencies();
        }

        private async void LoadCurrencies()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("http://cbpmr.net/infoprb.php?color=gold&info=val");
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                // Ищем таблицу с курсами
                var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'simple-little-table')]");
                if (table == null)
                {
                    MessageBox.Show("Не удалось найти таблицу с курсами.");
                    return;
                }

                var rows = table.SelectNodes(".//tr");
                if (rows == null || rows.Count < 2)
                {
                    MessageBox.Show("Не удалось найти строки в таблице.");
                    return;
                }

                var currencyList = new List<CurrencyRate>();

                foreach (var row in rows.Skip(1)) // пропускаем заголовок
                {
                    var cells = row.SelectNodes(".//td");
                    if (cells == null || cells.Count < 3)
                        continue;

                    // Флаг
                    var imgNode = cells[0].SelectSingleNode(".//img");
                    var imgSrc = imgNode?.GetAttributeValue("src", "");
                    var flagUrl = string.IsNullOrEmpty(imgSrc) ? null : "http://cbpmr.net/" + imgSrc;

                    // Название валюты
                    var nameNode = cells[1].SelectSingleNode(".//h12") ?? cells[1];
                    var name = nameNode.InnerText.Trim();

                    // Курс
                    var rateNode = cells[2].SelectSingleNode(".//h12") ?? cells[2];
                    var rateText = rateNode.InnerText.Trim().Replace(",", ".");
                    if (!decimal.TryParse(rateText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal rate))
                        continue;

                    if (name is "USD" or "EUR" or "UAH" or "RUB" or "MDL")
                    {
                        currencyList.Add(new CurrencyRate
                        {
                            Name = name,
                            Rate = rate,
                            Flag = flagUrl
                        });
                    }
                }

                CurrencyGrid.ItemsSource = currencyList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки курсов валют: " + ex.Message);
            }
        }

    }
}

