using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Extensions
{
    public static class IntExtensions
    {
        public static string ToStatusCodeDecsription(this int statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return "Unauthorized.";
                case 403:
                    return "Forbidden.";
                case 404:
                    return "Not found.";
                default:
                    return "An unexpected error has occurred.";
            }
        }
    }
}
