namespace Asyntai.Umbraco.Chatbot.Models;

/// <summary>
/// Settings model for Asyntai chatbot configuration.
/// </summary>
public class AsyntaiSettings
{
    /// <summary>
    /// The site ID provided by Asyntai after connection.
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the chat widget script.
    /// </summary>
    public string ScriptUrl { get; set; } = "https://asyntai.com/static/js/chat-widget.js";

    /// <summary>
    /// The email associated with the Asyntai account.
    /// </summary>
    public string AccountEmail { get; set; } = string.Empty;
}
