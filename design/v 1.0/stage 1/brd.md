# Change Landing Pages

Create two role-specific landing pages after login: one for teachers and one for students. Remove the existing Home page.

After login:
- Teachers are redirected to `/teacher`.
- Students are redirected to `/student`.

Teachers and students can only access their own landing page.

Remove the `/home` page and Home navigation item.

# Teacher Landing Page

The teacher landing page should help students connect to the class site from their own devices.

Show the site LAN URL and a QR code for the same URL. The URL should use the LAN host detected by the existing `UrlService` and include the current class query parameter, for example `?c=demo`.

The URL and QR code should point students to the student entry/login flow for the current class.

If `UrlService` cannot detect a LAN URL, show a clear error message instead of the QR code.

## Layout

Use a two-column layout:
- Left column: `Please enter this URL in your browser address bar:` and URL on the next line.
- Right column: `Or scan the QR code below with your mobile device:` and QR code on the next line.

# Student Landing Page

Show a message telling the student to wait until the teacher is ready for quiz activity.

