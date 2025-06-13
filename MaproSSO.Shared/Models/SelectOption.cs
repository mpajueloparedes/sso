using System.Text.Json.Serialization;

namespace MaproSSO.Shared.Models;

public class SelectOption
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; } = false;

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, object>? Data { get; set; }

    public SelectOption() { }

    public SelectOption(string value, string text, bool disabled = false, string? group = null)
    {
        Value = value;
        Text = text;
        Disabled = disabled;
        Group = group;
    }
}

public class SelectOptionGroup
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public List<SelectOption> Options { get; set; } = new();

    public SelectOptionGroup() { }

    public SelectOptionGroup(string label, List<SelectOption> options)
    {
        Label = label;
        Options = options;
    }
}