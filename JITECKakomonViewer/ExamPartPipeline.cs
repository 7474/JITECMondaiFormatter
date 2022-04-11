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
    public class ExamPartPipeline : Pipeline
    {
        public ExamPartPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadExamParts(new NormalizedPath("input/2021r03a.json"))
            };

            ProcessModules = new ModuleList
            {
                new MergeContent(new ReadFiles("ExamPart.cshtml")),
                new RenderRazor().WithModel(Config.FromDocument((doc, context) =>
                {
                    return doc.Get<ExamPartViewModel>("Model");
                })),
                new SetDestination(Config.FromDocument((doc, ctx) =>
                {
                    var vm =  doc.Get<ExamPartViewModel>("Model");
                    return new NormalizedPath($"exam/{vm.ExamPartResult.ExamPart.ExamPartId}.html");
                }))
            };

            OutputModules = new ModuleList {
                  new WriteFiles()
            };
        }
    }
}
