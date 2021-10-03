using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StarWars.API
{
    internal static class Config
    {
        public static IConfiguration? Configuration { get; set; }

        public static void InitConfiguration(IWebHostEnvironment env)
        {
            // This is only used for unit tests
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}