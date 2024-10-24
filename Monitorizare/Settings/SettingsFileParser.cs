using System.Diagnostics;
using System.Text.Json;

using Monitorizare.Models;
using Monitorizare.Services;

namespace Monitorizare.Settings;

public class SettingsFileParser
{
    private string _settingsFile;

    private CustomSettings _settingsData;

    private readonly Logger _logger;

    public SettingsFileParser(string settingsFile = "Settings.json")
    {
        _logger = Logger.Instance;
        _settingsFile = settingsFile;
        _settingsData = ParseJsonDocument();
    }

    public CustomSettings GetSettings() => _settingsData;

    private CustomSettings ParseJsonDocument()
    {
        Dictionary<string, string?> schema = new Dictionary<string, string?>();
        string? file = string.Empty;
        try
        {
            string content = File.Exists(_settingsFile)
                ? File.ReadAllText(_settingsFile)
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(content))
            {
                JsonDocument document = JsonDocument.Parse(content);
                JsonElement root = document.RootElement;

                if (root.TryGetProperty("Database", out JsonElement databaseElement))
                {
                    if (databaseElement.TryGetProperty("Schema", out JsonElement schemaElement))
                    {
                        schema = schemaElement.EnumerateObject().ToDictionary(ele => ele.Name, ele => ele.Value.GetString());
                    }

                    if (databaseElement.TryGetProperty("File", out JsonElement fileElement))
                    {
                        file = fileElement.GetString();
                    }
                }
            }
        }
        catch (Exception ex) when (Filter(ex))
        {
            #if DEBUG
            Debug.WriteLine($"Settings file not found or got an error while parsing for settings: {ex.Message}");
            #endif
        }

        return new CustomSettings
        {
            Schema = schema,
            File = file
        };
    }

    private bool Filter(Exception ex) => (ex is FileNotFoundException) || (ex is JsonException);
}