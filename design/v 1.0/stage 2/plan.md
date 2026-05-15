# Student Page Changes

- Students should use a single Student page at `/student`.
- After student login or registration, route students to `/student`.
- Students should not see an app navigation menu; the Student page should be their single in-app destination.
- The current quiz answer experience should be moved from the `/quiz-answer` page into a reusable component rendered by `/student`.
- `/student` should show a waiting state when no quiz question is active:
  `Please wait until your teacher is ready for the quiz.`
- When the teacher starts or advances a quiz question, `/student` should automatically show the quiz answer component without requiring page refresh.
- Student quiz state should update live through SignalR for question start, question finish, answer reveal, and quiz completion.
- The old `/quiz-answer` route should no longer be the primary student workflow.
