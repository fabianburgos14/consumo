using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumoApi
{
    public partial class MainPage : ContentPage
    {
        private const string apiUrl = "https://fakeapi.platzi.com/";
        private readonly LoginService _loginService;

        public MainPage()
        {
            InitializeComponent();
            _loginService = new LoginService();
            CheckSession();
        }

        private async void CheckSession()
        {
            if (await _loginService.IsSessionActive())
            {
                LoadData();
            }
            else
            {
                await DisplayAlert("Session", "Please log in to continue.", "OK");
            }
        }

        private async void LoadData()
        {
            try
            {
                var products = await GetApiData<List<Product>>("products");
                if (products != null)
                    productListView.ItemsSource = products;

                var categories = await GetApiData<List<Category>>("categories");
                if (categories != null)
                    categoryListView.ItemsSource = categories;

                var users = await GetApiData<List<User>>("users");
                if (users != null)
                    userListView.ItemsSource = users;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async Task<T> GetApiData<T>(string endpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{apiUrl}{endpoint}");
                response.EnsureSuccessStatusCode();
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(jsonContent);
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = usernameEntry.Text;
            var password = passwordEntry.Text;

            if (await _loginService.Login(username, password))
            {
                await DisplayAlert("Success", "Login successful", "OK");
                LoadData();
            }
            else
            {
                await DisplayAlert("Error", "Login failed", "OK");
            }
        }

        private void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            _loginService.Logout();
            DisplayAlert("Logout", "You have been logged out.", "OK");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}