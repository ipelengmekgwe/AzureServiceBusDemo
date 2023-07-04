using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SBReceiver.Services;

namespace SBReceiver
{
    internal class Program
    {
        static async Task Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTransient<IDequeueService, DequeueService>();
                })
                .Build();

            var dequeueService = ActivatorUtilities.CreateInstance<DequeueService>(host.Services);
            await dequeueService.Run();
        }
    }
}