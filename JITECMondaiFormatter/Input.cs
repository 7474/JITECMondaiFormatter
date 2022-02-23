using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JITECMondaiFormatter
{
    public record Input(IEnumerable<InputItem> Items)
    {
    }

    public record InputItem(string QuestionFilePath, string AnswerFilePath)
    {
    }
}
