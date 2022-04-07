using Azure.Storage.Blobs;
using JITECEntity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace JITECKakomonFunctions
{
    public class Functions
    {
        private Repository _repository;
        private ITwitterClient _twitterClient;

        public Functions(IConfiguration config)
        {
            string consumerKey = config.GetValue<string>("TWITTER_API_KEY");
            string consumerSecret = config.GetValue<string>("TWITTER_API_KEY_SECRET");
            string accessKey = config.GetValue<string>("TWITTER_ACCESS_KEY");
            string accessSecret = config.GetValue<string>("TWITTER_ACCESS_KEY_SECRET");
            _twitterClient = new TwitterClient(consumerKey, consumerSecret, accessKey, accessSecret);

            string storageConnectionString = config.GetValue<string>("STORAGE_CONNECTION_STRING");
            string containerName = config.GetValue<string>("BLOB_CONTAINER_NAME");
            var blobClient = new BlobServiceClient(storageConnectionString);
            _repository = new Repository(blobClient, containerName);
        }

        public record TweetNewKakomonRequest(string ExamId, string ExamPartId, int QuestionNo) { }

        [Function("TweetNewKakomon")]
        public async Task<HttpResponseData> RunTweetNewKakomonAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            ILogger log = executionContext.GetLogger(this.GetType().Name);

            log.LogInformation($"Start RunTweetNewKakomonAsync: {req}");

            var reqBody = await req.ReadFromJsonAsync<TweetNewKakomonRequest>();
            string examId = reqBody.ExamId;// "2021r03a_ap";
            string examPartId = reqBody.ExamPartId; // "2021r03a_ap_am_qs";
            int questionNo = reqBody.QuestionNo;

            var examPart = await _repository.GetExamPartAsync(examId, examPartId);
            var question = examPart.Questions.First(x => x.No == questionNo);

            var tweetText = $"#{examPart.ExamId} #{examPart.ExamPartId} 問.{question.No}\n{question.QuestionText.Substring(0, 80)}……";
            var pollOptions = new List<string>() { "ア", "イ", "ウ", "エ" };

            var uploadedImage = await _twitterClient.Upload.UploadTweetImageAsync(
                new UploadTweetImageParameters(await _repository.GetQuestionImageBinAsync(question))
                {
                });
            log.LogInformation($"uploadedImage: {uploadedImage}");
            var questionTweet = await _twitterClient.Tweets.PublishTweetAsync(new PublishTweetParameters(tweetText)
            {
                Medias = { uploadedImage }
            });
            log.LogInformation($"questionTweet: {questionTweet}");

            // v2
            var pollPayload = new TwitterApiV2.Body4()
            {
                Text = tweetText,
                Reply = new TwitterApiV2.Reply()
                {
                    In_reply_to_tweet_id = questionTweet.IdStr,
                },
                Poll = new TwitterApiV2.Poll2()
                {
                    Options = pollOptions,
                    Duration_minutes = 60 * 24,
                },
            };
            var pollPayloadJson = JsonConvert.SerializeObject(pollPayload);
            var pollContent = new StringContent(pollPayloadJson, Encoding.UTF8, "application/json");
            var pollTweet = await _twitterClient.Execute.RequestAsync(pollRequest =>
            {
                pollRequest.Url = "https://api.twitter.com/2/tweets";
                pollRequest.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                pollRequest.HttpContent = pollContent;
            });
            log.LogInformation($"pollTweet: {pollTweet?.Response?.Content}");
            var pollTweetObj = JsonConvert.DeserializeObject<TwitterApiV2.TweetCreateResponse>(pollTweet.Response.Content);

            var onTwitter = (await _repository.GetExamPartOnTwitterAsync(examId, examPartId)) ?? new ExamPartOnTwitter(
                examId, examPartId, new List<QuestionOnTwitter>(), 1
                );
            var onTwitterAdded = onTwitter.PutQuestion(new QuestionOnTwitter(
                question.No,
                new TwitterQuestion(
                    questionTweet.IdStr,
                    pollTweetObj.Data.Id,
                    DateTime.UtcNow
                ),
                null));
            await _repository.SaveExamPartOnTwitterAsync(onTwitterAdded);

            log.LogInformation($"End RunTweetNewKakomonAsync");

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
