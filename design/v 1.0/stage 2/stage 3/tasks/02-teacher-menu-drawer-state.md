# Task 02 - Teacher Menu Drawer State

## Goal

Keep the teacher menu drawer open after teacher login and stored-login restore while preserving the hidden student menu.

## Work

- Ensure teacher login leaves the drawer open when the app reaches the teacher landing page.
- Ensure stored teacher login state also shows the drawer open after login state is restored.
- Ensure student login and stored student login state continue to show no menu button and no drawer.
- Preserve public auth route behavior.
- Avoid introducing route-specific drawer hacks unless needed by the existing layout lifecycle.

## Deliverables

- Teacher drawer remains open after teacher authentication.
- Student users continue to have no app navigation surface.
- Public login/register pages remain free of the app drawer.

## Acceptance Criteria

- Teacher login redirects to `/teacher` with the drawer open.
- Refreshing or reopening with stored teacher login state shows the teacher drawer open.
- Student login redirects to `/student` with no menu button and no drawer.
- Stored student login state shows no menu button and no drawer.
- Closing and reopening the teacher drawer still works from the menu button.

## Notes

- Keep the current teacher-only navigation guard.
- Do not reintroduce student navigation links.
