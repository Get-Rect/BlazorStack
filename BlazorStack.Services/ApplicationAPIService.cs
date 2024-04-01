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

        public async Task<ApplicationResponse<LoginResponse>?> Login(string email, string password)
        {
            var result = await _http.PostAsJsonAsync(
                "login?useCookies=false", new
                {
                    email,
                    password
                });
            var test = await result.Content.ReadAsStringAsync();
            var response = await result.Content.ReadFromJsonAsync<ApplicationResponse<LoginResponse>>();
            return response;
        }

        //public async Task<bool> Logout()
        //{
        //    var result = await _http.PostAsJsonAsync("account/logout", new { });
        //    return result.IsSuccessStatusCode;
        //}

        public async Task<ApplicationResponse<List<string>>?> GetAllRoles()
        {
            var response = await _http.GetAsync("users/allroles");
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<List<string>>>();
            return content;
        }

        public async Task<bool> CreateUser(UserViewModel newUser)
        {
            try
            {
                var result = await _http.PostAsJsonAsync("users", newUser);
                return result.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

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

        public async Task<List<RoleClaim>?> GetRoles()
        {
            try
            {
                var result = await _http.GetAsync("account/roles");
                return await result.Content.ReadFromJsonAsync<List<RoleClaim>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationResponse<List<UserViewModel>>?> GetUsers(string search = "")
        {
            try
            {
                var result = await _http.GetAsync("users");
                return await result.Content.ReadFromJsonAsync<ApplicationResponse<List<UserViewModel>>?>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationResponse<UserDetailsViewModel>?> GetUser(string id)
        {
            var response = await _http.GetAsync($"users/{id}");
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<UserDetailsViewModel>?>();
            return content;
        }

        public async Task<ApplicationResponse<bool?>?> ChangePassword(string id, string newPassword)
        {
            var response = await _http.PostAsJsonAsync($"users/change-password/{id}", new ChangePasswordRequest { newPassword = newPassword });
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
            return content;
        }

        public async Task<ApplicationResponse<bool?>?> UpdateRole(string id, string newRole)
        {
            var response = await _http.PostAsJsonAsync($"users/update-role/{id}", new UpdateRoleRequest { NewRole = newRole });
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
            return content;
        }

        public async Task<ApplicationResponse<bool?>?> DeleteUser(string id)
        {
            var response = await _http.DeleteAsync($"users/{id}");
            var test = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
            return content;
        }
    }
}
