using Blazored.LocalStorage;

namespace BlazorStack.Portal.Auth
{
    public class ApplicationTokenHandler : DelegatingHandler
    {
        ILocalStorageService _localStorage;

        public ApplicationTokenHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = await GetTokenFromLocalStorage();
            if (!string.IsNullOrEmpty(token)) request.Headers.Add("Authorization", $"Bearer {token}");
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetTokenFromLocalStorage()
        {
            return await _localStorage.GetItemAsStringAsync("token") ?? string.Empty;
        }
    }
}
