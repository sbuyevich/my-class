using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;
using MyClass.Core.Options;

namespace MyClass.Core.Services;

// service for managing student quiz answers,
// including retrieving the current state of the quiz answer page and submitting answers.
// This service interacts with the database to validate student access, load quiz content, track progress, and handle answer submissions.
public sealed class QuizAnswerService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService,
    IActiveQuizSelectionService activeQuizSelectionService,
    IQuizNotificationService quizNotificationService,
    IOptions<QuizOptions> quizOptions) : IQuizAnswerService
{
    public async Task<Result<QuizAnswerPageState>> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        // only students can answer quizzes, so validate the login state and class context
        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Value is null)
        {
            return Result<QuizAnswerPageState>.Failure(studentResult.Message);
        }

        var activeQuizPath = activeQuizSelectionService.GetSelectedQuizPath(currentClass.ClassId);

        var contentResult = await quizContentService.LoadQuizAsync(activeQuizPath, cancellationToken);

        if (!contentResult.Succeeded || contentResult.Value is null)
        {
            // No-content mode: the student is valid, but the selected quiz cannot
            // be loaded. Keep the page renderable so the UI can show the message.
            return Result<QuizAnswerPageState>.Success(value: CreateState(
                hasInProgressAnswer: false,
                alreadyAnswered: false,
                failedNoAnswer: false,
                message: contentResult.Message,
                activeQuizPath: activeQuizPath));
        }

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Value,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            // Waiting mode: no teacher-started answer rows exist yet, so the page
            // shows the quiz title and an inactive progress strip.
            return Result<QuizAnswerPageState>.Success(value: CreateState(
                hasInProgressAnswer: false,
                alreadyAnswered: false,
                failedNoAnswer: false,
                message: "Waiting for the teacher to start the first question.",
                quizTitle: contentResult.Value.Title,
                progressItems: CreateNeutralProgress(contentResult.Value),
                activeQuizPath: activeQuizPath));
        }

        var answerChoices = CreateAnswerChoices(current.AnswerCount);
        var progressItems = await BuildQuestionProgressAsync(
            dbContext,
            contentResult.Value,
            current,
            studentResult.Value.Id,
            cancellationToken);

        var answer = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(
                    answer =>
                    answer.QuestionIndex == current.QuestionIndex &&
                    answer.QuestionKey == current.QuestionKey &&
                    answer.StudentId == studentResult.Value.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (current.IsExpired && !current.IsAnswerRevealed)
        {
            // The teacher may not have an open page when the timer expires, so the
            // student view also closes stale answer rows before rendering state.
            await FinishExpiredQuestionAsync(
                dbContext,
                current,
                currentClass.ClassId,
                cancellationToken);
            current = current with
            {
                IsInProgress = false,
                Remaining = TimeSpan.Zero
            };

            progressItems = await BuildQuestionProgressAsync(
                dbContext,
                contentResult.Value,
                current,
                studentResult.Value.Id,
                cancellationToken);

            answer = await dbContext.QuizAnswers
                .AsNoTracking()
                .Where(
                    answer =>
                        answer.QuestionIndex == current.QuestionIndex &&
                        answer.QuestionKey == current.QuestionKey &&
                        answer.StudentId == studentResult.Value.Id)
                .OrderByDescending(answer => answer.StartedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (current.IsAnswerRevealed)
        {
            var hasAnswered = answer is not null && answer.Answer.Length > 0;
            var isCorrect = hasAnswered ? answer!.IsCorrect : null as bool?;

            // Reveal mode: answer buttons are disabled and the student sees either
            // their correctness result or the no-answer message for this question.
            return Result<QuizAnswerPageState>.Success(value:
                CreateState(
                    hasInProgressAnswer: false,
                    alreadyAnswered: hasAnswered,
                    failedNoAnswer: !hasAnswered,
                    message: GetRevealMessage(isCorrect),
                    quizTitle: contentResult.Value.Title,
                    currentQuestion: current,
                    answerChoices: answerChoices,
                    progressItems: progressItems,
                    isCorrect: isCorrect,
                    answeredAtUtc: hasAnswered ? answer!.EndedAtUtc : null,
                    answerElapsed: hasAnswered && answer!.EndedAtUtc is not null
                        ? answer.EndedAtUtc.Value - answer.StartedAtUtc
                        : null,
                    activeQuizPath: activeQuizPath));
        }

        if (answer is null)
        {
            // Waiting mode after a current question lookup usually means the page
            // refreshed between teacher actions before this student's row existed.
            return Result<QuizAnswerPageState>.Success(value:
                CreateState(
                    hasInProgressAnswer: false,
                    alreadyAnswered: false,
                    failedNoAnswer: false,
                    message: "Waiting for the teacher to start the first question.",
                    quizTitle: contentResult.Value.Title,
                    progressItems: progressItems,
                    activeQuizPath: activeQuizPath));
        }

        if (answer.Answer.Length > 0)
        {
            var submittedMessage = GetSubmittedAnswerMessage(answer.Answer, current);

            // Submitted mode: the student has locked in an answer, but the teacher
            // has not revealed results or advanced to another question yet.
            return Result<QuizAnswerPageState>.Success(value:
                CreateState(
                    hasInProgressAnswer: false,
                    alreadyAnswered: true,
                    failedNoAnswer: false,
                    message: submittedMessage,
                    quizTitle: contentResult.Value.Title,
                    currentQuestion: current,
                    answerChoices: answerChoices,
                    progressItems: progressItems,
                    activeQuizPath: activeQuizPath));
        }

        if (answer.EndedAtUtc is not null)
        {
            // Missed mode: the row was closed with no answer, either by timeout or
            // by all-student completion, and reveal has not happened yet.
            return Result<QuizAnswerPageState>.Success(value:
                CreateState(
                    hasInProgressAnswer: false,
                    alreadyAnswered: false,
                    failedNoAnswer: true,
                    message: "This question has finished.",
                    quizTitle: contentResult.Value.Title,
                    currentQuestion: current,
                    progressItems: progressItems,
                    activeQuizPath: activeQuizPath));
        }

        // Answering mode: the question is open, this student's row is still open,
        // and the UI should render active answer choices with the countdown.
        return Result<QuizAnswerPageState>.Success(value:
            CreateState(
                hasInProgressAnswer: true,
                alreadyAnswered: false,
                failedNoAnswer: false,
                message: "Choose an answer.",
                quizTitle: contentResult.Value.Title,
                currentQuestion: current,
                answerChoices: answerChoices,
                progressItems: progressItems,
                activeQuizPath: activeQuizPath));
    }

    public async Task<Result<bool>> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default)
    {
        var selectedAnswerText = selectedAnswer.Trim();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Value is null)
        {
            return Result<bool>.Failure(studentResult.Message);
        }

        var activeQuizPath = activeQuizSelectionService.GetSelectedQuizPath(currentClass.ClassId);

        var contentResult = await quizContentService.LoadQuizAsync(activeQuizPath, cancellationToken);

        if (!contentResult.Succeeded || contentResult.Value is null)
        {
            return Result<bool>.Failure(contentResult.Message);
        }

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Value,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return Result<bool>.Failure("No question is available yet. Wait for the teacher to start.");
        }

        if (!IsAnswerInRange(selectedAnswerText, current.AnswerCount))
        {
            return Result<bool>.Failure($"Answer must be between 1 and {current.AnswerCount}.");
        }

        // A submit request may arrive after the page rendered but before the user
        // clicked; close the question first so every student row shares final state.
        if (current.IsExpired)
        {
            await FinishExpiredQuestionAsync(
                dbContext,
                current,
                currentClass.ClassId,
                cancellationToken);

            return Result<bool>.Failure("This question has finished.");
        }

        var answer = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey &&
                answer.StudentId == studentResult.Value.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (answer is null)
        {
            return Result<bool>.Failure("No answer record is available yet. Wait for the teacher to start.");
        }

        if (answer.Answer.Length > 0)
        {
            return Result<bool>.Failure("You already answered this question.");
        }

        if (answer.EndedAtUtc is not null)
        {
            return Result<bool>.Failure("This question has finished.");
        }

        var submittedAtUtc = DateTime.UtcNow;

        // Submitting closes only this student's row. The answer remains private
        // until the teacher reveals results for the whole question.
        answer.Answer = selectedAnswerText;
        answer.EndedAtUtc = submittedAtUtc;
        answer.IsCorrect = string.Equals(selectedAnswerText, answer.CorrectAnswer, StringComparison.Ordinal);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (await CompleteQuestionIfAllStudentsAnsweredAsync(
            dbContext,
            current,
            currentClass.ClassId,
            cancellationToken))
        {
            await quizNotificationService.NotifyQuizStateChangedAsync(currentClass, cancellationToken);
        }

        return Result<bool>.Success(true, "Answer submitted.");
    }

    private static async Task<Result<Student>> ValidateStudentAccessAsync(
        ApplicationDbContext dbContext,
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken)
    {
        if (loginState is null)
        {
            return Result<Student>.Failure("Sign in as a student to answer quizzes.");
        }

        if (loginState.IsTeacher)
        {
            return Result<Student>.Failure("Teachers cannot submit student quiz answers.");
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return Result<Student>.Failure("Sign in as a student for this class to answer quizzes.");
        }

        var normalizedUserName = loginState.UserName.Trim().ToLower();

        var student = await dbContext.Students
            .SingleOrDefaultAsync(
                student =>
                    student.ClassId == currentClass.ClassId &&
                    student.UserName.ToLower() == normalizedUserName,
                cancellationToken);

        return student is null
            ? Result<Student>.Failure("Student login is required to answer quizzes.")
            : Result<Student>.Success(student);
    }

    private static async Task<CurrentQuestion?> GetCurrentQuestionAsync(
        ApplicationDbContext dbContext,
        QuizContent quiz,
        int classId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        // look up the latest question for this class by finding the most recent QuizAnswer row,
        // which is created when the teacher starts a question
        var latestQuestion = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .ThenByDescending(answer => answer.QuestionIndex)
            .Select(answer => new
            {
                answer.QuestionIndex,
                answer.QuestionKey,
                answer.QuestionText
            })
            .FirstOrDefaultAsync(cancellationToken);

        // QuizAnswers are created per student when the teacher starts a question.
        // The newest answer batch is therefore the source of truth for the active
        // class question, even before any student submits an answer.
        if (latestQuestion is null)
        {
            return null;
        }

        var rows = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId &&
                answer.QuestionIndex == latestQuestion.QuestionIndex &&
                answer.QuestionKey == latestQuestion.QuestionKey)
            .Select(answer => new
            {
                answer.StartedAtUtc,
                answer.EndedAtUtc,
                answer.AnswerRevealedAtUtc
            })
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
        {
            return null;
        }

        var questionContent = quiz.Questions.FirstOrDefault(question =>
            question.Index == latestQuestion.QuestionIndex &&
            string.Equals(question.Key, latestQuestion.QuestionKey, StringComparison.OrdinalIgnoreCase));

        var startedAtUtc = rows.Min(row => row.StartedAtUtc);
        var timeoutSeconds = questionContent?.TimeoutSeconds ?? quiz.TimeLimitSeconds;
        var hasOpenAnswers = rows.Any(row => row.EndedAtUtc is null);
        var isExpired = now >= startedAtUtc.AddSeconds(timeoutSeconds);

        // A reveal timestamp on any student row means the question result is public
        // for the whole class; teacher actions update the rows as a group.
        var answerRevealedAtUtc = rows
            .Where(row => row.AnswerRevealedAtUtc is not null)
            .Select(row => row.AnswerRevealedAtUtc)
            .Max();
        var remaining = isExpired || !hasOpenAnswers
            ? TimeSpan.Zero
            : startedAtUtc.AddSeconds(timeoutSeconds) - now;

        return new CurrentQuestion(
            latestQuestion.QuestionIndex,
            latestQuestion.QuestionKey,
            questionContent?.Title ?? latestQuestion.QuestionText,
            questionContent?.AnswerCount ?? 4,
            quiz.Questions.Count,
            timeoutSeconds,
            startedAtUtc,
            answerRevealedAtUtc,
            hasOpenAnswers && !isExpired,
            remaining,
            isExpired);
    }

    private static QuizAnswerPageState CreateState(
        bool hasInProgressAnswer,
        bool alreadyAnswered,
        bool failedNoAnswer,
        string message,
        string quizTitle = "Quiz Answer",
        CurrentQuestion? currentQuestion = null,
        IReadOnlyList<string>? answerChoices = null,
        IReadOnlyList<QuizQuestionProgressItem>? progressItems = null,
        bool? isCorrect = null,
        DateTime? answeredAtUtc = null,
        TimeSpan? answerElapsed = null,
        string? activeQuizPath = null)
    {
        return new QuizAnswerPageState(
            hasInProgressAnswer,
            alreadyAnswered,
            failedNoAnswer,
            message,
            quizTitle,
            activeQuizPath,
            currentQuestion?.QuestionKey,
            currentQuestion?.Title,
            currentQuestion?.QuestionIndex,
            currentQuestion?.QuestionCount,
            currentQuestion?.IsAnswerRevealed == true,
            isCorrect,
            answeredAtUtc,
            answerElapsed,
            currentQuestion?.IsAnswerRevealed == true ? message : null,
            currentQuestion?.IsInProgress == true,
            currentQuestion?.Remaining ?? TimeSpan.Zero,
            progressItems ?? [],
            answerChoices ?? []);
    }

    private static IReadOnlyList<QuizQuestionProgressItem> CreateNeutralProgress(QuizContent quiz)
    {
        return quiz.Questions
            .Select(question => new QuizQuestionProgressItem(
                question.Index,
                false,
                false,
                QuizQuestionProgressResult.NotAnswered))
            .ToList();
    }

    private static async Task<IReadOnlyList<QuizQuestionProgressItem>> BuildQuestionProgressAsync(
        ApplicationDbContext dbContext,
        QuizContent quiz,
        CurrentQuestion currentQuestion,
        int studentId,
        CancellationToken cancellationToken)
    {
        var answerRows = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer => answer.StudentId == studentId)
            .Select(answer => new
            {
                answer.QuestionIndex,
                answer.Answer,
                answer.EndedAtUtc,
                answer.AnswerRevealedAtUtc,
                answer.IsCorrect,
                answer.StartedAtUtc
            })
            .ToListAsync(cancellationToken);

        var answers = answerRows
            .GroupBy(answer => answer.QuestionIndex)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(answer => answer.StartedAtUtc)
                    .First());

        // Keep the progress strip aligned to the quiz definition, but decorate each
        // question with the student's latest saved answer row when one exists.
        return quiz.Questions
            .Select(question =>
            {
                var isCurrent = question.Index == currentQuestion.QuestionIndex;
                var isPast = question.Index < currentQuestion.QuestionIndex;
                var isClosedFinalQuestion =
                    isCurrent &&
                    question.Index >= currentQuestion.QuestionCount - 1 &&
                    !currentQuestion.IsInProgress;
                var isResultVisible = isPast || isClosedFinalQuestion;
                answers.TryGetValue(question.Index, out var answer);

                var result = GetQuestionProgressResult(
                    answer?.Answer,
                    answer?.EndedAtUtc,
                    answer?.AnswerRevealedAtUtc,
                    answer?.IsCorrect,
                    isResultVisible);

                return new QuizQuestionProgressItem(
                    question.Index,
                    isCurrent,
                    question.Index <= currentQuestion.QuestionIndex,
                    result);
            })
            .ToList();
    }

    private static QuizQuestionProgressResult GetQuestionProgressResult(
        string? answer,
        DateTime? endedAtUtc,
        DateTime? answerRevealedAtUtc,
        bool? isCorrect,
        bool isResultVisible)
    {
        if (answerRevealedAtUtc is null && !isResultVisible)
        {
            if (!string.IsNullOrEmpty(answer))
            {
                return QuizQuestionProgressResult.Answered;
            }

            return endedAtUtc is not null
                ? QuizQuestionProgressResult.Missed
                : QuizQuestionProgressResult.NotAnswered;
        }

        if (endedAtUtc is null)
        {
            return QuizQuestionProgressResult.NotAnswered;
        }

        if (string.IsNullOrEmpty(answer))
        {
            return QuizQuestionProgressResult.Missed;
        }

        return isCorrect == true
            ? QuizQuestionProgressResult.Correct
            : QuizQuestionProgressResult.Incorrect;
    }

    private static IReadOnlyList<string> CreateAnswerChoices(int answerCount)
    {
        return Enumerable.Range(1, answerCount)
            .Select(answer => answer.ToString())
            .ToList();
    }

    private static bool IsAnswerInRange(string answer, int answerCount)
    {
        return int.TryParse(answer, out var answerNumber) &&
            answerNumber >= 1 &&
            answerNumber <= answerCount &&
            string.Equals(answer, answerNumber.ToString(), StringComparison.Ordinal);
    }

    private static string GetSubmittedAnswerMessage(string answer, CurrentQuestion currentQuestion)
    {
        var message = $"Answer {answer} was submitted.";

        return currentQuestion.QuestionIndex >= currentQuestion.QuestionCount - 1
            ? $"{message} It was last question." 
            : $"{message} Waiting for the next question.";
    }

    private string GetRevealMessage(bool? isCorrect)
    {
        var messages = quizOptions.Value.RevealMessages;

        return isCorrect switch
        {
            true => GetConfiguredMessage(messages.Correct, "Correct. Good job!"),
            false => GetConfiguredMessage(messages.Incorrect, "Incorrect. Not this time."),
            _ => GetConfiguredMessage(messages.NoAnswer, "No answer submitted. Stay focused!")
        };
    }

    private static string GetConfiguredMessage(string? configuredMessage, string fallback)
    {
        return string.IsNullOrWhiteSpace(configuredMessage)
            ? fallback
            : configuredMessage.Trim();
    }

    private static async Task<bool> CompleteQuestionIfAllStudentsAnsweredAsync(
        ApplicationDbContext dbContext,
        CurrentQuestion current,
        int classId,
        CancellationToken cancellationToken)
    {
        var answers = await dbContext.QuizAnswers
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId &&
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey)
            .ToListAsync(cancellationToken);

        if (answers.Count == 0 ||
            answers.Any(answer => answer.EndedAtUtc is null) ||
            answers.Any(answer => answer.AnswerRevealedAtUtc is not null))
        {
            return false;
        }

        // When every row is closed, lock in correctness so the reveal state can be
        // shown immediately without recalculating answer outcomes per request.
        foreach (var answer in answers)
        {
            answer.IsCorrect = answer.Answer.Length > 0 &&
                string.Equals(answer.Answer, answer.CorrectAnswer, StringComparison.Ordinal);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static async Task<DateTime> FinishExpiredQuestionAsync(
        ApplicationDbContext dbContext,
        CurrentQuestion current,
        int classId,
        CancellationToken cancellationToken)
    {
        var finishedAtUtc = DateTime.UtcNow;

        var answers = await dbContext.QuizAnswers
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId &&
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey)
            .ToListAsync(cancellationToken);

        // Expiration is represented by closing any still-open rows with the same
        // timestamp, including rows for students who never chose an answer.
        foreach (var answer in answers)
        {
            answer.EndedAtUtc ??= finishedAtUtc;
            answer.IsCorrect = answer.Answer.Length > 0 &&
                string.Equals(answer.Answer, answer.CorrectAnswer, StringComparison.Ordinal);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return finishedAtUtc;
    }

    private sealed record CurrentQuestion(
        int QuestionIndex,
        string QuestionKey,
        string Title,
        int AnswerCount,
        int QuestionCount,
        int TimeoutSeconds,
        DateTime StartedAtUtc,
        DateTime? AnswerRevealedAtUtc,
        bool IsInProgress,
        TimeSpan Remaining,
        bool IsExpired)
    {
        public bool IsAnswerRevealed => AnswerRevealedAtUtc is not null;
    }
}


