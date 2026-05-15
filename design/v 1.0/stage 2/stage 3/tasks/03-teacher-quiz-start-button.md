# Task 03 - Teacher Quiz Start Button

## Goal

Fix the Teacher Quiz Start button so clicking Start begins the selected quiz/question and updates student screens.

## Work

- Reproduce or inspect the Start button failure before changing code.
- Verify whether the problem is disabled-state logic, missing selected quiz state, login state, service result handling, UI refresh, SignalR notification, or another cause.
- Apply the smallest fix that makes Start create active quiz/question state.
- Ensure teacher UI updates after Start.
- Ensure student pages update through the existing live quiz notification flow.
- Preserve Restart, Finish, Show Answer, and Next behavior.

## Deliverables

- Working Start button on the teacher quiz page.
- Teacher UI shows the active question after Start.
- Student page updates without manual refresh when Start succeeds.

## Acceptance Criteria

- Start is enabled when a teacher has a valid selected quiz and can start a quiz.
- Clicking Start creates the first active question for the current class.
- Teacher status/table/image updates after Start.
- Student `/student` page receives the started question without refresh.
- If Start cannot run, the UI exposes a meaningful failure message or existing alert/snackbar feedback.
- Existing teacher quiz controls are not regressed.

## Notes

- Treat this as investigation-first; do not pre-decide the root cause.
- Keep changes scoped to quiz start behavior unless inspection shows a shared bug.
