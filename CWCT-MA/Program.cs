using CWCTMA.Helpers;
using CWCTMA.Model;
using CWCTMA.Model.XMD;
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
            CzomPack.Settings.Application = new CzomPack.Application(typeof(Program).Assembly);
            CzomPack.Settings.WorkingDirectory = Globals.DataDirectory;

            Globals.LoadConfigs();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    Path.Combine(Globals.LogsDirectory, @$"{Assembly.GetExecutingAssembly().GetName().Name}.{DateTime.Now:yyyy-MM-dd}.log"),
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
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
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
