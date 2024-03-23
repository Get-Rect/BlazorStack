using BlazorStack.Portal;
using BlazorStack.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<ApplicationAPIService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? throw new Exception("Backend url is not configured."));
});

await builder.Build().RunAsync();
