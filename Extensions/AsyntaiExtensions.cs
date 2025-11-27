using Asyntai.Umbraco.Chatbot.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Asyntai.Umbraco.Chatbot.Extensions;

/// <summary>
/// Extension methods for configuring Asyntai chatbot.
/// </summary>
public static class AsyntaiExtensions
{
    /// <summary>
    /// Adds the Asyntai widget middleware to automatically inject the chatbot script.
    /// Add this in your Configure method: app.UseAsyntaiWidget();
    /// </summary>
    public static IApplicationBuilder UseAsyntaiWidget(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AsyntaiWidgetMiddleware>();
    }
}
