using Blazored.LocalStorage;
using BlazorStack.Portal.Services;
using BlazorStack.Services;
using BlazorStack.Services.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorStack.Portal.Auth
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ApplicationAPIService _api;

        public TokenAuthenticationStateProvider(ApplicationAPIService api)
        {
            _api = api;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userInfo = await _api.GetUserInfo();
            if (userInfo == null) return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email)
                    };

            claims.AddRange(
                userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                    .Select(c => new Claim(c.Key, c.Value)));

            var roles = await _api.GetRoles();

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
            var id = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
            var user = new AuthenticationState(new ClaimsPrincipal(id));
            //NotifyAuthenticationStateChanged(user);
            // you need to implement your login methods here so you can call NotifyAuthenticationStateChanged with using .result.
            return user;
        }
    }
}
