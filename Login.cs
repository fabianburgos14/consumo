using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumoApi
{
    public class LoginService
    {
        private const string apiUrl = "https://fakeapi.platzi.com/";
        private readonly HttpClient _httpClient;

        public LoginService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> Login(string username, string password)
        {
            var loginData = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{apiUrl}auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<LoginResponse>(jsonResponse)?.Token;
                if (!string.IsNullOrEmpty(token))
                {
                    await SecureStorage.SetAsync("auth_token", token);
                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            SecureStorage.Remove("auth_token");
        }

        public async Task<bool> IsSessionActive()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        private class LoginResponse
        {
            public string? Token { get; set; }
        }
    }
}
