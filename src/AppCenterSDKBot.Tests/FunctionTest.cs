using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

namespace AppCenterSDKBot.Tests;

public class FunctionTest
{
    [Fact]
    public async void TestToUpperFunction()
    {
        Environment.SetEnvironmentVariable("AWS_PROFILE", "AppCenterSDKBot_Role");

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function();
        var context = new TestLambdaContext();
        var upperCase = await function.FunctionHandler("hello world", context);

        Assert.Equal("HELLO WORLD", upperCase);
    }
}
