using Blazored.LocalStorage;
using BlazorStack.Portal.Services;
using BlazorStack.Services;
using BlazorStack.Services.Extensions;
using BlazorStack.Services.Models;
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
            // Set basic claims
            var userInfoResponse = await _api.GetUserAdditionalInfo();
            if (!userInfoResponse?.IsSuccess == true || userInfoResponse is null || userInfoResponse.Data is null) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var userInfo = userInfoResponse.Data;
            var claims = new List<Claim>
            {
                    new(ClaimTypes.Name, userInfo.Email),
                    new(ClaimTypes.Email, userInfo.Email)
            };

            // Set custom ApplicationUser property
            if (!string.IsNullOrEmpty(userInfo.PhotoUrl)) claims.Add(new Claim("photo-url", userInfo.PhotoUrl.AddTimestampQueryString() ?? string.Empty));

            // Set role
            var roleClaim = userInfo.RoleClaim;
            if (!string.IsNullOrEmpty(roleClaim?.Type) && !string.IsNullOrEmpty(roleClaim?.Value))
            {
                claims.Add(new Claim(roleClaim.Type, roleClaim.Value, roleClaim.ValueType, roleClaim.Issuer, roleClaim.OriginalIssuer));
            }

            // build and return new identity principal
            var identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
            var state = new AuthenticationState(new ClaimsPrincipal(identity));
            return state;
        }

        public async Task<List<string>> Register(string email, string password)
        {
            var response = await _api.CreateUser(new UserViewModel() { Email = email, Password = password });
            return response?.Errors ?? new List<string>();
        }
    }
}
