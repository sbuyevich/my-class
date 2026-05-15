# AGENTS.md

## Project Overview

This repository contains a .NET 10 Blazor Server/Web app for MyClass.

- `src/MyClass.slnx` is the main solution file.
- `src/MyClass.Web` contains the Blazor UI, app host, pages, layout, SignalR hub wiring, and web-specific behavior.
- `src/MyClass.Core` contains shared services, models, EF Core data access, options, and business logic.
- `src/assets/quizzes` contains quiz content and media used by the app.
- `design/` contains staged planning, product, database, and implementation notes.

## Documentation And Library Guidance

Use Context7 MCP to fetch current documentation whenever asked about a library, framework, SDK, API, CLI tool, or cloud service. This includes ASP.NET Core, Blazor, EF Core, SignalR, MudBlazor, the .NET CLI, and similar implementation-specific questions.

Always start with Context7 library resolution unless the user provides an exact `/org/project` library ID. Answer from the fetched docs, not memory, when current API or configuration details matter.

Do not use Context7 for refactoring, writing scripts from scratch, debugging business logic, code review, or general programming concepts.

Context7 workflow:

1. Start with `resolve-library-id` using the library name and the user's question unless the user provides an exact library ID in `/org/project` format.
2. Pick the best match by exact name match, description relevance, code snippet count, source reputation, and benchmark score. If results do not look right, retry with alternate library names or rephrased queries.
3. Use `query-docs` with the selected library ID and the user's full question, not isolated keywords.
4. Answer using the fetched documentation.

## Coding Conventions

- Follow existing project patterns before adding new abstractions.
- Keep shared app logic in `MyClass.Core`.
- Keep UI, routing, page state, and web-host behavior in `MyClass.Web`.
- In `.razor` files, prefer MudBlazor components, parameters, and utility classes over custom string CSS classes.
- Prefer existing service interfaces and dependency injection registration patterns.
- For Razor components, use `.razor` plus `.razor.cs` code-behind where the repo already follows that pattern.
- Keep changes small, reviewable, and focused on the requested feature or fix.

## Data And Assets

- The SQLite connection string is configured in `src/MyClass.Web/appsettings.json`.
- Quiz content lives under `src/assets/quizzes`.
- Follow `design/db.md` for school, class, student, and quiz answer relationships.
- Treat local SQLite files, quiz media, and packaged distribution assets carefully because they may be user-facing data or artifacts.

## Generated And Distribution Files

Do not manually edit generated output directories unless explicitly asked.

- Avoid editing `dist-win/app`, `dist-macos/app`, `src/artifacts`, `bin`, or `obj`.
- Treat `dist-win`, `dist-macos`, and `.zip` packages as distribution artifacts.
- Do not overwrite generated distribution assets unless the task is specifically about rebuilding or packaging them.

## Build, Run, And Verification

- Use `dotnet build .\src\MyClass.slnx` for compile verification.
- Use `dotnet run --project .\src\MyClass.Web\MyClass.Web.csproj --launch-profile http` to run locally.
- The local HTTP profile runs at `http://localhost:5555`.
- For publish or package checks, prefer `.\scripts\publish.ps1` and its existing parameters.
- There are no obvious test projects currently, so use focused build checks and manual workflow verification for UI behavior unless tests are added.

## Planning Workflow

- Respect the staged design docs under `design/stage *`.
- For meaningful feature work, update relevant design or task docs when implementation changes the plan.
- Keep BRDs business-focused and plans/task files implementation-focused, following `design/development.md`.
- Implement one task or one small group of related tasks at a time.

## Safety Notes

- Do not change global Git config automatically if Git reports a safe-directory warning; report it and ask before changing global config.
- Do not overwrite user changes.
- Do not commit secrets or local-only settings such as `appsettings.Development.json`, `.env`, `.keys`, or local DB sidecar files.
- Do not revert or delete user-facing distribution artifacts unless explicitly requested.
