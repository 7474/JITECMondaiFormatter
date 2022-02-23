using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JITECMondaiFormatter
{
    public record Output(IEnumerable<OutputItem> Items)
    {
    }

    public record OutputItem(IEnumerable<Question> Questions)
    {
    }
}
