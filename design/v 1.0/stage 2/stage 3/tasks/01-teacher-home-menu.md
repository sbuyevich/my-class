# Task 01 - Teacher Home Menu

## Goal

Add a teacher-only Home menu item that routes to the existing teacher landing page.

## Work

- Add a teacher menu item labeled `Home` or `Teacher Home`.
- Point the menu item to the existing `/teacher` route.
- Preserve the existing teacher landing page content.
- Do not create a new route unless `/teacher` cannot satisfy the requirement.
- Preserve the current student no-menu behavior.

## Deliverables

- Teacher navigation includes a Home/Teacher Home menu item.
- The Home/Teacher Home item routes to `/teacher`.
- Student users still see no app menu or menu button.

## Acceptance Criteria

- Signed-in teachers can navigate to `/teacher` from the menu.
- The teacher home menu item is visible only to teachers.
- The existing teacher landing page still displays the student URL and QR code.
- Student users do not see a menu item for Home, Teacher Home, or `/teacher`.

## Notes

- Prefer reusing the current `/teacher` page instead of creating a new page.
- Keep teacher-only access enforced through existing role-gating behavior.
