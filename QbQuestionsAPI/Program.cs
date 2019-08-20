using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using QbQuestionsAPI.Domain.Services;
using QbQuestionsAPI.Services;

namespace QbQuestionsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(serviceCollection =>
                    serviceCollection.AddScoped<ISecretManagementService, SecretManagementService>())
                .UseStartup<Startup>();
    }
}
