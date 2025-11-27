using Asyntai.Umbraco.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;

namespace Asyntai.Umbraco.Chatbot.ViewComponents;

/// <summary>
/// View component for rendering the Asyntai chat widget.
/// Use in templates with: @await Component.InvokeAsync("AsyntaiWidget")
/// </summary>
public class AsyntaiWidgetViewComponent : ViewComponent
{
    private readonly AsyntaiSettingsService _settingsService;

    public AsyntaiWidgetViewComponent(AsyntaiSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public IViewComponentResult Invoke()
    {
        var settings = _settingsService.GetSettings();
        return View("~/App_Plugins/Asyntai/Views/Widget.cshtml", settings);
    }
}
