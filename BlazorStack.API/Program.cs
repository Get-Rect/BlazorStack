using BlazorStack.API.Middleware;
using BlazorStack.Data;
using BlazorStack.Data.Contexts;
using BlazorStack.Data.Models;
using BlazorStack.Services;
using BlazorStack.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Integrated Security=true"));
builder.Services.Configure<StorageAccountSettings>(builder.Configuration.GetSection("StorageAccount"));
builder.Services.AddTransient<BlobService>();

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();

// Add identity and opt-in to endpoints
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CacheResourceFilter>();
builder.Services.AddScoped<EvictCacheKeysFilter>();

// Add a CORS policy for the client
builder.Services.AddCors(
    options => options.AddPolicy(
        "wasm",
        policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? throw new Exception("BackendUrl not configured."),
            builder.Configuration["FrontendUrl"] ?? throw new Exception("FrontendUrl not configured.")])
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await Seeder.Seed(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ResponseTransformationMiddleware>();
app.MapIdentityApi<ApplicationUser>();
app.UseCors("wasm");

app.Run();