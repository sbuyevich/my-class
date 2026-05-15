# Task 03 - Teacher Landing Page

## Goal

Build the teacher landing page that helps students connect to the current class site.

## Work

- Create the `/teacher` page.
- Use the existing `IUrlService` / `UrlService` to detect the LAN host URL.
- Display the student entry URL using the LAN URL returned by `UrlService`.
- Preserve the current class query parameter in the displayed URL, for example `?c=demo`.
- Generate and display a QR code for the same URL.
- Show a clear error message when `UrlService` cannot detect a LAN URL.
- Use a two-column desktop layout:
  - Left column: instruction text and URL.
  - Right column: QR instruction text and QR code.
- Ensure the layout remains readable on narrow screens.
- Restrict page access to teachers.

## Deliverables

- Teacher landing page UI.
- QR code generation/display.
- LAN URL failure state.
- Teacher-only access behavior.

## Acceptance Criteria

- Teacher can access `/teacher`.
- Student and unauthenticated users cannot use `/teacher`.
- The displayed URL is based on the LAN URL returned by `UrlService`.
- The displayed URL includes the current `c` query parameter.
- The QR code encodes the same URL shown on the page.
- `UrlService` LAN URL detection failure shows a clear error and does not show a stale QR code.
- The page uses the required two-column layout on desktop.

## Notes

- Do not add a second LAN IP detection implementation in the page.
- Let `UrlService` detect the LAN base URL, then append/preserve the current class query parameter for the student entry URL.
- The QR URL should send students through the existing class-aware entry/login flow.
