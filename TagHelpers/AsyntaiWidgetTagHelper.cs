using System.Text.Encodings.Web;
using Asyntai.Umbraco.Chatbot.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asyntai.Umbraco.Chatbot.TagHelpers;

/// <summary>
/// Tag helper for manually adding the Asyntai widget.
/// Usage: <asyntai-widget />
/// </summary>
[HtmlTargetElement("asyntai-widget")]
public class AsyntaiWidgetTagHelper : TagHelper
{
    private readonly AsyntaiSettingsService _settingsService;

    public AsyntaiWidgetTagHelper(AsyntaiSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var settings = _settingsService.GetSettings();

        if (string.IsNullOrWhiteSpace(settings.SiteId))
        {
            output.SuppressOutput();
            return;
        }

        var siteId = HtmlEncoder.Default.Encode(settings.SiteId);
        var scriptUrl = HtmlEncoder.Default.Encode(settings.ScriptUrl);

        output.TagName = "script";
        output.Attributes.SetAttribute("async", null);
        output.Attributes.SetAttribute("defer", null);
        output.Attributes.SetAttribute("src", scriptUrl);
        output.Attributes.SetAttribute("data-asyntai-id", siteId);
        output.TagMode = TagMode.StartTagAndEndTag;
    }
}
