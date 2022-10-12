using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AWS.Helper.Credentials.AssumeRole;

internal sealed class AssumeRoleService
{
    public static async Task<AWSCredentials> AssumeRoleAsync(AmazonSecurityTokenServiceClient stsClient,
                                                             string roleArn,
                                                             string sessionName,
                                                             int sessionPeriodInSeconds)
    {
        var request = new AssumeRoleRequest
        {
            DurationSeconds = sessionPeriodInSeconds,
            RoleArn = roleArn,
            RoleSessionName = sessionName
        };

        var response = await stsClient.AssumeRoleAsync(request);

        return response.Credentials;
    }
}