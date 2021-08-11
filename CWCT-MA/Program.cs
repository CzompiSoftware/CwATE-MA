using CWCTMA.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace CWCTMA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists(Globals.ConfigFile))
            {
                File.WriteAllText(Globals.ConfigFile, JsonSerializer.Serialize(new Config
                {
                    Id = "CWCTMADE".ToLower(),
                    ShortName = "CWCT/MA DE",
                    FullName = "Czompi WebAPP Common Template for Microsoft ASP.NET - Development Environment",
                    SiteURL = "./",
                    Meta = new()
                    {
                        Title = "CWCT/MA DE",
                        Description = "Czompi WebAPP Common Template for Microsoft ASP.NET - Development Environment",
                        Image = null,
                        PrimaryColor = "#EAEAEA"
                    },
                    Pages = new List<Page>()
                    {
                        new() { Id = "", Name = "Main page", Alias = new List<string> { "main" } },
                        new() { Id = "page2", Name = "Page #2" }
                    }
                }, Globals.JsonSerializerOptions));
            }
            if (!File.Exists(Globals.GroupFile))
            {
                File.WriteAllText(Globals.GroupFile, JsonSerializer.Serialize(new GroupConfig
                {
                    Current = "czd",
                    Groups = new()
                    {
                        new() { Id = "czd", Name = "Czompi", Url = "https://czompi.hu" },
                        new() { Id = "czs", Name = "Czompi Software", Url = "https://czompisoftware.hu" },
                        new() { Id = "hls", Name = "HunLuxSCHOOL", Url = "https://hunluxschool.hu" },
                        new() { Id = "hll", Name = "HunLux Launcher", Url = "https://hunluxlauncher.hu" }
                    }
                }, Globals.JsonSerializerOptions));
            }
            Globals.Config = JsonSerializer.Deserialize<Config>(File.ReadAllText(Globals.ConfigFile), Globals.JsonSerializerOptions);
            Globals.Group = JsonSerializer.Deserialize<GroupConfig>(File.ReadAllText(Globals.GroupFile), Globals.JsonSerializerOptions);
            Globals.AppMeta = new()
            {
                Name = "CWCT/MA",
                FullName = "Czompi WebAPP Common Template for Microsoft ASP.NET",
                Version = Assembly.GetExecutingAssembly().GetName().Version,
                CompileTime = CWCTMA.Builtin.CompileTime,
                BuildId = CWCTMA.Builtin.BuildId
                //CompileTimeString = Assembly.GetExecutingAssembly().GetBuildDateTime().ToSqlTimeString()
            };
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                //.MinimumLevel.Override("System", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    @$"{Assembly.GetExecutingAssembly().GetName().Name}.{DateTime.Now:yyyy-MM-dd}.log",
                    outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}",
                    fileSizeLimitBytes: 1_000_000,
#if RELEASE
                    restrictedToMinimumLevel: LogEventLevel.Information,
#else
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
#endif
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Literate,
#if RELEASE
                    restrictedToMinimumLevel: LogEventLevel.Information
#else
                    restrictedToMinimumLevel: LogEventLevel.Verbose
#endif
                )
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                Log.Information(Globals.ConfigFile);
                CreateHostBuilder(args).Build().Run();
                //return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                //return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
