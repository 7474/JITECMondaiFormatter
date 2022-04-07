using System;
using System.Collections.Generic;

namespace JITECEntity
{
    public record Exam(
        string ExamId,
        IList<ExamPart> Items
        )
    {
    }

    public record ExamPart(
        string ExamId,
        string ExamPartId,
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
}
