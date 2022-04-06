using JITECEntity;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace JITECKakomonFunctions
{
    public class TweetNewKakomon
    {
        [FunctionName("TweetNewKakomon")]
        public async Task RunAsync([QueueTrigger("new-tweet", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"TweetNewKakomon#RunAcync: {myQueueItem}");

            string consumerKey = "xxx";
            string consumerSecret = "xxx";
            string accessKey = "xxx-xxx";
            string accessSecret = "xxx";
            var appClient = new TwitterClient(consumerKey, consumerSecret, accessKey, accessSecret);

            //var blobServiceClient = new BlobServiceClient("");
            var questionsUri = "https://ipakakomon.blob.core.windows.net/ipa-kakomon/2021r03a_ap/2021r03a_ap_am_qs.json";
            var httpClient = new HttpClient();
            var examPart = JsonConvert.DeserializeObject<ExamPart>(await httpClient.GetStringAsync(questionsUri));
            var question = examPart.Questions.First();
            var qImageUri = $"https://ipakakomon.blob.core.windows.net/ipa-kakomon/{question.QuestionImagePath}";
            var tweetText = $"#{examPart.ExamId} #{examPart.ExamPartId} 問.{question.No}\n{question.QuestionText.Substring(0, 80)}……";
            var pollOptions = new List<string>() { "ア", "イ", "ウ", "エ" };

            var uploadedImage = await appClient.Upload.UploadTweetImageAsync(
                new UploadTweetImageParameters(await httpClient.GetByteArrayAsync(qImageUri))
                {
                });
            log.LogInformation($"uploadedImage: {uploadedImage}");
            var questionTweet = await appClient.Tweets.PublishTweetAsync(new PublishTweetParameters(tweetText)
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
            var pollTweet = await appClient.Execute.RequestAsync(pollRequest =>
            {
                pollRequest.Url = "https://api.twitter.com/2/tweets";
                pollRequest.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                pollRequest.HttpContent = pollContent;
            });
            log.LogInformation($"pollTweet: {pollTweet?.Response?.Content}");
        }
    }
}
