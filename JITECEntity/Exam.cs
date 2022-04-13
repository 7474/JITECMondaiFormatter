using System;
using System.Collections.Generic;

namespace JITECEntity
{
    public record ExamPartSummary(
        string ExamId,
        string ExamPartId
        )
    {
    }

    public record QuestionSummary(
        string ExamId,
        string ExamPartId,
        int No
        )
    {
    }

    public record Exam(
        string ExamId,
        string Name,
        IList<ExamPart> Items
        )
    {
    }

    public record ExamPart(
        string ExamId,
        string ExamPartId,
        string Name,
        IList<Question> Questions,
        int Version
        )
    {
    }

    public record Question(
        int No,
        string QuestionText,
        string QuestionImagePath,
        string AnswerText
        )
    {
    }

    public record ExamPartOnTwitter(
        string ExamId,
        string ExamPartId,
        IList<QuestionOnTwitter> Questions,
        int Version
        )
    {
    }
    public record QuestionOnTwitter(
        int No,
        TwitterQuestion Question,
        TwitterAnswer Answer
        )
    {
    }
    public record TwitterQuestion(
        string QuestionTweetId,
        string PollTweetId,
        DateTime CreatedAt
        )
    {
    }
    public record TwitterAnswer(
        IList<string> PollOptions,
        IList<int> PollAnswers,
        DateTime CreatedAt
        )
    {
    }

    public record ExamResult(
        Exam Exam,
        ExamPartResult ExamPartResult
        )
    {
    }
    public record ExamPartResult(
        ExamPart ExamPart,
        IList<QuestionResult> Questions
        )
    {
    }
    public record QuestionResult(
        Question Question,
        QuestionOnTwitter QuestionOnTwitter
        )
    {
    }
}
