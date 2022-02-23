using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JITECMondaiFormatter
{
    public record Question(
        int No,
        string QuestionText,
        string QUestionImagePath,
        string AnswerText
        )
    {
    }
}
