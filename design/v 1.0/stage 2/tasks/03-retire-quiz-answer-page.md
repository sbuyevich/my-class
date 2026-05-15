# Task 03 - Retire Quiz Answer Page

## Goal

Ensure `/quiz-answer` is no longer the primary student workflow.

## Work

- Remove the `/quiz-answer` page route or replace it with a redirect to `/student`.
- Choose the smaller implementation that fits the current routing structure.
- Keep `StudentQuizAnswerPanel` as a reusable component; do not delete it.
- Remove references that direct students to `/quiz-answer`.
- Preserve direct `/student` access for signed-in students.

## Deliverables

- `/quiz-answer` is not used as a normal student page.
- Existing student quiz functionality lives through `/student`.
- No dead navigation links point to `/quiz-answer`.

## Acceptance Criteria

- Normal student workflow never navigates to `/quiz-answer`.
- Opening `/student` directly as a signed-in student works.
- Opening `/quiz-answer` does not create a separate quiz workflow.
- `StudentQuizAnswerPanel` remains available for `/student`.

## Notes

- If redirecting `/quiz-answer`, use `/student` as the target.
- Do not remove services, models, or SignalR code used by `StudentQuizAnswerPanel`.
