using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.AspNetCore.HttpOverrides;
using CzSoft.CwateMa.Extensions;
using CzSoft.CwateMa.Model;
using CzSoft.CwateMa.Components;

const string csCdnCors = "_cscdncors";

CzomPack.Settings.Application = new CzomPack.Application(typeof(Program).Assembly);
CzomPack.Settings.WorkingDirectory = Globals.DataDirectory;

Globals.LoadConfigs();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
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
    .WriteTo.GrafanaLoki(Globals.Config.LokiUrl)
    .CreateLogger();

try
{
    Log.Information("Starting host...");
    Log.Information(Globals.ConfigFile);
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddCors(options => {
        options.AddPolicy(name: csCdnCors,
        policy =>
        {
            policy.WithOrigins(
                "https://cdn.czsoft.hu",
                "https://cdn-beta.czsoft.hu",
                "https://cdn.czsoft.dev",
                Globals.Config.CdnUrl
            );
        });
    });

    var app = builder.Build();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    // Prettify rendered HTML content
    app.Use(async (context, next) =>
    {
        var body = context.Response.Body;

        using var updatedBody = new MemoryStream();
        context.Response.Body = updatedBody;

        await next();

        context.Response.Body = body;

        updatedBody.Seek(0, SeekOrigin.Begin);
        var newContent = new StreamReader(updatedBody).ReadToEnd();

        await context.Response.WriteAsync(Globals.PrettifyHtml(newContent));
    });
    
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    // Configure the HTTP request pipeline.
    else
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseCloudFlareConnectingIp();
    app.UseAntiforgery();
    app.UseCors(csCdnCors);

    app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

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
