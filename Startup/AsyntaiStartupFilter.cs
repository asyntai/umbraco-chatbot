using Asyntai.Umbraco.Chatbot.Middleware;
using Microsoft.AspNetCore.Builder;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Asyntai.Umbraco.Chatbot.Startup;

/// <summary>
/// Pipeline filter that adds the Asyntai widget middleware to the Umbraco pipeline.
/// </summary>
public class AsyntaiPipelineFilter : IUmbracoPipelineFilter
{
    public string Name => "Asyntai";

    public void OnEndpoints(IApplicationBuilder app)
    {
        // Not needed
    }

    public void OnPostPipeline(IApplicationBuilder app)
    {
        // Add our middleware at the end of the pipeline
        app.UseMiddleware<AsyntaiWidgetMiddleware>();
    }

    public void OnPrePipeline(IApplicationBuilder app)
    {
        // Not needed
    }
}
