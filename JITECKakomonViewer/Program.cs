using System.Threading.Tasks;
using Statiq.App;
using Statiq.Web;

namespace JITECKakomonViewer
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
          await Bootstrapper
            .Factory
            .CreateDefault(args)
            .AddHostingCommands()
            .RunAsync();
    }
}