using System.Text.Json;
using Asyntai.Umbraco.Chatbot.Models;
using Microsoft.Extensions.Hosting;

namespace Asyntai.Umbraco.Chatbot.Services;

/// <summary>
/// Service for managing Asyntai settings persistence.
/// </summary>
public class AsyntaiSettingsService
{
    private readonly string _settingsFilePath;
    private readonly object _lock = new();
    private AsyntaiSettings? _cachedSettings;

    public AsyntaiSettingsService(IHostEnvironment hostEnvironment)
    {
        var appDataPath = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", "Asyntai");
        Directory.CreateDirectory(appDataPath);
        _settingsFilePath = Path.Combine(appDataPath, "settings.json");
    }

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    public AsyntaiSettings GetSettings()
    {
        lock (_lock)
        {
            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }

            if (!File.Exists(_settingsFilePath))
            {
                _cachedSettings = new AsyntaiSettings();
                return _cachedSettings;
            }

            try
            {
                var json = File.ReadAllText(_settingsFilePath);
                _cachedSettings = JsonSerializer.Deserialize<AsyntaiSettings>(json) ?? new AsyntaiSettings();
            }
            catch
            {
                _cachedSettings = new AsyntaiSettings();
            }

            return _cachedSettings;
        }
    }

    /// <summary>
    /// Saves the settings.
    /// </summary>
    public void SaveSettings(AsyntaiSettings settings)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_settingsFilePath, json);
            _cachedSettings = settings;
        }
    }

    /// <summary>
    /// Updates specific settings fields.
    /// </summary>
    public void UpdateSettings(string siteId, string? scriptUrl = null, string? accountEmail = null)
    {
        var settings = GetSettings();
        settings.SiteId = siteId;

        if (!string.IsNullOrWhiteSpace(scriptUrl))
        {
            settings.ScriptUrl = scriptUrl;
        }

        if (!string.IsNullOrWhiteSpace(accountEmail))
        {
            settings.AccountEmail = accountEmail;
        }

        SaveSettings(settings);
    }

    /// <summary>
    /// Resets all settings to defaults.
    /// </summary>
    public void ResetSettings()
    {
        SaveSettings(new AsyntaiSettings());
    }

    /// <summary>
    /// Checks if the chatbot is connected (has a site ID).
    /// </summary>
    public bool IsConnected()
    {
        var settings = GetSettings();
        return !string.IsNullOrWhiteSpace(settings.SiteId);
    }
}
