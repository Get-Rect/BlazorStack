using Blazored.LocalStorage;
using BlazorStack.Portal;
using BlazorStack.Portal.Auth;
using BlazorStack.Portal.Services;
using BlazorStack.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<ApplicationTokenHandler>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();

builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddHttpClient<ApplicationAPIService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new Exception("Backend url is not configured."));
}).AddHttpMessageHandler<ApplicationTokenHandler>();

builder.Services.AddHttpClient<TokenService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new Exception("Backend url is not configured."));
});

builder.Services.AddBlazoredLocalStorageAsSingleton();

builder.Services.AddSingleton<NotificationService>();

await builder.Build().RunAsync();

//test
