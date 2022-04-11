using JITECEntity;
using Newtonsoft.Json;
using Statiq.Common;

namespace JITECKakomonViewer
{
    public class ReadExamParts : Module
    {
        private Input input;

        public ReadExamParts(NormalizedPath inputFilePath)
        {
            input = JsonConvert.DeserializeObject<Input>(File.ReadAllText(inputFilePath.FullPath));
            Console.WriteLine(inputFilePath);
            Console.WriteLine(JsonConvert.SerializeObject(input));
        }

        public static async Task<T> LoadFromUriAsync<T>(string uri)
        {
            try
            {
                using var http = new HttpClient();
                var content = await http.GetStringAsync(uri);
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch { }
            return default(T);
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            // XXX この辺の処理の責務はServiceとして切り出すのがいい感じっぽい
            return input.Items
                .Select(async x =>
                {
                    var examPart = await LoadFromUriAsync<ExamPart>(input.ToBlobUrl($"{x.ExamId}/{x.ExamPartId}.json"));
                    var examPartOnTwitter = await LoadFromUriAsync<ExamPartOnTwitter>(input.ToBlobUrl($"{x.ExamId}/{x.ExamPartId}-twitter.json"));

                    var qMap = examPart?.Questions.GroupBy(x => x.No).ToDictionary(x => x.Key, x => x.First());
                    var tMap = examPartOnTwitter?.Questions.GroupBy(x => x.No).ToDictionary(x => x.Key, x => x.First());
                    var qNos = qMap.Keys.Concat(tMap?.Keys.ToArray() ?? new int[] { })
                    .Distinct().OrderBy(x => x).ToList();

                    return new ExamPartResult(
                        examPart,
                        qNos.Select(x => new QuestionResult(
                            qMap.ContainsKey(x) ? qMap[x] : null,
                            tMap == null ? null : tMap.ContainsKey(x) ? tMap[x] : null))
                        .Where(x => x.Question != null)
                        .ToList()
                        );
                })
                .Select(x => x.Result)
                .Select(x => new ExamPartViewModel(input.ImageBaseUrl, x))
                .Select(x => context.CreateDocument(
                    x.ExamPartResult.ExamPart.ExamPartId,
                    new[]{
                        new KeyValuePair<string, object>("Type", x.GetType()),
                        new KeyValuePair<string, object>("Model", x),
                        new KeyValuePair<string, object>("Title", x.ExamPartResult.ExamPart.Name),
                    })
                );
        }
    }
}
