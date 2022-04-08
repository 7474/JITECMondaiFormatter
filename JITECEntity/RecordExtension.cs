using System;
using System.Collections.Generic;
using System.Linq;

namespace JITECEntity
{
    public static class RecordExtension
    {
        private static Random random = new Random();

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

        public static T ChoseRandom<T>(this ICollection<T> collection)
        {
            return collection.Skip(random.Next(collection.Count)).First();
        }
    }
}
