using CzSoft.CwateMa.Model;

namespace CzSoft.CwateMa.Middlewares;

public class HtmlFormatterMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Response.ContentType != null && !context.Response.ContentType.ToLower().Contains("text/html"))
        {
            await next(context);
            return;
        }
        var body = context.Response.Body;

        using var updatedBody = new MemoryStream();
        context.Response.Body = updatedBody;
        //context.Response.ContentLength = updatedBody.Length;

        await next(context);

        
        context.Response.Body = body;

        updatedBody.Seek(0, SeekOrigin.Begin);
        var newContent = await new StreamReader(updatedBody).ReadToEndAsync();
        var responseStr = Globals.PrettifyHtml(newContent);
        
        await context.Response.WriteAsync(responseStr);
        //context.Response.ContentLength = responseStr.Length;
    }
}
