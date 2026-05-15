# Task 01 - Student Route and Navigation

## Goal

Make `/student` the only normal student destination after login and in app navigation.

## Work

- Confirm student login and registration redirects already use `/student` through `LandingRoutes`.
- Remove the student-facing `Quiz Answer` navigation link that points to `/quiz-answer`.
- Add or keep a student-facing navigation link to `/student` only if the current navigation pattern needs an explicit student landing link.
- Preserve teacher navigation behavior.
- Preserve class query parameter handling and current class context behavior.

## Deliverables

- Student navigation no longer points to `/quiz-answer`.
- Student login and registration continue to land on `/student`.
- Teacher navigation remains unchanged.

## Acceptance Criteria

- Student login redirects to `/student`.
- Student registration redirects to `/student`.
- Stored student login state lands on `/student`.
- Student navigation does not expose `/quiz-answer`.
- Teacher navigation still exposes teacher-only pages.

## Notes

- Do not change teacher landing behavior.
- Do not rename `LandingRoutes` unless needed by the implementation.
