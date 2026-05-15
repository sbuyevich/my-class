# Task 01 - BRD Update

## Goal

Update the Stage 1 BRD so it clearly defines the new teacher and student landing page behavior.

## Work

- Replace the current short BRD text with the planned clarified requirements.
- Document that the Home page is removed.
- Document the new `/teacher` and `/student` landing routes.
- Document post-login redirects by role.
- Document teacher and student access boundaries.
- Document that the teacher LAN URL is detected through the existing `UrlService`.
- Document the teacher QR code, class query parameter, and fallback behavior.
- Document the student waiting message.

## Deliverables

- Updated `design/v 1.0/stage 1/brd.md`.

## Acceptance Criteria

- BRD states teachers redirect to `/teacher` after login.
- BRD states students redirect to `/student` after login.
- BRD states `/home` and Home navigation are removed.
- BRD states the teacher QR/display URL includes the current `c` query parameter.
- BRD states `UrlService` LAN URL detection failure shows a clear error instead of a QR code.
- BRD states students wait until the teacher is ready for quiz activity.

## Notes

- Keep the BRD concise and implementation-oriented.
