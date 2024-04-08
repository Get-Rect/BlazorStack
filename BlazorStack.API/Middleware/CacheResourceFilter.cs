using BlazorStack.Services.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.Json;

public class CacheResourceFilter : IResourceFilter
{
    private readonly IMemoryCache _cache;

    public CacheResourceFilter(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var requestUri = context.HttpContext.Request.GetDisplayUrl();

        if (_cache.TryGetValue(requestUri, out object? cachedValue))
        {
            var result = new OkObjectResult(cachedValue);
            context.Result = result;
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        if (context.Result is ObjectResult result)
        {
            var requestUri = context.HttpContext.Request.GetDisplayUrl();
            var path = context.HttpContext.Request.Path.ToString().GetBasePath();
            
            // create a cache entry to hold the unique uri's we have cache entries for based on controller name
            // for example, '/users?search="test"' and all its variations will get added to a list of strings cached with the key "users"
            var cacheValue = _cache.GetOrCreate(path, entry =>
            {
                entry.Value = new List<string>();
                entry.SetPriority(CacheItemPriority.NeverRemove);
                return new List<string>();
            });
            bool keyAlreadyExists = cacheValue?.Any(x => requestUri.Equals(x, StringComparison.OrdinalIgnoreCase)) == true;
            if (!keyAlreadyExists)
            {
                if (cacheValue is not null) cacheValue.Add(requestUri);
                _cache.Set(path, cacheValue);
            }

            _cache.Set(requestUri, result.Value, TimeSpan.FromMinutes(5)); // Cache duration of 5 minutes
        }
    }
}
