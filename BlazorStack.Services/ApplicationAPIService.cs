using BlazorStack.Data.Models;
using BlazorStack.Services.Models;
using BlazorStack.Services.Models.Requests;
using BlazorStack.Services.Models.Responses;
using BlazorStack.Services.Models.ViewModels;
using System.Net.Http.Json;

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
            var response = await result.Content.ReadFromJsonAsync<ApplicationResponse<LoginResponse>>();
            return response;
        }

        public async Task<ApplicationResponse<LoginResponse>?> RefreshToken(string refreshToken)
        {
            var result = await _http.PostAsJsonAsync(
                "refresh", new
                {
                    refreshToken
                });
            var response = await result.Content.ReadFromJsonAsync<ApplicationResponse<LoginResponse>>();
            return response;
        }

        public async Task<bool> Logout()
        {
            var result = await _http.PostAsJsonAsync("account/logout", new { });
            return result.IsSuccessStatusCode;
        }

        public async Task<ApplicationResponse<List<string>>?> GetAllRoles()
        {
            try
            {
                var response = await _http.GetAsync("users/allroles");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<List<string>>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<List<string>>(ex);
            }
        }

        public async Task<ApplicationResponse<bool?>?> CreateUser(UserViewModel newUser)
        {
            var response = await _http.PostAsJsonAsync("users", newUser);
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
            return content;
        }

        public async Task<ApplicationResponse<UserInfo>?> GetUserInfo()
        {
            try
            {
                var result = await _http.GetAsync("manage/info");
                return await result.Content.ReadFromJsonAsync<ApplicationResponse<UserInfo>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationResponse<ApplicationUser>?> GetUserAdditionalInfo()
        {
            try
            {
                var result = await _http.GetAsync("account/me");
                return await result.Content.ReadFromJsonAsync<ApplicationResponse<ApplicationUser>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ApplicationResponse<List<RoleClaim>>?> GetRoles()
        {
            try
            {
                var result = await _http.GetAsync("account/roles");
                return await result.Content.ReadFromJsonAsync<ApplicationResponse<List<RoleClaim>>>();
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<List<RoleClaim>>(ex);
            }
        }

        public async Task<ApplicationResponse<List<UserViewModel>>?> GetUsers(string search = "")
        {
            try
            {
                var result = await _http.GetAsync("users");
                return await result.Content.ReadFromJsonAsync<ApplicationResponse<List<UserViewModel>>?>();
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<List<UserViewModel>>(ex);
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
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
            return content;
        }

        public async Task<ApplicationResponse<PhotoUploadResponse>?> UploadProfilePhoto(string userId, string base64)
        {
            var response = await _http.PostAsJsonAsync($"users/{userId}/upload-profile-photo", new UploadRequest { Base64 = base64 });
            var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<PhotoUploadResponse>>();
            return content;
        }
    }
}
