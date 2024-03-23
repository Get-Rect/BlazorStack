using BlazorStack.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorStack.Portal.Auth
{
    //public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    //{
    //    private readonly ApplicationAPIService _api;

    //    public TokenAuthenticationStateProvider(ApplicationAPIService api)
    //    {
    //        _api = api;
    //    }

    //    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        // default user to a blank unauthenticated identity
    //        var user = new ClaimsPrincipal(new ClaimsIdentity());

    //        var userResponse = await _api.GetUserInfo();
    //    }
    //}
}
