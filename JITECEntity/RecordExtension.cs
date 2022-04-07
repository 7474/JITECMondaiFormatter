using System.Linq;

namespace JITECEntity
{
    public static class RecordExtension
    {
        public static ExamPartOnTwitter PutQuestion(this ExamPartOnTwitter examPart, QuestionOnTwitter question)
        {
            return new ExamPartOnTwitter(
                examPart.ExamId,
                examPart.ExamPartId,
                examPart.Questions
                    .Where(x => x.No != question.No || x?.Question.QuestionTweetId != question?.Question.QuestionTweetId)
                    .Append(question)
                    .OrderByDescending(x => x?.Question.CreatedAt)
                    .OrderBy(x => x.No).ToList(),
                examPart.Version + 1
                );
        }
    }
}
