using AutoMapper;
using AutoMapper.EquivalencyExpression;
using FluentChoco;
using HotChocolate.AspNetCore;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using StarWars.Core;
using StarWars.Core.Settings;
using StarWars.Core.GraphQL;
using StarWars.ResourceLibrary;
using Serilog;
using StarWarsApiCSharp;
using StarWars.Core.Services;

namespace StarWars.API
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            StaticData.Settings = new PlatformSettings();
            Config.Configuration.Bind(StaticData.Settings);
            services.AddSingleton(StaticData.Settings);

            services
                .AddSingleton<IRepository<Core.Models.Starship>, Repository<Core.Models.Starship>>()
                .AddSingleton<IRepository<Person>, Repository<Person>>()
                .AddSingleton<IStarshipService, StarshipService>();

            services
                .AddCors(options => options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(StaticData.Settings.AllowedOrigins.MakeListString().ToArray())
                            .AllowCredentials();
                    })
                )
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();

            services
                .AddHttpContextAccessor();

            services
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(o =>
                {
                    var supportedCultures = new List<CultureInfo>()
                    {
                        new CultureInfo("en"),
                    };
                    o.DefaultRequestCulture = new RequestCulture("en");
                    o.SupportedCultures = supportedCultures;
                    o.SupportedUICultures = supportedCultures;
                });

            ConfigureHotChocolate(services);

            services.RegisterFluentValidators<Startup>();

            services
                .AddResponseCaching(options => options.UseCaseSensitivePaths = false)
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<BrotliCompressionProvider>();
                    options.Providers.Add<GzipCompressionProvider>();
                })
                .AddMemoryCache()
                .AddRouting(options => options.LowercaseUrls = true)
                .AddSingleton<INamingConventions, PlatformNamingConventions>();

            var configuration = new MapperConfiguration(config =>
            {
                config.AddCollectionMappers();
                config.AddMaps("StarWars.Core");
            });

            services.AddSingleton(sp => configuration.CreateMapper());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger logger,
            IStringLocalizer<SharedResource> localizer)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            StaticData.Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

            Config.InitConfiguration(env);

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            StaticData.Logger = logger;

            app
                .UseRequestLocalization()
                .UseResponseCompression()
                .UseResponseCaching()
                // This UseStaticFiles line is specifically for the .well-known/acme-challenge files
                .UseStaticFiles(new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                })
                .UseExceptionHandler(a => a.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var ex = feature.Error;

                    if (StaticData.Logger != null)
                    {
                        StaticData.Logger.Error(ex, "Caught by global exception handler");
                    }

                    var code = HttpStatusCode.InternalServerError;

                    if (ex is FileNotFoundException)
                        code = HttpStatusCode.NotFound;
                    else if (ex is UnauthorizedAccessException)
                        code = HttpStatusCode.Unauthorized;
                    else if (ex is InvalidOperationException)
                        code = HttpStatusCode.BadRequest;

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)code;

                    var result = JsonConvert.SerializeObject(new { error = ex.Message });

                    await context.Response.WriteAsync(result).ConfigureAwait(false);
                }))
                .UseStatusCodePages(async context =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var message = StaticData.Localizer["Unknown"];

                    if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
                        message = StaticData.Localizer["Not found"];
                    else if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                        message = StaticData.Localizer["Unauthorised"];
                    else if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                        message = StaticData.Localizer["Your login does not have the required role permission"];

                    var result = JsonConvert.SerializeObject(new { error = message });

                    await context.HttpContext.Response.WriteAsync(result).ConfigureAwait(false);
                })
                .UseWebSockets()
                .UseRouting()
                .UseCors()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    // This is required for the health checks in kubernetes
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Alive").ConfigureAwait(false);
                    });

                    // This is required for the REST endpoints
                    endpoints.MapControllers();

                    // This is required for Hot Chocolate GraphQL
                    endpoints
                        .MapGraphQL()
                        .WithOptions(new GraphQLServerOptions
                        {
                            Tool = { Enable = true }
                        });
                });
        }
    }
}