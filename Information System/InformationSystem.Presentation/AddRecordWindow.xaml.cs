using InformationSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace InformationSystem.Presentation
{
    public partial class AddRecordWindow : Window
    {
        private readonly Type _tableType;
        private readonly string _tableName;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly Dictionary<string, Control> _inputControls = new();
        private List<TradePointDto> _tradePoints = new();
        public ObservableCollection<TradePointDto> TradePoints { get; set; }
        public TradePointDto SelectedTradePoint { get; set; }
        private readonly bool _isEditMode;
        private readonly object? _recordToEdit;
        private readonly List<string> _excludedProperties = new()
        {
            "UserId",
            "IncomeId",
            "ExpenseId",
            "BalanceId",
            "ProfitOrLoss",
            "DebtPayableId",
            "DebtReceivableId",
            "ProductId",
            "MaterialId",
            "TradePointEconomicsId",
            "WorkerId",

        };


        public object CreatedObject { get; private set; }

        public AddRecordWindow(Type tableType, string tableName, HttpClient httpClient, string baseUrl, object recordToEdit = null)
        {
            InitializeComponent();
            _tableType = tableType;
            _tableName = tableName;
            _httpClient = httpClient;
            _baseUrl = "http://localhost:5000/api";
            Loaded += AddRecordWindow_Loaded;
            _isEditMode = recordToEdit != null;
            _recordToEdit = recordToEdit;

            if (_isEditMode)
            {
                FillFormFields();
            }

        }

        private async void AddRecordWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tableName == "TradePointEconomics")
            {
                await LoadTradePointsAsync();
            }
            GenerateFormFields();

        }

        private void FillFormFields()
        {
            if (_recordToEdit == null) return;

            var idProperty = _tableType.GetProperty("TradePointId");
            if (_isEditMode && idProperty != null)
            {
                if (_recordToEdit is TradePointEconomicsDto economicsDto &&
                    _inputControls.TryGetValue("TradePointId", out var control) &&
                    control is ComboBox combo)
                {
                    combo.SelectedValue = economicsDto.TradePointId;
                }
            }

            foreach (var property in _tableType.GetProperties())
            {

                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    continue; // Уже заполнили Id выше

                if (!_inputControls.ContainsKey(property.Name)) continue;

                var value = property.GetValue(_recordToEdit);
                var control = _inputControls[property.Name];

                switch (control)
                {
                    case TextBox textBox:
                        textBox.Text = value?.ToString() ?? "";
                        break;
                    case DatePicker datePicker:
                        if (value is DateTime dt)
                            datePicker.SelectedDate = dt;
                        break;
                    case ComboBox comboBox:
                        if (_tableName == "TradePointEconomics")
                        {
                            var combo = new ComboBox
                            {
                                ItemsSource = _tradePoints,
                                DisplayMemberPath = "Name",
                                SelectedValuePath = "TradePointId",
                                Margin = new Thickness(0, 0, 0, 10)
                            };
                            _inputControls["TradePointId"] = combo;
                            formPanel.Children.Add(combo);
                        }
                        else
                        {
                            comboBox.SelectedValue = value;
                        }
                        break;

                }
            }
        }


        private async Task LoadTradePointsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/TradePoint/GetAllTradePoints");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                _tradePoints = JsonSerializer.Deserialize<List<TradePointDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<TradePointDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки торговых точек: {ex.Message}");
            }
        }

        private void GenerateFormFields()
        {
            var properties = _tableType.GetProperties()
                .Where(p =>
                    p.CanWrite &&
                    (!_excludedProperties.Contains(p.Name)) &&
                    (_tableName == "TradePointEconomics" || p.Name != "TradePointId") && // Исключить TradePointId, кроме TradePointEconomics
                    (!p.PropertyType.IsClass || p.PropertyType == typeof(string)))
                .ToList();

            // ✅ Добавляем скрытое поле Id при редактировании
            if (_isEditMode)
            {
                var idProperty = _tableType.GetProperty("Id");
                if (idProperty != null)
                {
                    TextBox hiddenIdBox = new TextBox
                    {
                        Visibility = Visibility.Collapsed,
                        Text = idProperty.GetValue(_recordToEdit)?.ToString() ?? "0"
                    };
                    _inputControls["Id"] = hiddenIdBox;
                    formPanel.Children.Add(hiddenIdBox);
                }
            }

            foreach (var property in properties)
            {
                if (_tableName == "TradePointEconomics" &&
                    (property.Name == "Profit" || property.Name == "Name" || property.Name == "TradePointName"))
                    continue;
                var readOnlyAttr = property.GetCustomAttribute<ReadOnlyAttribute>();
                if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                    continue;

                string labelText = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;

                TextBlock label = new TextBlock
                {
                    Text = labelText,
                    Margin = new Thickness(0, 0, 0, 5),
                    FontWeight = FontWeights.SemiBold
                };
                formPanel.Children.Add(label);

                Control inputControl;

                if (_tableName == "TradePointEconomics" && property.Name == "TradePointId")
                {
                    var comboBox = new ComboBox
                    {
                        ItemsSource = _tradePoints,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "TradePointId",
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    inputControl = comboBox;
                }
                else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    inputControl = new DatePicker
                    {
                        SelectedDate = DateTime.Now,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                }
                else
                {
                    inputControl = new TextBox
                    {
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                }

                _inputControls[property.Name] = inputControl;
                formPanel.Children.Add(inputControl);
            }
            if (_tableName == "FinishedGoodsWarehouse")
            {
                // Цена за единицу
                AddCustomField("UnitPrice", "Цена за единицу", new TextBox());

                // Единица измерения (ComboBox)
                var unitComboBox = new ComboBox
                {
                    ItemsSource = new List<string> { "кг", "литры", "тонны", "штуки", "м³" },
                    SelectedIndex = 0,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                AddCustomField("Unit", "Единица измерения", unitComboBox);

                // Чекбокс НДС
                var vatCheckBox = new CheckBox
                {
                    Content = "Учитывать НДС (20%)",
                    Margin = new Thickness(0, 0, 0, 10)
                };
                _inputControls["IncludeVAT"] = vatCheckBox;
                formPanel.Children.Add(vatCheckBox);
            }

            void AddCustomField(string key, string label, Control inputControl)
            {
                formPanel.Children.Add(new TextBlock
                {
                    Text = label,
                    Margin = new Thickness(0, 0, 0, 5),
                    FontWeight = FontWeights.SemiBold
                });

                inputControl.Margin = new Thickness(0, 0, 0, 10);
                _inputControls[key] = inputControl;
                formPanel.Children.Add(inputControl);
            }


            if (_isEditMode)
            {
                FillFormFields();
            }
        }
        private string GetDisplayName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var instance = Activator.CreateInstance(_tableType);
                if (_isEditMode)
                {
                    var idProperty = _tableType.GetProperty("Id");
                    if (idProperty != null && _inputControls.TryGetValue("Id", out var idControl) && idControl is TextBox idTextBox)
                    {
                        if (int.TryParse(idTextBox.Text, out int parsedId))
                        {
                            idProperty.SetValue(instance, parsedId);
                        }
                    }
                }

                foreach (var property in _tableType.GetProperties())
                {
                    // Не устанавливаем Id только при создании
                    if (!_isEditMode && property.Name == "Id")
                        continue;


                    if (_inputControls.TryGetValue(property.Name, out var control))
                    {
                        object? value = null;

                        if (control is TextBox textBox)
                        {
                            var text = textBox.Text;

                            if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                            {
                                if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out var decValue))
                                {
                                    string displayName = GetDisplayName(property);
                                    MessageBox.Show($"Неверный формат числа в поле \"{displayName}\".", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                value = decValue;
                            }
                            else
                            {
                                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                var converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
                                if (converter != null && converter.IsValid(text))
                                {
                                    value = converter.ConvertFromString(null, CultureInfo.CurrentCulture, text);
                                }
                                else
                                {
                                    throw new InvalidCastException($"Невозможно преобразовать значение \"{text}\" в {targetType.Name}");
                                }
                            }
                        }
                        else if (control is DatePicker datePicker)
                        {
                            if (control is DatePicker dp && dp.SelectedDate.HasValue)
                            {
                                if (dp.SelectedDate.Value > DateTime.Today.AddYears(5) || dp.SelectedDate.Value < DateTime.Today)
                                {
                                    MessageBox.Show("Дата не может быть позже чем через 5 лет. Или раньше сегодняшней", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                else
                                {
                                    value = datePicker.SelectedDate;
                                }
                            }

                        }
                        else if (control is ComboBox comboBox)
                        {
                            value = comboBox.SelectedValue;
                        }

                        property.SetValue(instance, value);
                    }
                }
                if (_tableName == "FinishedGoodsWarehouse")
                {
                    if (_inputControls.TryGetValue("Quantity", out var qtyBox) &&
                        _inputControls.TryGetValue("UnitPrice", out var priceBox) &&
                        _inputControls.TryGetValue("IncludeVAT", out var vatCheckBox))
                    {
                        int.TryParse((qtyBox as TextBox)?.Text, out int quantity);
                        decimal.TryParse((priceBox as TextBox)?.Text, out decimal unitPrice);
                        bool includeVat = (vatCheckBox as CheckBox)?.IsChecked ?? false;

                        decimal profit = quantity * unitPrice;
                        if (includeVat)
                        {
                            profit *= 1.2m; // НДС 20%
                        }

                        var profitProperty = _tableType.GetProperty("PotentialRevenue");
                        if (profitProperty != null)
                        {
                            profitProperty.SetValue(instance, profit);
                        }
                    }
                }

                HttpResponseMessage response;
                if (_isEditMode && _recordToEdit != null)
                {
                    var id = GetObjectId(_recordToEdit);
                    var json = JsonSerializer.Serialize(instance, instance.GetType());
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PatchAsync($"{_baseUrl}/{_tableName}/{id}", content);
                }
                else
                {
                    response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{_tableName}", instance);
                }

                if (response.IsSuccessStatusCode)
                {
                    CreatedObject = instance;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Ошибка при сохранении: {response.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private object? GetObjectId(object obj)
        {
            // Поиск свойства с именем "Id"
            var idProp = obj.GetType().GetProperty("Id");

            // Если не нашли — пробуем первое свойство, заканчивающееся на "Id"
            if (idProp == null)
            {
                idProp = obj.GetType().GetProperties()
                    .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
            }

            return idProp?.GetValue(obj);
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

