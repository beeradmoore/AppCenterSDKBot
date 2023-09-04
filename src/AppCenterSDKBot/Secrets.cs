using System;
using System.Text.Json.Serialization;

namespace AppCenterSDKBot;

public class Secrets
{
    [JsonPropertyName("api_token_xamarin_ios")]
    public string APIToken_Xamairn_iOS { get; set; } = String.Empty;

    [JsonPropertyName("api_token_xamarin_android")]
    public string APIToken_Xamairn_Android { get; set; } = String.Empty;

    [JsonPropertyName("xamarin_ios_app")]
    public string Xamarin_iOS_App { get; set; } = String.Empty;

    [JsonPropertyName("xamarin_android_app")]
    public string Xamarin_Android_App { get; set; } = String.Empty;

    [JsonPropertyName("xamarin_ios_org")]
    public string Xamarin_iOS_Org { get; set; } = String.Empty;

    [JsonPropertyName("xamarin_android_org")]
    public string Xamarin_Android_Org { get; set; } = String.Empty;

    [JsonPropertyName("s3_bucket")]
    public string S3Bucket { get; set; } = "appcentersdkbot";



}