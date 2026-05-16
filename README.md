This repo was created to explore `Agentic Engineering`.

`Agentic Engineering` means using AI agents as part of the software engineering process, not just asking AI to write small code snippets.

## App Creation Flow By Docs

1. [Idea](design/idea.md)
2. [Spec](design/spec.md)
3. [Development](design/development.md)


# MyClass App

MyClass is a .NET 10 Blazor web app for running classroom quizzes. Teachers can manage schools, classes, and students, start quiz questions, and review or export results. Students can register or sign in with a class code and submit quiz answers from their own browser.

## Features

- Teacher and student login flows scoped by class code.
- School, class, and student management backed by SQLite.
- Real-time quiz status updates with SignalR.
- Quiz content loaded from local JSON and image assets.
- MudBlazor UI components.
- Windows and macOS publish targets with packaged `dist-win.zip` and `dist-macos.zip`.

## Download And Run Release Zip Files

1. Open the [MyClass releases page](https://github.com/sbuyevich/my-class/releases).
2. Open the latest release.
3. Download the zip file for the teacher computer:
   - `dist-win.zip` for Windows.
   - `dist-macos.zip` for macOS.
4. Unzip the downloaded file.
5. Open the unzipped `My Class` folder.
6. Open the included `README.md` or `README.pdf` teacher guide.
7. Run the app from that folder by double-clicking:

```bat
demo.bat
```

The launcher starts the app with class code `demo`, opens the teacher computer browser, and uses port `5555`. The app home page shows the URL and QR code students can use from devices on the same Wi-Fi/LAN.

## Network Requirements

MyClass is designed for local classroom use without cloud hosting. Student devices must be able to reach the teacher's computer over the same LAN/Wi-Fi network.

This usually works on a home Wi-Fi network, but school or guest Wi-Fi can block it. Common blockers include:

- Windows or macOS firewall blocking inbound connections to the app or port `5555`.
- School Wi-Fi client isolation, where devices can access the internet but cannot connect to each other.
- Student devices connected to a different Wi-Fi network or a guest network.

To check a network, run the app on the teacher computer and open the shared URL from another device on the same Wi-Fi:

```text
http://<teacher-local-ip>:5555/?c=demo
```

From another Windows laptop, the port can also be tested with:

```powershell
Test-NetConnection <teacher-local-ip> -Port 5555
```

If `TcpTestSucceeded` is `False`, the teacher computer firewall, the school network, or the app binding is blocking access.

## Repository Layout

```text
src/
  MyClass.slnx                 Main solution
  MyClass.Web/                 Blazor app, pages, layout, hubs, static files
  MyClass.Core/                Services, models, EF Core data access, options
  assets/quizzes/              Quiz JSON and media assets
  database/my-class.db         Local SQLite database
scripts/
  publish.ps1                  Publish one target package
  dist.bat                     Build Windows and macOS packages and zip them
dist-win/                      Windows distribution folder
dist-macos/                    macOS distribution folder
design/                        Product, database, stage, and launch notes
```

## Quiz Content

Quiz content is stored under `src/assets/quizzes`. Each quiz folder has a `quiz.json` file and numbered question folders:

```text
src/assets/quizzes/astronomy/
  quiz.json
  01/
    q.json
    q.jpg
    a.jpg
```

Files in each quiz folder:

- `quiz.json`: quiz-level metadata, including the quiz title, default time limit, and question list.
- `01/`, `02/`, etc.: one folder per question. The folder number should match the question order.
- `q.json`: question-level metadata, including the question title, key, answer count, correct answer, and optional timeout.
- `q.jpg`: image shown to students and the teacher while the question is active.
- `a.jpg`: image shown after the teacher clicks **Show Answer**.

Example `quiz.json`:

```json
{
  "name": "Astronomy - easy",
  "title": "How good do you know Astronomy?",
  "timeLimitSeconds": 300,
  "answerCount": 4
}
```

`quiz.json` values:

- `name`: required display name shown in the teacher quiz selector.
- `title`: required title shown on the teacher and student quiz screens.
- `timeLimitSeconds`: required default question timer in seconds. Must be greater than `0`.
- `answerCount`: optional default number of answer buttons for each question. Supported values are `1` to `4`; when omitted, the app uses `4`.

Example `q.json`:

```json
{
  "question": "Which planet is known as the Red Planet?",
  "correctAnswer": "2",
  "timeLimitSeconds": 10,
  "answerCount": 4
}
```

`q.json` values:

- `question`: optional question text shown as the question title. When omitted or empty, the folder name, such as `01`, is used.
- `correctAnswer`: required correct answer button number as text, for example `"1"`, `"2"`, `"3"`, or `"4"`. It must be within the configured answer count.
- `timeLimitSeconds`: optional timer override for this question. When omitted, the app uses `quiz.json` `timeLimitSeconds`.
- `answerCount`: optional answer button count for this question. When omitted, the app uses `quiz.json` `answerCount`, or `4` if no quiz default is set.

JSON property names are case-insensitive, so `timeLimitSeconds` and `TimeLimitSeconds` both work.

The quiz root is configured with `Quiz:RootFolder` in `src/MyClass.Web/appsettings.json`. Currently included quiz sets are:

- `astronomy`
- `csharp`
- `funny`

## Database

The app uses SQLite with this default connection string:

```json
"DefaultConnection": "Data Source=../database/my-class.db"
```

On startup, `DatabaseInitializer` creates missing tables/columns and seeds a demo school/class when the database is empty.

## Create And Release Zip Files 

From a Windows terminal at the repository root, run:

```bat
.\scripts\dist.bat
```

The script publishes both Windows and macOS packages, then creates:

- `dist-win.zip`
- `dist-macos.zip`

Upload zip files in [release](https://github.com/sbuyevich/my-class/releases)

Each zip contains a top-level folder named `My Class`.


## Development Notes

- Keep app host, Razor pages, layout, and hubs in `src/MyClass.Web`.
- Keep shared services, models, options, and EF Core data access in `src/MyClass.Core`.
- Avoid manually editing generated output under `dist-*`, `src/artifacts`, `bin`, or `obj`.
- Design and implementation planning notes live in `design/`.
