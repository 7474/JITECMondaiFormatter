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
        IList<Question> Questions
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
}
