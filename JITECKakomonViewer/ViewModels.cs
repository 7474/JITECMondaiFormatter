using JITECEntity;

namespace JITECKakomonViewer
{
    public class ExamPartViewModel
    {
        public string ImageBaseUrl { get; private set; }
        public ExamPartResult ExamPartResult { get; private set; }

        public string ToTweetUrl(string tweetId)
        {
            // XXX 他のユーザーのTweetは扱わないが、ユーザー名部分はどんな値でも正しいURLにリダイレクトされる模様
            // （ユーザー名は変更できるためか？）
            return $"https://twitter.com/ipa_kakomon/status/{tweetId}";
        }

        public string ToQuestionImageUrl(ExamPart examPart, Question question)
        {
            return $"{ImageBaseUrl.TrimEnd('/')}/{question.QuestionImagePath}";
        }

        public ExamPartViewModel(
            string imageBaseUrl,
            ExamPartResult examPartResult
            )
        {
            ImageBaseUrl = imageBaseUrl;
            ExamPartResult = examPartResult;
        }
    }
}
