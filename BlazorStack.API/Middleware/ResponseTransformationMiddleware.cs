using BlazorStack.Services.Models;
using BlazorStack.Services.Models.Responses;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using System.Text.Json;

namespace BlazorStack.API.Middleware
{
    /// <summary>
    /// Transforms all responses from the API to a consitent format based on the ApplicationResponse<T> class.
    /// </summary>
    public class ResponseTransformationMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTransformationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType = "application/json";

            if(context.Response.StatusCode == 401)
            {
                await HandleUnauthorizedResponse(context, responseBody);
            }
            else if(context.Response.StatusCode == 403)
            {
                await HandleForbiddenResponse(context, responseBody);
            }
            else if(context.Response.StatusCode == 404)
            {
                await HandleNotFoundResponse(context, responseBody);
            }
            else if (context.Response.StatusCode >= 200 && context.Response.StatusCode <= 299)
            {
                await HandleSuccessResponse(context, responseBody);
            }
            else
            {
                await HandleErrorResponse(context, responseBody);
            }
        }

        private async Task HandleSuccessResponse(HttpContext context, MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var readToEnd = await new StreamReader(responseBody).ReadToEndAsync();
            var data = string.IsNullOrEmpty(readToEnd) ? null : JsonSerializer.Deserialize<object>(readToEnd);

            var response = new ApplicationResponse<object>
            {
                StatusCode = context.Response.StatusCode,
                Data = data
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleErrorResponse(HttpContext context, MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var readToEnd = await new StreamReader(responseBody).ReadToEndAsync();
            var data = string.IsNullOrEmpty(readToEnd) ? null : JsonSerializer.Deserialize<List<string>>(readToEnd);
            var errors = data?.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var response = new ApplicationResponse<object>
            {
                StatusCode = context.Response.StatusCode,
                Errors = errors,
                Data = null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleUnsuccessfulResponse(HttpContext context, string errorMessage)
        {
            var response = new ApplicationResponse<object>
            {
                StatusCode = context.Response.StatusCode,
                Errors = new List<string>() { errorMessage },
                Data = null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleNotFoundResponse(HttpContext context, MemoryStream responseBody) => await HandleUnsuccessfulResponse(context, "Not found.");
        private async Task HandleUnauthorizedResponse(HttpContext context, MemoryStream responseBody) => await HandleUnsuccessfulResponse(context, "Unauthorized.");
        private async Task HandleForbiddenResponse(HttpContext context, MemoryStream responseBody) => await HandleUnsuccessfulResponse(context, "Forbidden.");
    }
}
