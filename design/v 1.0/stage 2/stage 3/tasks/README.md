# UI Changes Task Breakdown

Implement focused UI fixes for the teacher home/menu experience, teacher quiz start flow, student page display, and responsive header behavior.

## Tasks

1. [x] [Task 01 - Teacher Home Menu](01-teacher-home-menu.md)
2. [x] [Task 02 - Teacher Menu Drawer State](02-teacher-menu-drawer-state.md)
3. [x] [Task 03 - Teacher Quiz Start Button](03-teacher-quiz-start-button.md)
4. [ ] [Task 04 - Student Page Display Cleanup](04-student-page-display-cleanup.md)
5. [ ] [Task 05 - Responsive Header](05-responsive-header.md)
6. [ ] [Task 10 - Verification](10-verification.md)

## Notes

- Teacher menu behavior must remain teacher-only.
- Student users should continue to see no app menu or menu button.
- Use the existing `/teacher` route for Teacher Home unless implementation discovers a concrete blocker.
- Treat the Start button issue as a bug investigation task; do not assume the root cause before inspecting the runtime behavior.
