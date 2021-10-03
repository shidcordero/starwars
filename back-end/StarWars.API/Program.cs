using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Debugging;
using System;

namespace StarWars.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseSerilog((builder, cfg) =>
                {
                    Config.InitConfiguration(builder.HostingEnvironment);

                    SelfLog.Enable(Console.Error);

                    cfg = cfg
                        .WriteTo.Console();
                })
                .UseStartup<Startup>();
        }
    }
}