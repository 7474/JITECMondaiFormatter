using System.Collections.Generic;

namespace JITECMondaiFormatter
{
    public record Input(IEnumerable<InputItem> Items)
    {
    }

    public record InputItem(
        string ExamId,
        string ExamPartId,
        // 出典などに使用する試験名
        string ExamRefName,
        string QuestionFilePath, 
        string AnswerFilePath
        )
    {
    }
}
