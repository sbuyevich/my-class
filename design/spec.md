# MyClass Product Spec

## Product Thesis

MyClass is a local classroom web app that lets a teacher host quizzes and class interactions from their own Windows or Mac computer. Students join from browsers on the same Wi-Fi/LAN by opening a shared class URL. The app does not require cloud hosting, paid infrastructure, or external accounts.

## Goals

- Provide a free classroom tool that can run from a teacher-owned computer.
- Let students use ordinary browsers without installing client software.
- Support live classroom quiz workflows where teacher and student screens stay synchronized.
- Keep classroom data local to the teacher-hosted app and SQLite database.
- Make startup simple enough for non-technical classroom use: run app, share URL, students join.

## Non-Goals

- Do not require cloud deployment, SaaS hosting, or internet-facing infrastructure.
- Do not require student mobile or desktop app installation.
- Do not attempt to work across different networks without VPN, tunneling, or cloud relay.
- Do not position the first version as a full learning management system.
- Do not depend on school identity providers or ASP.NET Core Identity for the local MVP.

## Users

### Teacher

The teacher runs the app on their computer, shares the class URL, manages schools/classes/students, starts quiz questions, monitors answer status, reveals answers, and reviews/export results.

### Student

The student opens the class URL in a browser, registers or signs in, sees the current quiz question, submits an answer, and sees answer feedback when the teacher reveals it.

## Operating Model

- One teacher computer acts as the local server.
- Student devices are browser clients.
- Server and clients must be on the same LAN/Wi-Fi.
- The teacher shares a URL in this form:

```text
http://<teacher-local-ip>:5555/?c=<class-code>
```

- The class code in the `c` query parameter selects the active class context.
- Packaged launch scripts should prefer the teacher computer's LAN IP over `localhost` and open the browser with the class code already included.

## Network Constraints

MyClass can work on a home Wi-Fi network and on school networks that allow device-to-device traffic. It may fail on school or guest Wi-Fi networks that block local device connections.

Common blockers:

- Teacher computer firewall blocks inbound traffic to the app or port `5555`.
- School Wi-Fi enables client isolation, preventing student devices from connecting to the teacher computer.
- Teacher and students are connected to different networks.
- The app is bound only to `localhost` instead of the teacher computer's LAN IP.

The app and documentation should make this limitation explicit. Troubleshooting should include testing the shared URL from a second device and, on Windows, checking port access with:

```powershell
Test-NetConnection <teacher-local-ip> -Port 5555
```

## Functional Requirements

- The app starts locally on the teacher computer.
- The teacher can open a class URL and authenticate as the teacher with credentials configured in `appsettings.json`.
- The app works only with the class defined by the query parameter in the class URL.
- Because there is no web authentication:
    - The teacher must reset students to `inactive` status.
    - Students can change status to `active` only after signing in or registering.
- Students should open the same class URL and log in or self-register for that class.
- The teacher can manage schools, classes, and students.
- The teacher can select a quiz from local quiz assets.
- The teacher can start, advance, finish, and reveal quiz questions.
- Student quiz screens update during the live quiz flow.
- Student answers are saved to the local SQLite database.
- The teacher can view quiz results and export them as CSV.

## Data And Content Requirements

- SQLite is the local persistence mechanism.
- Schools own classes; classes own students; students own live quiz answer rows.
- Quiz content is local file content under the configured quiz root.
- Quiz folders contain metadata JSON plus question media and metadata.
- Local database and quiz assets must be treated as user-facing data, not disposable build output.

## Security And Trust Model

- MyClass is intended for trusted classroom LAN use, not public internet hosting.
- Browser-stored login state is convenience state only.
- Server-side services must enforce role and class scope for sensitive operations.
- Teacher credentials can be app settings-based for the local MVP.
- Student passwords should be stored as hashes, not plain text.
- The app should not expose arbitrary filesystem paths to browsers.

## Packaging And Distribution

- The app should be publishable into Windows and macOS distribution folders.
- A packaged launcher should start the app, bind it to a reachable local URL, and open the teacher's browser.
- Distribution packages should include app binaries, local database, quiz assets, and launch scripts needed for classroom use.
- The default port is `5555`, with support for overriding it when necessary.

## Success Criteria

- A teacher can run the packaged app on a local computer without cloud setup.
- A student device on the same allowed LAN can open the shared URL and join the correct class.
- A teacher can run a quiz end-to-end with multiple student browsers.
- Quiz answers are persisted locally and visible on the teacher results page.
- The app clearly documents that some school Wi-Fi networks may block LAN access.
