using System;
using System.Text.Json.Serialization;

namespace AppCenterSDKBot;

public class ToolsetsResponse
{
    [JsonPropertyName("xamarin")]
    public List<XamarinSDKItem> Xamarin { get; set; } = new List<XamarinSDKItem>();
}

