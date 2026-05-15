# Change Landing Pages

## Summary
Replace `design/v 1.0/stage 1/brd.md` with a clearer BRD for teacher and student landing pages, keeping the existing intent but making routes, redirects, QR behavior, and fallback behavior explicit.

## Key Changes
- State that `/home` is removed and replaced by two role-specific landing pages.
- Define routes:
  - Teacher landing page: `/teacher`
  - Student landing page: `/student`
- Define post-login redirects:
  - Teachers go to `/teacher`
  - Students go to `/student`
- Define access behavior:
  - Teachers can only see the teacher landing page.
  - Students can only see the student landing page.
- Define the teacher page QR/display URL:
  - Use the detected LAN host.
  - Include the current class query parameter, such as `?c=demo`.
  - Point students to the student entry/login flow for the current class.
- Define fallback behavior:
  - If LAN URL cannot be detected, show a clear error instead of the QR code.
- Keep the student page requirement simple:
  - Show a waiting message until the teacher is ready for quiz activity.

## Replacement BRD Text
```md
# Change Landing Pages

Create two role-specific landing pages after login: one for teachers and one for students. Remove the existing Home page.

After login:
- Teachers are redirected to `/teacher`.
- Students are redirected to `/student`.

Teachers and students can only access their own landing page.

Remove the Home page and Home navigation item.

# Teacher Landing Page

The teacher landing page should help students connect to the class site from their own devices.

Show the site LAN URL and a QR code for the same URL. The URL should use the detected LAN host and include the current class query parameter, for example `?c=demo`.

The URL and QR code should point students to the student entry/login flow for the current class.

If the LAN URL cannot be detected, show a clear error message instead of the QR code.

## Layout

Use a two-column layout:
- Left column: `Please enter this URL in your browser address bar:` and URL on the next line
- Right column: `Or scan the QR code below with your mobile device:` and QR code on the next line

# Student Landing Page

Show a message telling the student to wait until the teacher is ready for the quiz.
```

## Test Plan
- Verify teacher login redirects to `/teacher`.
- Verify student login redirects to `/student`.
- Verify `/home` and Home navigation are removed.
- Verify teacher page displays LAN URL and matching QR code.
- Verify displayed/QR URL includes the current `c` query parameter.
- Verify LAN detection failure shows an error message.
- Verify students cannot access `/teacher` and teachers cannot access `/student`.

## Assumptions
- `/teacher` and `/student` are the intended new routes.
- The class query parameter remains named `c`.
- The QR code should send students through the existing class-aware entry/login flow.

## Tasks
- [x] [Task 01 - BRD Update](tasks/01-brd-update.md)
- [x] [Task 02 - Routes and Redirects](tasks/02-routes-and-redirects.md)
- [x] [Task 03 - Teacher Landing Page](tasks/03-teacher-landing-page.md)
- [x] [Task 04 - Student Landing Page](tasks/04-student-landing-page.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
