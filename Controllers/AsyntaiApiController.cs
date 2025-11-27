using Asyntai.Umbraco.Chatbot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Authorization;

namespace Asyntai.Umbraco.Chatbot.Controllers;

/// <summary>
/// API controller for Asyntai settings management.
/// </summary>
[ApiController]
[Route("umbraco/api/asyntai")]
public class AsyntaiApiController : Controller
{
    private readonly AsyntaiSettingsService _settingsService;

    public AsyntaiApiController(AsyntaiSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    [HttpGet("settings")]
    [AllowAnonymous]
    public IActionResult GetSettings()
    {
        var settings = _settingsService.GetSettings();
        return Ok(new
        {
            success = true,
            siteId = settings.SiteId,
            scriptUrl = settings.ScriptUrl,
            accountEmail = settings.AccountEmail,
            connected = !string.IsNullOrWhiteSpace(settings.SiteId)
        });
    }

    /// <summary>
    /// Saves the connection settings.
    /// </summary>
    [HttpPost("save")]
    [AllowAnonymous]
    public IActionResult Save([FromBody] SaveSettingsRequest? request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SiteId))
        {
            return BadRequest(new { success = false, error = "missing site_id" });
        }

        try
        {
            _settingsService.UpdateSettings(
                request.SiteId.Trim(),
                request.ScriptUrl?.Trim(),
                request.AccountEmail?.Trim()
            );

            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Resets all settings.
    /// </summary>
    [HttpPost("reset")]
    [AllowAnonymous]
    public IActionResult Reset()
    {
        try
        {
            _settingsService.ResetSettings();
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}

/// <summary>
/// Request model for saving settings.
/// </summary>
public class SaveSettingsRequest
{
    public string? SiteId { get; set; }
    public string? ScriptUrl { get; set; }
    public string? AccountEmail { get; set; }
}
