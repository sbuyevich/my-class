namespace MyClass.Core.Models;

/// <summary>
/// Snapshot used by the student quiz answer page. The boolean flags describe the
/// current page mode: waiting, answering, submitted, missed, or reveal.
/// </summary>
/// <param name="HasInProgressAnswer">
/// True only while the student may still choose an answer for the current question.
/// </param>
/// <param name="AlreadyAnswered">
/// True after the student submitted an answer, including during reveal mode.
/// </param>
/// <param name="FailedNoAnswer">
/// True when the question closed before this student submitted an answer.
/// </param>
/// <param name="Message">
/// Primary status text shown to the student for the current page mode.
/// </param>
/// <param name="IsAnswerRevealed">
/// True after the teacher reveals the answer outcome for this question.
/// </param>
/// <param name="RevealMessage">
/// Result-specific message shown only in reveal mode.
/// </param>
/// <param name="CurrentQuestionIsInProgress">
/// True while the active question still has time remaining and open answer rows.
/// </param>
/// <param name="QuestionProgress">
/// Per-question progress strip state, ordered by the quiz content definition.
/// </param>
public sealed record QuizAnswerPageState(
    bool HasInProgressAnswer,
    bool AlreadyAnswered,
    bool FailedNoAnswer,
    string Message,
    string QuizTitle,
    string? ActiveQuizPath,
    string? QuestionKey,
    string? QuestionTitle,
    int? QuestionIndex,
    int? QuestionCount,
    bool IsAnswerRevealed,
    bool? IsCorrect,
    DateTime? AnsweredAtUtc,
    TimeSpan? AnswerElapsed,
    string? RevealMessage,
    bool CurrentQuestionIsInProgress,
    TimeSpan CurrentQuestionRemaining,
    IReadOnlyList<QuizQuestionProgressItem> QuestionProgress,
    IReadOnlyList<string> AnswerChoices);

/// <summary>
/// One item in the student progress strip for a quiz question.
/// </summary>
public sealed record QuizQuestionProgressItem(
    int QuestionIndex,
    bool IsCurrent,
    bool IsBold,
    QuizQuestionProgressResult Result);

/// <summary>
/// Visual result state for one question in the student progress strip.
/// </summary>
public enum QuizQuestionProgressResult
{
    // Not reached yet, not revealed yet, or no answer row exists for this student.
    NotAnswered,

    // The student submitted an answer, but the teacher has not revealed correctness.
    Answered,

    // The answer was submitted and matched the configured correct answer.
    Correct,

    // The answer was submitted but did not match the configured correct answer.
    Incorrect,

    // The question closed without a submitted answer.
    Missed
}


