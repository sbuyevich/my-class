# MyClass

MyClass is a .NET 10 Blazor web app for running classroom quizzes. Teachers can manage schools, classes, and students, start quiz questions, and review/export results. Students can register or sign in with a class code and submit quiz answers from their own browser.

## Features

- Teacher and student login flows scoped by class code.
- School, class, and student management backed by SQLite.
- Real-time quiz status updates with SignalR.
- Quiz content loaded from local JSON and image assets.
- MudBlazor UI components.
- Windows and macOS publish targets with packaged `dist-*` folders.

## Repository Layout

```text
src/
  MyClass.slnx                 Main solution
  MyClass.Web/                 Blazor app, pages, layout, hubs, static files
  MyClass.Core/                Services, models, EF Core data access, options
  assets/quizzes/              Quiz JSON and media assets
  database/my-class.db         Local SQLite database
scripts/
  publish.ps1                  Publish one target package
  dist.bat                     Build Windows and macOS packages and zip them
dist-win/                      Windows distribution folder
dist-macos/                    macOS distribution folder
design/                        Product, database, stage, and launch notes
```

## Requirements

- .NET 10 SDK
- PowerShell for publish/package scripts

The app targets `net10.0` and uses EF Core SQLite, SignalR, and MudBlazor.

## Run Locally

From the repository root:

```powershell
dotnet build .\src\MyClass.slnx
dotnet run --project .\src\MyClass.Web\MyClass.Web.csproj --launch-profile http
```

Open:

```text
http://localhost:5555/?c=demo
```

The seeded demo class code is `demo`. The default teacher credentials are configured in `src/MyClass.Web/appsettings.json`:

```text
Username: t
Password: t
```

Students can register from the login screen while using a valid class code.

## Quiz Content

Quiz content is stored under `src/assets/quizzes`. Each quiz folder has a `quiz.json` file and numbered question folders:

```text
src/assets/quizzes/astronomy/
  quiz.json
  01/
    q.json
    q.jpg
    a.jpg
```

The quiz root is configured with `Quiz:RootFolder` in `src/MyClass.Web/appsettings.json`. Current included quiz sets are:

- `astronomy`
- `csharp`
- `funny`

## Database

The app uses SQLite with this default connection string:

```json
"DefaultConnection": "Data Source=../database/my-class.db"
```

On startup, `DatabaseInitializer` creates missing tables/columns and seeds a demo school/class when the database is empty.

## Publish

Publish one target package:

```powershell
.\scripts\publish.ps1 -OS win -Architecture x64
.\scripts\publish.ps1 -OS macos -Architecture x64
```

Useful options:

- `-Configuration Release`
- `-Architecture x64` or `arm64`
- `-RuntimeIdentifier win-x64` or `osx-arm64`
- `-FrameworkDependent`
- `-NoSingleFile`
- `-NoClean`

Build both distribution folders and zip files from Windows:

```bat
.\scripts\dist.bat
```

This creates/updates:

- `dist-win/`
- `dist-win.zip`
- `dist-macos/`
- `dist-macos.zip`

## Run Packaged Windows App

After publishing, launch the packaged Windows app with:

```bat
.\dist-win\scripts\run.bat demo
```

or:

```powershell
.\dist-win\scripts\run.ps1 -ClassCode demo -Port 5555
```

The launcher binds the app to a local network IPv4 address, opens the browser with `?c=<class-code>`, and stores the app PID in `dist-win/myclass.pid`.

## Development Notes

- Keep app host, Razor pages, layout, and hubs in `src/MyClass.Web`.
- Keep shared services, models, options, and EF Core data access in `src/MyClass.Core`.
- Avoid manually editing generated output under `dist-*`, `src/artifacts`, `bin`, or `obj`.
- Design and implementation planning notes live in `design/`.
