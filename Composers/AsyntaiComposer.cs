using Asyntai.Umbraco.Chatbot.Services;
using Asyntai.Umbraco.Chatbot.Startup;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Asyntai.Umbraco.Chatbot.Composers;

/// <summary>
/// Umbraco composer for registering Asyntai services.
/// This is automatically discovered and executed by Umbraco.
/// </summary>
public class AsyntaiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Register the settings service as singleton
        builder.Services.AddSingleton<AsyntaiSettingsService>();

        // Register the pipeline filter to auto-inject middleware
        builder.Services.Configure<UmbracoPipelineOptions>(options =>
        {
            options.AddFilter(new AsyntaiPipelineFilter());
        });
    }
}
