using JITECEntity;
using Newtonsoft.Json;

namespace JITECKakomonViewer
{
    public class Repository : IRepository
    {
        private string _baseUrl;
        private HttpClient _http;

        public Repository(HttpClient http, string baseUrl)
        {
            _baseUrl = baseUrl;
            _http = http;
        }

        public async Task<ExamPart> GetExamPartAsync(string examId, string examPartId)
        {
            var content = await _http.GetStringAsync($"{_baseUrl.TrimEnd('/')}/{examId}/{examPartId}.json");
            return JsonConvert.DeserializeObject<ExamPart>(content);
        }

        public async Task<ExamPartOnTwitter?> GetExamPartOnTwitterAsync(string examId, string examPartId)
        {
            try
            {
                var content = await _http.GetStringAsync($"{_baseUrl.TrimEnd('/')}/{examId}/{examPartId}-twitter.json");
                return JsonConvert.DeserializeObject<ExamPartOnTwitter>(content);
            }
            catch { }
            return null;
        }

        public async Task SaveExamPartAsync(ExamPart examPart)
        {
            throw new NotSupportedException();
        }

        public async Task SaveExamPartOnTwitterAsync(ExamPartOnTwitter examPartOnTwitter)
        {
            throw new NotSupportedException();
        }

        public async Task<byte[]> GetQuestionImageBinAsync(Question question)
        {
            throw new NotSupportedException();
        }
    }
}
