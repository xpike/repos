using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using XPike.Configuration.Microsoft.AspNetCore;
using XPike.Logging.Microsoft.AspNetCore;

namespace XPikeRepos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder => builder.AddXPikeLogging())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.AddXPikeConfiguration(xpike => { })
                        .UseStartup<Startup>();
                });
    }
}