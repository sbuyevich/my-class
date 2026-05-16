# My Class Teacher Guide

This folder contains the MacOS version of My Class.

## Run The App

Double-click:

```bat
demo.bat
```

This starts the app with class code `demo`, opens the teacher computer browser, and uses port `5555`.

Replace `demo` with your class code if needed.

Teacher login:

```text
Username: t
Password: t
```

The launcher opens the correct teacher computer URL automatically.
The app home page shows the URL and QR code students can use to open the app on their devices.

> Students must be on the same Wi-Fi/LAN and use the displayed URL.

## Create A New Class

1. Start the app.
2. Sign in as the teacher.
3. Open the `School/Class` page from the app menu.
4. In the `Schools` section, click **Add** and create a school.
5. In the `Classes` section, select the school.
6. Click **Add** and create a class.
7. Enter a class name and a unique class code, for example `S-A-2025`.

Class codes are normalized to uppercase. Use a short, unique code that is easy for students to type. Adding a school or year prefix can help, for example `S-A-2025`.

Give students this link:

```text
http://<teacher-computer-ip>:5555/?c=<class-code>
```

Example:

```text
http://192.168.1.165:5555/?c=S-A-2025
```

You can also copy `demo.bat`, rename it to something like `S-A-2025.bat`, and change this line:

```bat
call run.bat demo %*
```

to:

```bat
call run.bat S-A-2025 %*
```

Then double-click `S-A-2025.bat` to start that class.

To run a class on a different port, pass the port as the second value:

```bat
.\scripts\run.bat S-A-2025 3333
```

Students can register or sign in using the same class code.

## Create A Quiz

Quizzes are stored here:

```text
assets\quizzes
```

Create one folder for each quiz:

```text
assets\quizzes\my-quiz\
  quiz.json
  01\
    q.json
    q.jpg
    a.jpg
  02\
    q.json
    q.jpg
    a.jpg
```

Use numbered folders such as `01`, `02`, `03` for question order.

## quiz.json

Create `quiz.json` in the quiz folder:

```json
{
  "name": "My Quiz",
  "title": "My Classroom Quiz",
  "timeLimitSeconds": 30,
  "answerCount": 4
}
```

Values:

- `name`: quiz name shown in the teacher quiz selector.
- `title`: title shown on teacher and student screens.
- `timeLimitSeconds`: default timer for each question, in seconds.
- `answerCount`: optional default number of answer buttons. Use `1`, `2`, `3`, or `4`. If omitted, the app uses `4`.

## q.json

Create `q.json` inside each question folder:

```json
{
  "question": "Which planet is known as the Red Planet?",
  "correctAnswer": "2",
  "timeLimitSeconds": 10,
  "answerCount": 4
}
```

Values:

- `question`: question title shown in the app.
- `correctAnswer`: correct answer button number as text, such as `"1"`, `"2"`, `"3"`, or `"4"`.
- `timeLimitSeconds`: optional timer for this question only. If omitted, the app uses `quiz.json` `timeLimitSeconds`.
- `answerCount`: optional number of answer buttons for this question only. If omitted, the app uses the quiz default.

## Images

Each question folder needs:

- `q.jpg`: question image shown while students answer.
- `a.jpg`: answer image shown after the teacher clicks **Show Answer**.

`q.jpeg` also works for question images, but answer images should be named `a.jpg`.

## After Adding Quizzes

Restart the app after adding or changing quiz files. The new quiz should appear in the teacher quiz selector.

If students cannot open the link, check:

- They are on the same Wi-Fi/LAN as the teacher computer.
- Windows Firewall allows the app or port `5555`.
- The school or guest Wi-Fi is not blocking device-to-device connections.
