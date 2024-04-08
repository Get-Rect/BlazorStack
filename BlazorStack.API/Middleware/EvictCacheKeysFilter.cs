using BlazorStack.Services.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorStack.API.Middleware
{
    public class EvictCacheKeysFilter : IActionFilter
    {
        private readonly IMemoryCache _cache;

        public EvictCacheKeysFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // do nothing
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            EvictAllCacheKeysForTheRelevantController(context);
        }

        private void EvictAllCacheKeysForTheRelevantController(ActionExecutedContext context)
        {
            var statusCode = context.HttpContext.Response.StatusCode;
            if (statusCode >= 200 && statusCode <= 299)
            {
                var path = context.HttpContext.Request.Path.ToString().GetBasePath();
                var keys = _cache.Get<List<string>>(path);
                if (keys?.Any() == true)
                {
                    foreach (var key in keys)
                    {
                        _cache.Remove(key);
                    }
                    _cache.Set(path, new List<string>());
                }
            }
        }
    }
}
