using Blazored.LocalStorage;
using BlazorStack.Portal.Services;

namespace BlazorStack.Portal.Auth
{
    public class ApplicationTokenHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly TokenService _tokens;

        public ApplicationTokenHandler(ILocalStorageService localStorage, TokenService tokens)
        {
            _localStorage = localStorage;
            _tokens = tokens;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authResponse = await _tokens.GetToken();
            if (authResponse is not null)
            {
                request.Headers.Add("Authorization", $"Bearer {authResponse.AccessToken}");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
