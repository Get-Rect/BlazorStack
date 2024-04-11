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
            try
            {
                var response = await _http.PostAsJsonAsync(
                        "login?useCookies=false", new
                        {
                            email,
                            password
                        });
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<LoginResponse>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<LoginResponse>(ex);
            }
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
            try
            {
                var response = await _http.PostAsJsonAsync("users", newUser);
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<bool?>(ex);
            }
        }

        public async Task<ApplicationResponse<UserInfo>?> GetUserInfo()
        {
            try
            {
                var response = await _http.GetAsync("manage/info");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<UserInfo>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<UserInfo>(ex);
            }
        }

        public async Task<ApplicationResponse<UserInfo>?> GetUserAdditionalInfo()
        {
            try
            {
                var response = await _http.GetAsync("account/me");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<UserInfo>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<UserInfo>(ex);
            }
        }

        public async Task<ApplicationResponse<List<RoleClaim>>?> GetRoles()
        {
            try
            {
                var response = await _http.GetAsync("account/roles");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<List<RoleClaim>>>();
                return content;
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
                var response = await _http.GetAsync("users");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<List<UserViewModel>>?>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<List<UserViewModel>>(ex);
            }
        }

        public async Task<ApplicationResponse<UserDetailsViewModel>?> GetUser(string id)
        {
            try
            {
                var response = await _http.GetAsync($"users/{id}");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<UserDetailsViewModel>?>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<UserDetailsViewModel>(ex);
            }
        }

        public async Task<ApplicationResponse<bool?>?> ChangePassword(string id, string newPassword)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"users/change-password/{id}", new ChangePasswordRequest { newPassword = newPassword });
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<bool?>(ex);
            }
        }

        public async Task<ApplicationResponse<bool?>?> UpdateRole(string id, string newRole)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"users/update-role/{id}", new UpdateRoleRequest { NewRole = newRole });
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<bool?>(ex);
            }
        }

        public async Task<ApplicationResponse<bool?>?> DeleteUser(string id)
        {
            try
            {
                var response = await _http.DeleteAsync($"users/{id}");
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<bool?>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<bool?>(ex);
            }
        }

        public async Task<ApplicationResponse<PhotoUploadResponse>?> UploadProfilePhoto(string userId, string base64)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"users/{userId}/upload-profile-photo", new UploadRequest { Base64 = base64 });
                var content = await response.Content.ReadFromJsonAsync<ApplicationResponse<PhotoUploadResponse>>();
                return content;
            }
            catch (HttpRequestException ex)
            {
                return new ApplicationResponse<PhotoUploadResponse>(ex);
            }
        }
    }
}
