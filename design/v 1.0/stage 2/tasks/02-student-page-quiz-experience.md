# Task 02 - Student Page Quiz Experience

## Goal

Render the quiz answer experience directly on `/student` so students wait and answer from the same page.

## Work

- Update the Student page to render `StudentQuizAnswerPanel` for the current class.
- Keep the Student page protected by `RoleGate RequireTeacher="false"`.
- Preserve the existing page header that identifies the Student page and current class.
- Ensure the no-active-question state shows a waiting message suitable for the Student page.
- Reuse the existing `StudentQuizAnswerPanel` SignalR, polling, timer, image, progress, reveal, and answer submission behavior.
- Do not duplicate quiz answer state logic in `Student.razor` or `Student.razor.cs`.

## Deliverables

- `/student` displays the waiting state before a quiz question starts.
- `/student` displays answer controls when a teacher starts a question.
- `/student` updates as the teacher advances, finishes, reveals answers, and completes quiz flow.

## Acceptance Criteria

- A student on `/student` sees `Please wait until your teacher is ready for the quiz.` or equivalent waiting text before an active question.
- When the teacher starts a question, the student sees the question image, timer, progress, and answer buttons on `/student`.
- After submitting an answer, the student remains on `/student` and sees submitted/waiting status.
- When the teacher reveals an answer, the student sees the reveal feedback on `/student`.
- No manual browser refresh is required for teacher-driven quiz state changes.

## Notes

- Prefer reusing the existing `StudentQuizAnswerPanel` rather than creating a second student quiz component.
- If the existing service message says `Waiting for the teacher to start a question.`, keep it unless the implementation also updates the core message consistently.
