# Auto-Reveal Student Answer Results On Finish/Timeout

## Summary
Change quiz completion behavior so students see their answer result when any question ends, either because the teacher clicks **Finish** or because the timer expires. This applies to every question, not just the final one.

## Key Changes
- Treat “finish question” as “finish and reveal question” across quiz services.
- In `QuizSessionService`, update the finish path used by teacher **Finish**, teacher state timeout handling, and move-next timeout handling so finished rows also set `AnswerRevealedAtUtc`.
- In `QuizAnswerService`, update the student timeout path so expired questions are finished and revealed, then return the same revealed state used by the existing **Show Answer** path.
- Preserve existing student UI behavior: once `IsAnswerRevealed` is true, the student panel shows the configured correct/incorrect/no-answer message and answer image.

## Public API / Types
- No database schema changes.
- No model shape changes required.
- Existing `QuizAnswer.AnswerRevealedAtUtc` remains the source of truth for whether a result is visible to students.

## Test Plan
- Build with `dotnet build .\src\MyClass.slnx` once NuGet/network and locked output issues are cleared.
- Manual teacher finish scenario:
  - Start a quiz question.
  - Student submits an answer.
  - Teacher clicks **Finish**.
  - Student immediately sees correct/incorrect result without teacher pressing **Show Answer**.
- Manual timeout scenario:
  - Start a timed question and let it expire.
  - Student sees result after refresh/polling.
- Regression scenario:
  - Move to the next question after a finished/revealed question.
  - Student progress marks prior questions correctly and current question remains answerable.

## Assumptions
- “Result” means the per-question answer result message and answer image, not a full student score summary page.
- Auto-reveal applies to every question, per your selection.
