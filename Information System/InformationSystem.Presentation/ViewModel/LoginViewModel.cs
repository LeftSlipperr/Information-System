using Domain.Models;
using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace InformationSystem.Presentation.ViewModel
{
    public class LoginViewModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        

        public LoginViewModel()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();

            string baseUrl = _configuration["ApiBaseUrl"] ?? "http://localhost:5000"; // ← fallback
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new Exception("Base API URL is missing in appsettings.json");
            }

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginDto = new LoginDto { Username = username, Password = password };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/User/GetUsername", content);
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Пользователь не найден.");
                    return false;
                }

                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserDto>(userJson, options);

                var decryptedPassword = EncryptionHelper.Decrypt(user.PasswordHash);
                if (decryptedPassword != password)
                {
                    MessageBox.Show("Неверный пароль.");
                    return false;
                }
                await LogActionAsync(user.UserId, "Авторизация");
                SessionManager.CurrentUserId = user.UserId;

                // Роутинг по ролям
                switch (user.Role.ToLower())
                {
                    case "admin":
                        new AdminWindow().Show();
                        break;
                    case "accountant":
                        new AccountantWindow().Show();
                        break;
                    case "storekeeper":
                        new StorekeeperWindow().Show();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль.");
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}");
                return false;
            }
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
