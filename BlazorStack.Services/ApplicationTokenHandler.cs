using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services
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
