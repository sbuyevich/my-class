# Task 10 - Verification

## Goal

Verify the role-specific landing pages work end to end and do not regress existing class, login, or quiz behavior.

## Work

- Build the solution.
- Verify teacher login and stored-login redirect behavior.
- Verify student login and stored-login redirect behavior.
- Verify teacher and student direct route access boundaries.
- Verify Home page and Home navigation removal.
- Verify teacher LAN URL and QR behavior through `UrlService`.
- Verify teacher LAN URL failure behavior when `UrlService` returns no URL.
- Verify student waiting page behavior.
- Smoke test existing class-context, registration, and quiz navigation flows.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds from `src`.
- Teacher login redirects to `/teacher`.
- Student login redirects to `/student`.
- Stored teacher login state redirects to `/teacher`.
- Stored student login state redirects to `/student`.
- `/home` is not used as a landing page.
- Home navigation item is removed.
- Teacher can access `/teacher`; student and unauthenticated users cannot.
- Student can access `/student`; teacher and unauthenticated users cannot.
- Teacher page displays a `UrlService`-detected LAN URL with the current `c` query parameter.
- Teacher page QR code encodes the same URL displayed on the page.
- `UrlService` LAN URL detection failure shows a clear error instead of a QR code.
- Student page tells the student to wait until the teacher is ready for the quiz.
- Existing class context, student registration, and quiz pages still work.

## Notes

- Run commands from `my-class/src`.
- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.
