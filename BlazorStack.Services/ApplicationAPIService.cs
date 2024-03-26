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

        //public async Task<bool> Logout()
        //{
        //    var result = await _http.PostAsJsonAsync("account/logout", new { });
        //    return result.IsSuccessStatusCode;
        //}

        public async Task<UserInfo?> GetUserInfo()
        {
            try
            {
                var result = await _http.GetAsync("manage/info");
                return await result.Content.ReadFromJsonAsync<UserInfo>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<UserViewModel>?> GetUsers(string search = "")
        {
            try
            {
                var result = await _http.GetAsync("users");
                return await result.Content.ReadFromJsonAsync<List<UserViewModel>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserDetailsViewModel?> GetUser(string id)
        {
            try
            {
                var result = await _http.GetAsync($"users/{id}");
                return await result.Content.ReadFromJsonAsync<UserDetailsViewModel>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
