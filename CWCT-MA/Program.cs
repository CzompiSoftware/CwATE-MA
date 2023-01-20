using CwctMa.Model;
using Markdig;
using Markdig.Xmd;
using Markdig.Prism;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CwctMa.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Reflection;
using System;

string CzSoftCdnCors = "_cscdnmacors";

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
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCors(options => {
        options.AddPolicy(name: CzSoftCdnCors,
        builder =>
        {
            builder.WithOrigins(
                "https://cdn.czsoft.hu",
                "https://cdn-beta.czsoft.hu",
                "https://cdn.czompisoftware.hu",
                "https://cdn-beta.czompisoftware.hu"
            );
        });
    });

    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCloudFlareConnectingIp();

    app.UseRouting();
    app.UseCors(CzSoftCdnCors);

    Globals.MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXMDLanguage().Build();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapBlazorHub();
        endpoints.MapFallbackToPage("/_Host");
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}