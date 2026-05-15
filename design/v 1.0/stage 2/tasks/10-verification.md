# Task 10 - Verification

## Goal

Verify `/student` is the single student quiz page and live quiz updates still work.

## Work

- Build the solution.
- Verify student login and registration routing.
- Verify student navigation no longer points to `/quiz-answer`.
- Verify `/student` waiting behavior before a question starts.
- Verify live update behavior when the teacher starts, advances, finishes, and reveals a quiz question.
- Verify student answer submission and submitted-state messaging.
- Verify teacher quiz controls still work.
- Verify class context remains scoped to the current `c` query parameter.

## Deliverables

- Successful build.
- Manual verification notes for the student quiz workflow.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\src\MyClass.slnx` succeeds.
- Student login redirects to `/student`.
- Student registration redirects to `/student`.
- Student navigation does not show a `/quiz-answer` link.
- `/student` shows a waiting state before a teacher starts a question.
- `/student` updates without refresh when a teacher starts or advances a question.
- `/student` updates without refresh when a teacher finishes or reveals a question.
- Student answer submission still persists and updates the UI.
- Teacher quiz page behavior is not regressed.
- Class-scoped student and quiz behavior still uses the active class context.

## Notes

- Use `dotnet build .\src\MyClass.slnx --no-restore` if packages are already restored and network restore is unavailable.
- Manual verification should use at least one teacher browser session and one student browser session.
