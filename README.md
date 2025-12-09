# PermitApplication

Small ASP.NET Core web app for submitting permit applications with basic USPS address validation.

## Overview

- ASP.NET Core MVC project targeting .NET 10
- Simple multi-step form: Home (enter form) ? Review ? Submit
- Uses a `UspsService` to validate addresses via the USPS XML Verify API

## Prerequisites

- .NET 10 SDK
- (Optional) A USPS Web Tools USERID if you want real address validation

## Configuration

1. Open `PermitApplication/appsettings.json` and set your USPS USERID:

```json
{
  "Usps": {
    "UserId": "YOUR_USPS_USERID_HERE"
  }
}
```

2. The `Program.cs` registers an HttpClient named `usps` and registers `UspsService` in DI. No other config is required.

## How to run

From the project folder (`PermitApplication`) run:

```bash
dotnet run
```

Then browse to `https://localhost:<port>/`.

## Usage

1. Fill out the form on the Home page (Name, Street, City, State, Zip, County, Permit Type).
2. Click `Next Step`. The app will attempt to validate the address with USPS and then show the Review page.
3. On the Review page, click `Submit Application` to complete the flow (server-side placeholder — save logic not implemented).

## Important files

- `Program.cs` — service registration and routing
- `Controllers/HomeController.cs` — initial form and USPS validation that leads to Review view
- `Controllers/PermitController.cs` — final submit handling
- `Services/UspsService.cs` — USPS XML request/response handling
- `Views/Home/Index.cshtml`, `Views/Permit/Review.cshtml`, `Views/Permit/Success.cshtml` — UI

## Notes & TODOs

- The app currently uses server-side USPS XML Verify calls. For local development without a USPS USERID the service will return null and show a validation error.
- Consider improving:
  - Persisting permits to a database (EF Core)
  - Stronger input validation and sanitization
  - Better error messages and logging
  - Replace free-text `State` input with a select box

## Third-party components

- `jquery-validation` is included under `wwwroot/lib/jquery-validation` and is MIT licensed. See `wwwroot/lib/jquery-validation/LICENSE.md`.

## Development

- Build: `dotnet build`
- Run: `dotnet run`
- The app is a small learning/demo application and not hardened for production.

