using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tweetinvi;

var buildder = new HostBuilder();
var host =buildder
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();