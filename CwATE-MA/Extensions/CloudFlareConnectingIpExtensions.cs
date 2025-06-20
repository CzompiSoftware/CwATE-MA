using Microsoft.AspNetCore.HttpOverrides;

namespace CzSoft.CwateMa.Extensions;

public static class CloudFlareConnectingIpExtensions
{
    public static IApplicationBuilder UseCloudFlareConnectingIp(this IApplicationBuilder app)
    {
        return app.UseCloudFlareConnectingIp(false);
    }

    public static IApplicationBuilder UseCloudFlareConnectingIp(this IApplicationBuilder app, bool checkOriginatesFromCloudflare)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (checkOriginatesFromCloudflare)
        {
            app.UseMiddleware<CloudFlareConnectingIpMiddleware>();
        }
        else
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedForHeaderName = CloudFlareConnectingIpMiddleware.HeaderName,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor
            });
        }

        return app;
    }
}