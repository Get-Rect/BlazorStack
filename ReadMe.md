## Summary
This web app and api demonstrate how easy it is to create a Blazor Web Assembly app secured with ASP.Net Identity in .Net 8.

## Features
### Front End
- Standalone Blazor Web Assembly front-end secured with ASP.Net Identity
- Role based authorization
    - To access different pages / components
    - Different visible content within components
- Manage users on a users page
- Upload profile photos for users
### Backend
- ASP.Net Web API Secured with ASP.Net Identity
- Role based authorization for accessing different API endpoints
- In memory response caching middleware for performance
- Response formatting middleware for consistent responses and easier frontend error handling.
- SQL Server with LocalDB for local development.
- Azurite development storage for local development.

### In Development
- Signlar R for live refreshing of data


## Setup
1. Execute "update-database" in package manager console with BlazorStack.API selected as your startup program and BlazorStack.Data selected as your default project.
2. Setup an Azurite Emulator.
    - Either use Docker or the built in VS Azurite emulator
    - https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage
3. Configure your solution to run both the API and the Portal projects.
    - Right click solution
    - Select configure startup projects
    - Select "multiple startup projects" 
    - Set the action for the API project to "Start."
    - Use the arrows on the right of the projectsgrid to move the API to the top of the list.
    - Set the action for the Portal project to "Start."
    - Use the arrows on the right of the projects grid to move the Portal project below the API project.
4. Run the project
5. Login with test@test.com, Test123!


## Resources
- https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0
- https://github.com/dotnet/blazor-samples/blob/main/8.0/BlazorWebAssemblyStandaloneWithIdentity/BlazorWasmAuth/Program.cs
- https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage