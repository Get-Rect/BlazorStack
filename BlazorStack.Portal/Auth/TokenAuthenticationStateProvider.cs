using Blazored.LocalStorage;
using BlazorStack.Portal.Services;
using BlazorStack.Services;
using BlazorStack.Services.Extensions;
using BlazorStack.Services.Models.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Security.Claims;

namespace BlazorStack.Portal.Auth
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
    {
        private readonly ApplicationAPIService _api;
        private readonly ILocalStorageService _localStorage;

        public TokenAuthenticationStateProvider(ApplicationAPIService api, ILocalStorageService localStorage)
        {
            _api = api;
            _localStorage = localStorage;
        }

        public async Task<string> Login(string email, string password)
        {
            string error = string.Empty;
            var loginResponse = await _api.Login(email, password);
            if (loginResponse?.IsSuccess == true && loginResponse.Data is not null)
            {
                loginResponse.Data.Issued = DateTime.UtcNow;
                await _localStorage.SetItemAsync("token", loginResponse?.Data);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
            else
            {
                error = "Login failed.";
            }
            return error;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("token");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userInfoResponse = await _api.GetUserInfo();
            if (!userInfoResponse?.IsSuccess == true || userInfoResponse is null || userInfoResponse.Data is null) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var userInfo = userInfoResponse.Data;

            var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email)
                    };

            claims.AddRange(
                userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                    .Select(c => new Claim(c.Key, c.Value)));

            var rolesResponse = await _api.GetRoles();
            if(!rolesResponse?.IsSuccess == true || rolesResponse is null) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var roles = rolesResponse?.Data;

            if (roles?.Count > 0)
            {
                foreach (var role in roles)
                {
                    if (!string.IsNullOrEmpty(role.Type) && !string.IsNullOrEmpty(role.Value))
                    {
                        claims.Add(new Claim(role.Type, role.Value, role.ValueType, role.Issuer, role.OriginalIssuer));
                    }
                }
            }

            var additionalInfo = await _api.GetUserAdditionalInfo();
            if (additionalInfo != null && !string.IsNullOrEmpty(additionalInfo?.Data?.PhotoUrl))
            {
                claims.Add(new Claim("photo-url", additionalInfo?.Data?.PhotoUrl.AddTimestampQueryString() ?? string.Empty));
            }
            var id = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
            var user = new AuthenticationState(new ClaimsPrincipal(id));
            return user;
        }

        public async Task<List<string>> Register(string email, string password)
        {
            var response = await _api.CreateUser(new UserViewModel() { Email = email, Password = password });
            return response?.Errors ?? new List<string>();
        }

        //public async Task<AuthenticationState> CheckAuthenticatedAsync()
        //{
        //    var state = await GetAuthenticationStateAsync();
        //    return state;
        //}
    }
}
