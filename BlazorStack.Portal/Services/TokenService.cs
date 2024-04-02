using Blazored.LocalStorage;
using BlazorStack.Services.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorStack.Portal.Services
{
    public class TokenService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public TokenService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<LoginResponse?> GetToken()
        {
            var token = await GetTokenFromLocalStorage();
            if (token is null || token.Issued is null) return null;

            if(token.Issued.Value.AddMinutes(55) <= DateTime.UtcNow)
            {
                token = await RefreshToken(token.RefreshToken);
                await _localStorage.SetItemAsync("token", token);
            }
            return token;
        }

        private async Task<LoginResponse?> RefreshToken(string refreshToken)
        {
            var response = await _http.PostAsJsonAsync("refresh", new
            {
                refreshToken,
            });
            var token = await response.Content.ReadFromJsonAsync<ApplicationResponse<LoginResponse>>();
            token.Data.Issued = DateTime.UtcNow;
            return token.Data;
        }

        private async Task<LoginResponse?> GetTokenFromLocalStorage()
        {
            var token = await _localStorage.GetItemAsStringAsync("token") ?? string.Empty;
            if (string.IsNullOrEmpty(token)) return null;
            var response = JsonSerializer.Deserialize<LoginResponse>(token);
            return response;
        }
    }
}
