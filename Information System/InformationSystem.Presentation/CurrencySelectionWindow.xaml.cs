using System;
using System.Collections.Generic;
using System.Linq;
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

namespace InformationSystem.Presentation
{
    /// <summary>
    /// Логика взаимодействия для CurrencySelectionWindow.xaml
    /// </summary>
    public partial class CurrencySelectionWindow : Window
    {
        public static readonly Dictionary<string, decimal> CurrencyRates = new()
        {
            {"RUP", 1},
            { "USD", 16.1m },
            { "EUR", 18.5198m },
            { "RUB", 0.1998m },
            { "UAH", 0.3881m },
            { "MDL", 0.9415m }
        };

        // Свойства, которые можно использовать после закрытия окна
        public string SelectedCurrency { get; private set; }
        public decimal SelectedRate { get; private set; }
        public CurrencySelectionWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (CurrencyComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string currency = selectedItem.Content.ToString();
                if (CurrencyRates.TryGetValue(currency, out decimal rate))
                {
                    SelectedCurrency = currency;
                    SelectedRate = rate;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Курс для выбранной валюты не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Выберите валюту.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}