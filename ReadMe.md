## Summary
This web app and api demonstrate how easy it is to create a Blazor Web Assembly app secured with ASP.Net Identity in .Net 8.

## Features
- ASP.Net Web API Secured ASP.Net Identity
- Standalone Blazor Web Assembly front-end also secured with ASP.Net Identity
- Role based authorization for accessing different components, pages, and api end points
- Manage users on a users page

### In Development
- Users can create images from a hugging face AI text-to-image model.
    - Images can be saved and shared with other users
- Output caching for improved API performance
- Signlar R for live refreshing of data


## Setup
- Create an account at https://huggingface.co/, get an access token, and add it to your app settings.

## Resources
- https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0
- https://github.com/dotnet/blazor-samples/blob/main/8.0/BlazorWebAssemblyStandaloneWithIdentity/BlazorWasmAuth/Program.cs