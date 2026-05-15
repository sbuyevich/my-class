# Task 05 - Responsive Header

## Goal

Make the shared app header usable on mobile without text overflow or crowding.

## Work

- Review the app bar layout for auth, teacher, and student states.
- Make title, greeting, class, and school display responsive on narrow screens.
- Ensure class and school text wrap, truncate, stack, or hide in a predictable way on mobile.
- Preserve the desktop header layout as much as practical.
- Keep the teacher menu button available for teachers and hidden for students.

## Deliverables

- Responsive header behavior for mobile and desktop.
- No overlapping or overflowing header text.
- Teacher/student/auth header states remain recognizable.

## Acceptance Criteria

- On mobile width, header content does not overflow horizontally.
- Header text does not overlap with the menu button, greeting, class, or school chips.
- Teacher users still see the menu button.
- Student users still do not see the menu button.
- Desktop header behavior remains consistent with the current app.

## Notes

- Prefer MudBlazor components, parameters, utility classes, and simple responsive layout changes over custom CSS where practical.
- If CSS is required for responsive behavior, keep it scoped and minimal.
