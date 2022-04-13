using Azure.Storage.Blobs;
using JITECEntity;

namespace JITECKakomonFunctions
{
    public class Repository : IRepository
    {
        private string _containerName;
        private BlobContainerClient _blobContainerClient;

        public Repository(BlobServiceClient blobServiceClient, string containerName)
        {
            _containerName = containerName;
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        }

        private string ToExamPartBlobName(string examId, string examPartId)
        {
            return $"{examId}/{examPartId}.json";
        }
        private BlobClient ToExamPartBlob(string examId, string examPartId)
        {
            return _blobContainerClient.GetBlobClient(ToExamPartBlobName(examId, examPartId));
        }
        private string ToExamPartOnTwitterBlobName(string examId, string examPartId)
        {
            return $"{examId}/{examPartId}-twitter.json";
        }
        private BlobClient ToExamPartOnTwitterBlob(string examId, string examPartId)
        {
            return _blobContainerClient.GetBlobClient(ToExamPartOnTwitterBlobName(examId, examPartId));
        }

        public async Task<ExamPart> GetExamPartAsync(string examId, string examPartId)
        {
            var blob = ToExamPartBlob(examId, examPartId);
            var content = await blob.DownloadContentAsync();
            return content.Value.Content.ToObjectFromJson<ExamPart>();
        }

        public async Task<ExamPartOnTwitter?> GetExamPartOnTwitterAsync(string examId, string examPartId)
        {
            var blob = ToExamPartOnTwitterBlob(examId, examPartId);
            if (!await blob.ExistsAsync()) { return null; }
            var content = await blob.DownloadContentAsync();
            return content.Value.Content.ToObjectFromJson<ExamPartOnTwitter>();
        }

        public async Task SaveExamPartAsync(ExamPart examPart)
        {
            var blob = ToExamPartBlob(examPart.ExamId, examPart.ExamPartId);
            await blob.UploadAsync(new BinaryData(examPart), true);
        }

        public async Task SaveExamPartOnTwitterAsync(ExamPartOnTwitter examPartOnTwitter)
        {
            var blob = ToExamPartOnTwitterBlob(examPartOnTwitter.ExamId, examPartOnTwitter.ExamPartId);
            await blob.UploadAsync(new BinaryData(examPartOnTwitter), true);
        }

        public async Task<byte[]> GetQuestionImageBinAsync(Question question)
        {
            var blob = _blobContainerClient.GetBlobClient(question.QuestionImagePath);
            return (await blob.DownloadContentAsync()).Value.Content.ToArray();
        }
    }
}
