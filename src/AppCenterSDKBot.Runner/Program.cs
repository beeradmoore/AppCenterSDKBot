﻿using Amazon.Lambda.TestUtilities;
using AppCenterSDKBot;

Environment.SetEnvironmentVariable("AWS_PROFILE", "AppCenterSDKBot_Role");

// Invoke the lambda function and confirm the string was upper cased.
var function = new Function();
var context = new TestLambdaContext();
var upperCase = await function.FunctionHandler("hello world", context);
