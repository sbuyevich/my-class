# Task 04 - Student Page Display Cleanup

## Goal

Clean up the Student page waiting state and remove duplicated student/class headings from page content.

## Work

- Remove the local `Student` title from the Student page content.
- Remove the local class subtitle from the Student page content.
- Keep class context visible through the shared app header where available.
- Ensure quiz answer numbers are hidden before a question is active.
- Ensure answer buttons are hidden before a question is active.
- Preserve active question, submitted answer, reveal feedback, no-answer, and completed-question states.

## Deliverables

- Student page content starts directly with the student quiz/waiting experience.
- Waiting state does not show answer numbers or answer buttons.
- Active quiz states remain functional.

## Acceptance Criteria

- `/student` no longer renders a local `Student` heading.
- `/student` no longer renders a local class subtitle beneath that heading.
- Before a question starts, students see a clean waiting/status message with no answer numbers and no answer buttons.
- During an active question, students see the expected question image, timer, progress, and answer buttons.
- Submitted, reveal, and finished states still show the correct feedback.

## Notes

- Prefer updating `StudentQuizAnswerPanel` display conditions rather than duplicating quiz-state logic in `Student.razor`.
- Do not remove shared header class context in this task.
