using System.Threading.Tasks;

namespace JITECEntity
{
    public interface IRepository
    {
        Task<ExamPart> GetExamPartAsync(string examId, string examPartId);
        Task SaveExamPartAsync(ExamPart examPart);

        Task<ExamPartOnTwitter> GetExamPartOnTwitterAsync(string examId, string examPartId);
        Task SaveExamPartAsync(ExamPartOnTwitter examPart);
    }
}
