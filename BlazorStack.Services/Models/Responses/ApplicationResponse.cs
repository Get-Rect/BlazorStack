using BlazorStack.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services.Models.Responses
{
    public class ApplicationResponse<T>
    {
        public ApplicationResponse() { }
        public ApplicationResponse(HttpRequestException ex)
        {
            StatusCode = ex.StatusCode is not null ? (int)ex.StatusCode : 0;
            Errors = new List<string> { StatusCode.ToStatusCodeDecsription() };
        }

        public int StatusCode { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess { get => StatusCode >= 200 && StatusCode <= 299; }
    }
}
