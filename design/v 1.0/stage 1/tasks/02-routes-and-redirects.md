# Task 02 - Routes and Redirects

## Goal

Replace Home-page navigation with role-specific landing routes and redirects.

## Work

- Remove the `/home` page route and Home navigation item.
- Add a teacher landing route at `/teacher`.
- Add a student landing route at `/student`.
- Update successful login redirects:
  - Teachers navigate to `/teacher`.
  - Students navigate to `/student`.
- Update existing stored-login redirect behavior to use the same role-specific destinations.
- Ensure direct route access respects the current login role.

## Deliverables

- Teacher landing route.
- Student landing route.
- Updated login redirect behavior.
- Removed Home navigation behavior.

## Acceptance Criteria

- Teacher login redirects to `/teacher`.
- Student login redirects to `/student`.
- Existing stored teacher login state redirects to `/teacher`.
- Existing stored student login state redirects to `/student`.
- `/home` is no longer used as a landing page.
- The navigation menu no longer shows a Home item.

## Notes

- Reuse existing login state and role information.
- Keep current class-context behavior intact.
