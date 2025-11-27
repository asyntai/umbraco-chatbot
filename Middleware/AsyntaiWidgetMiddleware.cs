using System.Text;
using System.Text.Json;
using Asyntai.Umbraco.Chatbot.Services;
using Microsoft.AspNetCore.Http;

namespace Asyntai.Umbraco.Chatbot.Middleware;

/// <summary>
/// Middleware that automatically injects the Asyntai chat widget script into HTML responses.
/// </summary>
public class AsyntaiWidgetMiddleware
{
    private readonly RequestDelegate _next;

    public AsyntaiWidgetMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AsyntaiSettingsService settingsService)
    {
        // Skip backoffice requests
        if (context.Request.Path.StartsWithSegments("/umbraco"))
        {
            await _next(context);
            return;
        }

        // Check if we have a site ID configured
        var settings = settingsService.GetSettings();
        if (string.IsNullOrWhiteSpace(settings.SiteId))
        {
            await _next(context);
            return;
        }

        // Capture the original response body
        var originalBodyStream = context.Response.Body;

        using var newBodyStream = new MemoryStream();
        context.Response.Body = newBodyStream;

        await _next(context);

        // Check if it's an HTML response
        var contentType = context.Response.ContentType?.ToLowerInvariant() ?? "";
        if (!contentType.Contains("text/html"))
        {
            newBodyStream.Seek(0, SeekOrigin.Begin);
            await newBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
            return;
        }

        // Read the response
        newBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();

        // Build the injection script
        var siteIdJson = JsonSerializer.Serialize(settings.SiteId);
        var scriptUrlJson = JsonSerializer.Serialize(settings.ScriptUrl);

        var injection = $@"<script type=""text/javascript"">(function(){{var s=document.createElement(""script"");s.async=true;s.defer=true;s.src={scriptUrlJson};s.setAttribute(""data-asyntai-id"",{siteIdJson});s.charset=""UTF-8"";var f=document.getElementsByTagName(""script"")[0];if(f&&f.parentNode){{f.parentNode.insertBefore(s,f);}}else{{(document.head||document.documentElement).appendChild(s);}}}})()</script>";

        // Inject before </body> or append at the end
        if (responseBody.Contains("</body>", StringComparison.OrdinalIgnoreCase))
        {
            responseBody = responseBody.Replace("</body>", injection + "</body>", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            responseBody += injection;
        }

        // Write the modified response
        var modifiedBytes = Encoding.UTF8.GetBytes(responseBody);
        context.Response.ContentLength = modifiedBytes.Length;
        context.Response.Body = originalBodyStream;
        await context.Response.Body.WriteAsync(modifiedBytes);
    }
}
