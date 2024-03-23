using BlazorStack.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

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
            var id = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
            return new AuthenticationState(new ClaimsPrincipal(id));
        }
    }
}
