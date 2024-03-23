using BlazorStack.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services
{
    public class ApplicationAPIService
    {
        private readonly HttpClient _http;

        public ApplicationAPIService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponse?> Login(string email, string password)
        {
            var result = await _http.PostAsJsonAsync(
                "login?useCookies=false", new
                {
                    email,
                    password
                });
            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
            return response;
        }

        public async Task<UserInfo?> GetUserInfo()
        {
            var result = await _http.GetAsync("manage/info");
            return await result.Content.ReadFromJsonAsync<UserInfo>();
        }
    }
}
