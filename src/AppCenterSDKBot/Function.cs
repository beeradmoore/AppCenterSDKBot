using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AppCenterSDKBot;

public class Function
{
    ILambdaLogger? logger;

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        logger = context?.Logger;

        var secrets = await GetSecrets();
        if (secrets == null)
        {
            logger?.LogLine("Could not load secrets.");
            return "fail";
        }

        var s3Client = new AmazonS3Client(RegionEndpoint.APSoutheast2);

        var iosTask = CheckXamariniOSSDK(secrets, s3Client);
        var androidTask = CheckAndroidSDK(secrets, s3Client);

        await Task.WhenAll(iosTask, androidTask);


        
        /*
   
        */
        return "fail";
    }

    async Task LogError(string message, [CallerMemberName] string memberName = "", bool logToSlack = true)
    {
        logger?.LogError($"({memberName}): {message}");
        // TODO: Slack message

        await Task.Delay(1);
    }

    async Task<bool> CheckXamariniOSSDK(Secrets secrets, AmazonS3Client s3Client)
    {
        ToolsetsResponse? oldToolsetResponse = null;
        ToolsetsResponse? newToolsetResponse = null;
        try
        {
            var oldObject = await s3Client.GetObjectAsync(secrets.S3Bucket, "xamarin_ios_sdk.json");

            oldToolsetResponse = JsonSerializer.Deserialize<ToolsetsResponse>(oldObject.ResponseStream);
            /*


            using (var memoryStream = new MemoryStream())
            {
                await oldObject.ResponseStream.CopyToAsync(memoryStream);
                var oldData = Encoding.UTF8.GetString(memoryStream.ToArray());
            }*/

        }
        catch (Exception err)
        {
            Debugger.Break();
        }

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-API-Token", secrets.APIToken_Xamairn_iOS);

            try
            {
                var responseMessage = await httpClient.GetAsync($"https://api.appcenter.ms/v0.1/apps/{secrets.Xamarin_iOS_Org}/{secrets.Xamarin_iOS_App}/toolsets?tools=xamarin");
                if (responseMessage.IsSuccessStatusCode == false)
                {
                    await LogError($"Could not fetch new content. Got status code {responseMessage.StatusCode}.");
                    return false;
                }

                var body = await responseMessage.Content.ReadAsStringAsync();

                newToolsetResponse = JsonSerializer.Deserialize<ToolsetsResponse>(body);
                if (newToolsetResponse == null)
                {
                    await LogError("Could not deserialize newToolsetResponse.");
                    await LogError(body, logToSlack: false);
                    return false;
                }

                // TODO: Only upload if it was changed.

                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    memoryStream.Position = 0;
                    var putObjectRequest = new PutObjectRequest()
                    {
                        BucketName = secrets.S3Bucket,
                        Key = "xamarin_ios_sdk.json",
                        InputStream = memoryStream,
                        AutoCloseStream = false,
                    };
                    var putObjectResponse = await s3Client.PutObjectAsync(putObjectRequest);
                    if (putObjectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    {
                        await LogError($"putObjectResponse did not have an a valid HttpStatusCode, {putObjectResponse.HttpStatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception err)
            {
                await LogError(err.Message);
                return false;
            }
        }

        if (oldToolsetResponse == null)
        {
            await LogError("Did not attempt to compare as oldToolsetResponse was null.");
            return false;
        }
        else if (newToolsetResponse == null)
        {
            await LogError("Did not attempt to compare as newToolsetResponse was null.");
            return false;
        }

        // TODO: Do compare.

        return true;
    }

    async Task<bool> CheckAndroidSDK(Secrets secrets, AmazonS3Client s3Client)
    {
        await Task.Delay(1);
        return true;
    }

    async Task<Secrets?> GetSecrets()
    {
        var secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.APSoutheast2);
        var getSecretValueRequest = new GetSecretValueRequest()
        {
            SecretId = "AppCenterSDKBot",
        };

        try
        {
            var getSecretValueResponse = await secretsManagerClient.GetSecretValueAsync(getSecretValueRequest);
            var secrets = JsonSerializer.Deserialize<Secrets>(getSecretValueResponse.SecretString);
            return secrets;
        }
        catch (Exception err)
        {
            logger?.LogError($"ERROR: {err.Message}");
        }

        return null;
    }
}
