using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Builder;

namespace Cwatema.Extensions;

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
                ForwardedForHeaderName = CloudFlareConnectingIpMiddleware.CLOUDFLARE_CONNECTING_IP_HEADER_NAME,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor
            });
        }

        return app;
    }
}