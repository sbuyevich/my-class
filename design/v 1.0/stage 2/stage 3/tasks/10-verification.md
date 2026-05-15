# Task 10 - Verification

## Goal

Verify the Stage 3 UI changes work end to end and do not regress teacher, student, or quiz workflows.

## Work

- Build the solution.
- Verify teacher Home menu behavior.
- Verify teacher drawer state after teacher login and stored-login restore.
- Verify student users see no menu button or drawer.
- Verify the Teacher Quiz Start button starts the selected quiz/question.
- Verify student waiting page display before a question starts.
- Verify student active quiz display after Start.
- Verify mobile header behavior for teacher, student, and auth states.
- Smoke test Restart, Finish, Show Answer, Next, and quiz result navigation.

## Deliverables

- Successful build.
- Manual verification notes for teacher and student workflows.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\src\MyClass.slnx` succeeds.
- Teacher menu contains Home/Teacher Home and routes to `/teacher`.
- Teacher drawer is open after teacher login and stored-login restore.
- Student users have no menu button, drawer, or menu links.
- Start begins the selected quiz/question and updates teacher UI.
- Student `/student` page updates without refresh after Start.
- Student waiting state has no answer numbers or answer buttons.
- Mobile header has no overlapping or overflowing text.
- Existing teacher quiz controls still work.

## Notes

- Use `dotnet build .\src\MyClass.slnx --no-restore` if packages are already restored and network restore is unavailable.
- If normal debug output is locked by Visual Studio or a running app, use an isolated output folder:

```powershell
dotnet build .\src\MyClass.slnx --no-restore -p:BaseOutputPath=C:\.sbuyevich\my-class\.build\
```

- Manual verification should use at least one teacher browser session and one student browser session.
