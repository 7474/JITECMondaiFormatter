using Newtonsoft.Json;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JITECKakomonViewer
{
    public class ExamPartIndexPipeline : Pipeline
    {
        public ExamPartIndexPipeline()
        {
            Dependencies.Add(typeof(ExamPartPipeline).Name);

            ProcessModules = new ModuleList
            {
                new ReplaceDocuments(
                    Config.FromContext(context =>
                    {
                        var vm = new IndexViewModel(context.Parent.Outputs
                            .Select(x => x.Get<ExamPartViewModel>("Model"))
                            .Where(x => x != null)
                            .ToList());
                        return (IEnumerable<IDocument>) new []
                        {
                            context.CreateDocument(
                                new[]
                                {
                                    new KeyValuePair<string, object>("Model", vm)
                                }
                            )
                        };
                    })
                ),
                new MergeContent(new ReadFiles("_Index.cshtml")),
                new RenderRazor().WithModel(Config.FromDocument((doc, context) =>
                {
                    var vm = doc.Get<IndexViewModel>("Model");
                    return vm;
                })),
                new SetDestination(Config.FromDocument((doc, ctx) =>
                {
                    return new NormalizedPath($"index.html");
                }))
            };

            OutputModules = new ModuleList {
                  new WriteFiles()
            };
        }
    }
}
