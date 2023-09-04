using System;
using System.Text.Json.Serialization;

namespace AppCenterSDKBot;

public class XamarinSDKItem
{
    [JsonPropertyName("monoVersion")]
    public string MonoVersion { get; set; } = String.Empty;

    [JsonPropertyName("sdkBundle")]
    public string SDKBundle { get; set; } = String.Empty;

    [JsonPropertyName("symlink")]
    public string SymLink { get; set; } = String.Empty;

    [JsonPropertyName("current")]
    public bool Current { get; set; } = false;

    [JsonPropertyName("stable")]
    public bool Stable { get; set; } = false;

    // Only relevant for iOS Xamarin SDK items
    [JsonPropertyName("xcodeVersions")]
    public List<string> XcodeVersions { get; set; } = new List<string>();

}

