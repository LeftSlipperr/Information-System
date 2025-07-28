using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using InformationSystem.Application.DTO;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Windows;
using System.Net.Http.Json;
using InformationSystem.Application.Interfaces;

public class AdminViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IConfiguration _configuration;


    public string SelectedTable { get; set; }
    public ObservableCollection<object> TableData { get; set; } = new();
    public ObservableCollection<TradePointDto> TradePoints { get; set; }
    public TradePointDto SelectedTradePoint { get; set; }

    public object? SelectedRecord { get; set; }

    public Dictionary<string, Type> _tableTypeMap = new()
    {
        { "Expense", typeof(ExpenseDto) },
        { "Income", typeof(IncomeDto) },
        { "BalanceAnalysis", typeof(BalanceAnalysisDto) },
        { "DebtReceivable", typeof(DebtReceivableDto) },
        { "DebtPayable", typeof(DebtPayableDto) },
        { "RawMaterialWarehouse", typeof(RawMaterialWarehouseDto) },
        { "FinishedGoodsWarehouse", typeof(FinishedGoodsWarehouseDto) },
        { "Worker", typeof(WorkerDto) },
        { "Salary", typeof(SalaryDto) },
        { "TradePoint", typeof(TradePointDto) },
        { "TradePointEconomics", typeof(TradePointEconomicsDto) },
        { "Log", typeof(LogDto) },
        { "User", typeof(UserDto) }
    };

    private readonly Dictionary<string, string> _tableEndpointMap = new()
    {
        { "Expense", "api/Expense/Search" },
        { "Income", "api/Income/Search" },
        { "BalanceAnalysis", "api/BalanceAnalysis/Search" },
        { "DebtReceivable", "api/DebtReceivable/Search" },
        { "DebtPayable", "api/DebtPayable/Search" },
        { "RawMaterialWarehouse", "api/RawMaterialWarehouse/Search" },
        { "FinishedGoodsWarehouse", "api/FinishedGoodsWarehouse/Search" },
        { "Worker", "api/Worker/Search" },
        { "Salary", "api/Salary/Search" },
        { "TradePoint", "api/TradePoint/Search" },
        { "TradePointEconomics", "api/TradePointEconomics/Search" },
        { "Log", "api/Log/Search" },
        { "User", "api/User/Search" }
    };


    public AdminViewModel(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task LoadTableAsync(string tableName)
    {
        SelectedTable = tableName;

        if (!_tableTypeMap.TryGetValue(tableName, out var modelType))
            throw new Exception($"Тип для таблицы '{tableName}' не найден.");

        var data = await GetAllAsync(modelType, tableName);

        if (tableName == "TradePointEconomics")
        {
            // Загружаем список торговых точек
            var tradePointsResponse = await _httpClient.GetAsync("api/TradePoint/GetAllTradePoints");
            tradePointsResponse.EnsureSuccessStatusCode();
            var tradePointsJson = await tradePointsResponse.Content.ReadAsStringAsync();
            var tradePoints = JsonSerializer.Deserialize<List<TradePointDto>>(tradePointsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();

            foreach (var item in data.OfType<TradePointEconomicsDto>())
            {
                var point = tradePoints.FirstOrDefault(tp => tp.TradePointId == item.TradePointId);
                item.TradePointName = point?.Name ?? "Неизвестно";
            }

        }

        TableData.Clear();
        foreach (var item in data)
            TableData.Add(item);

        OnPropertyChanged(nameof(TableData));
    }

    public async Task AddRecordAsync(object record)
    {
        if (string.IsNullOrEmpty(SelectedTable))
            throw new Exception("Не выбрана таблица");

        // Убедимся, что мы сериализуем уже модифицированный объект
        if (SelectedTable == "TradePointEconomics" && record is TradePointEconomicsDto dto)
        {
            if (SelectedTradePoint != null)
                dto.TradePointId = SelectedTradePoint.TradePointId;

            var point = TradePoints?.FirstOrDefault(tp => tp.TradePointId == dto.TradePointId);
            dto.TradePointName = point?.Name ?? "Неизвестно";

            // ❗ сериализуем именно dto, не record
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/{SelectedTable}", content);
            response.EnsureSuccessStatusCode();

            TableData.Add(dto);
        }
        else
        {
            var json = JsonSerializer.Serialize(record, record.GetType());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/{SelectedTable}", content);
            response.EnsureSuccessStatusCode();

            TableData.Add(record);
        }

        OnPropertyChanged(nameof(TableData));
    }

    public async Task EditRecordAsync(object record)
    {
        if (string.IsNullOrEmpty(SelectedTable)) throw new Exception("Не выбрана таблица");

        var idProp = GetIdProperty(record);
        var id = idProp?.GetValue(record)?.ToString();

        if (string.IsNullOrWhiteSpace(id)) throw new Exception("Id пустой");
        if (SelectedTable == "TradePointEconomics" && record is TradePointEconomicsDto dto)
        {
            var point = TradePoints?.FirstOrDefault(tp => tp.TradePointId == dto.TradePointId);
            dto.TradePointName = point?.Name ?? "Неизвестно";
        }

        var json = JsonSerializer.Serialize(record, record.GetType());
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"api/{SelectedTable}/{id}";
        Console.WriteLine($"PATCH URL: {url}");
        Console.WriteLine($"Payload: {json}");

        try
        {
            var response = await _httpClient.PatchAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка при PATCH-запросе: {response.StatusCode} - {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Исключение при PATCH: {ex.Message}");
            throw;
        }
    }

    public async Task SearchTableAsync(string tableName, string search, DateTime? dateFrom, DateTime? dateTo)
    {
        if (!_tableEndpointMap.TryGetValue(tableName, out string endpoint))
            throw new InvalidOperationException("Неизвестная таблица");

        if (!_tableTypeMap.TryGetValue(tableName, out Type dtoType))
            throw new InvalidOperationException("Неизвестный тип DTO для таблицы");

        string url = $"{endpoint}?";

        if (!string.IsNullOrEmpty(search))
            url += $"search={Uri.EscapeDataString(search)}&";

        if (dateFrom.HasValue)
            url += $"dateFrom={dateFrom:yyyy-MM-dd}&";

        if (dateTo.HasValue)
            url += $"dateTo={dateTo:yyyy-MM-dd}&";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var listType = typeof(List<>).MakeGenericType(dtoType);
        var result = JsonSerializer.Deserialize(json, listType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        TableData = new ObservableCollection<object>((IEnumerable<object>)result!);
    }


    public async Task DeleteRecordAsync(object record, string tableName)
    {
        if (record == null || string.IsNullOrEmpty(SelectedTable)) return;

        var idProperty = record.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

        if (idProperty == null) return;

        var idValue = idProperty.GetValue(record);
        if (idValue == null) return;

        try
        {
            var url = $"api/{SelectedTable}?guid={idValue}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();

            TableData.Remove(record);
            OnPropertyChanged(nameof(TableData));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
        }
    }



    private PropertyInfo? GetIdProperty(object obj)
    {
        return obj.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<List<object>> GetAllAsync(Type type, string tableName)
    {
        var response = await _httpClient.GetAsync($"api/{tableName}/GetAll{tableName}s");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var listType = typeof(List<>).MakeGenericType(type);
        var result = JsonSerializer.Deserialize(json, listType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return ((IEnumerable<object>)result).ToList();
    }
}