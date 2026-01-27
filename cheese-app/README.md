# Cheese App - .NET Framework 4.8 Demo

Simple ASP.NET Web API application (.NET Framework 4.8) used for demonstrating OpenTelemetry instrumentation.

## Building

This app is automatically built via GitHub Actions when a tag matching `dotnet48-cheese-app-*` is pushed.

### Trigger a build:

```bash
git tag dotnet48-cheese-app-1.0.0
git push origin dotnet48-cheese-app-1.0.0
```

The compiled application will be available as a GitHub Release artifact: `cheese-app-build.zip`

### Manual build (requires Windows):

```powershell
nuget restore cheese-app.sln
msbuild cheese-app.csproj /p:Configuration=Release
```

## API Endpoints

- `GET /api/values` - Returns sample values
- `GET /api/values/{id}` - Returns a specific value
- `POST /api/values` - Creates a value
- `PUT /api/values/{id}` - Updates a value
- `DELETE /api/values/{id}` - Deletes a value

## Usage in demos

This application is used in:
- `dotnet48-windows-alloy/` - Demonstrates Alloy collecting telemetry from .NET Framework 4.8 apps on Windows VMs
