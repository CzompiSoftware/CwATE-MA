using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using NetTools;
using CzomPack.Network;
using CzomPack.Logging;

namespace CzSoft.CwateMa.Extensions;

internal class CloudFlareConnectingIpMiddleware
{
    public const string HeaderName = "CF_CONNECTING_IP";

    private readonly List<IPAddressRange> _cloudFlareIpAddressRanges = new();

    private readonly RequestDelegate _next;
    private readonly ForwardedHeadersMiddleware _forwardedHeadersMiddleware;

    public CloudFlareConnectingIpMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        try
        {
            var ipv4 = NetHandler.SendRequest("https://www.cloudflare.com/ips-v4", RequestMethod.GET);
            string[] ipv4Addresses = (int)ipv4.StatusCode / 100 > 3 ? ipv4.Result.Split("\n").Select(x => x.Trim('\r')).ToArray() : Array.Empty<string>();
            _cloudFlareIpAddressRanges.AddRange(ipv4Addresses.Select(x => IPAddressRange.Parse(x)));
        }
        catch (Exception ex)
        {
            Logger.Error<CloudFlareConnectingIpMiddleware>($"{ex}");
        }
        try
        {
            var ipv6 = NetHandler.SendRequest("https://www.cloudflare.com/ips-v6", RequestMethod.GET);
            string[] ipv6Addresses = (int)ipv6.StatusCode / 100 > 3 ? ipv6.Result.Split("\n").Select(x => x.Trim('\r')).ToArray() : Array.Empty<string>();
            _cloudFlareIpAddressRanges.AddRange(ipv6Addresses.Select(x => IPAddressRange.Parse(x)));
        }
        catch (Exception ex)
        {
            Logger.Error<CloudFlareConnectingIpMiddleware>("{exception}", ex);
        }
        _next = next ?? throw new ArgumentNullException(nameof(next));

        _forwardedHeadersMiddleware = new ForwardedHeadersMiddleware(next, loggerFactory, Options.Create(new ForwardedHeadersOptions
        {
            ForwardedForHeaderName = HeaderName,
            ForwardedHeaders = ForwardedHeaders.XForwardedFor
        }));
    }

    public Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey(HeaderName) && IsCloudFlareIp(context.Connection.RemoteIpAddress))
        {
            return _forwardedHeadersMiddleware.Invoke(context);
        }

        return _next(context);
    }

    private bool IsCloudFlareIp(IPAddress ipadress)
    {
        bool isCloudFlareIp = false;

        for (int i = 0; i < _cloudFlareIpAddressRanges.Count; i++)
        {
            isCloudFlareIp = _cloudFlareIpAddressRanges[i].Contains(ipadress);
            if (isCloudFlareIp)
            {
                break;
            }
        }

        return isCloudFlareIp;
    }
}