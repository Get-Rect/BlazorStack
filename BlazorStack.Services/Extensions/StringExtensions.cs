using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Extensions
{
    public static class StringExtensions
    {
        public static string AddTimestampQueryString(this string query) => query += $"?timestamp={DateTime.UtcNow}";
        public static string GetBasePath(this string path) => path.Split("/")[1];
        
    }
}
