using JITECEntity;
using Newtonsoft.Json;
using Statiq.Common;

namespace JITECKakomonViewer
{
    public class ReadExamParts : Module
    {
        private Input input;
        private IRepository _repository;

        public ReadExamParts(NormalizedPath inputFilePath, IRepository repository)
        {
            input = JsonConvert.DeserializeObject<Input>(File.ReadAllText(inputFilePath.FullPath));
            _repository = repository;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteContextAsync(IExecutionContext context)
        {
            // XXX この辺の処理の責務はServiceとして切り出すのがいい感じっぽい
            return input.Items
                .Select(async x =>
                {
                    var examPart = await _repository.GetExamPartAsync(x.ExamId, x.ExamPartId);
                    var examPartOnTwitter = await _repository.GetExamPartOnTwitterAsync(x.ExamId, x.ExamPartId);

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
