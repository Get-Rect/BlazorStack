using Blazored.LocalStorage;
using BlazorStack.Portal;
using BlazorStack.Portal.Auth;
using BlazorStack.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();

builder.Services.AddSingleton<AuthenticationStateProvider, TokenAuthenticationStateProvider>();

builder.Services.AddTransient<ApplicationTokenHandler>();
builder.Services.AddHttpClient<ApplicationAPIService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new Exception("Backend url is not configured."));
}).AddHttpMessageHandler<ApplicationTokenHandler>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
