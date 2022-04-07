using System.Threading.Tasks;

namespace JITECEntity
{
    public interface IRepository
    {
        Task<ExamPart> GetExamPartAsync(string examId, string examPartId);
        Task SaveExamPartAsync(ExamPart examPart);

        Task<byte[]> GetQuestionImageBinAsync(Question question);

        Task<ExamPartOnTwitter> GetExamPartOnTwitterAsync(string examId, string examPartId);
        Task SaveExamPartOnTwitterAsync(ExamPartOnTwitter examPartOnTwitter);
    }
}
