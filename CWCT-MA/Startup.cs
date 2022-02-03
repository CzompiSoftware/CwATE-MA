using CWCTMA.Model;
using Markdig;
using Markdig.Xmd;
using Markdig.Prism;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CWCTMA.Extensions;

namespace CWCTMA
{
    public class Startup
    {
        private readonly string CzompiSoftwareCDNCors = "_czompiSoftwareCDNCors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy(name: CzompiSoftwareCDNCors,
                builder =>
                {
                    builder.WithOrigins("https://cdn.czompisoftware.hu",
                                        "https://cdn-beta.czompisoftware.hu");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
            app.UseCors(CzompiSoftwareCDNCors);

            Globals.MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXMDLanguage().Build();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
