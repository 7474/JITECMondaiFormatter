using System.Collections.Generic;

namespace JITECKakomonViewer
{
    public record Input(
        string ImageBaseUrl, 
        IList<InputItem> Items
        )
    {
        public string ToBlobUrl(string path)
        {
            return $"{ImageBaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
        }
    }

    public record InputItem(
        string ExamId,
        string ExamPartId,
        // 出典などに使用する試験名
        string ExamRefName
        )
    {
    }
}
