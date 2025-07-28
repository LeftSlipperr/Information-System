using InformationSystem.Application.DTO;
using InformationSystem.Application.Interfaces;
using InformationSystem.Application.Services;
using InformationSystem.Domain.Models;
using InformationSystem.Presentation.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace InformationSystem.Presentation
{
    public partial class MainWindow : Window
    {
        private bool isPasswordVisible = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Скрываем текст, показываем точки
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;

                EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/eye_closed.png"));
            }
            else
            {
                // Показываем текст, скрываем PasswordBox
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;

                EyeIcon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/eye_open.png"));
                PasswordTextBox.Focus();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;
            }

            isPasswordVisible = !isPasswordVisible;
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new LoginViewModel();

            string password = isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
            bool isAuthenticated = await viewModel.LoginAsync(UsernameTextBox.Text, password);

            if (isAuthenticated)
            {
                //new AdminWindow().Show();
                Close();
            }
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
    }
}
