using Amazon.Runtime;

namespace AWS.Helper.Core;

public interface IAssumeRoleCredentialsProvider
{
    public AWSCredentials GetCredentials(string roleArn, int? sessionPeriodInSeconds = null);
    public AWSCredentials GetCredentials(string accessKeyId, string secretKey, string roleArn, int? sessionPeriodInSeconds = null);
    public AWSCredentials GetCredentials(AWSCredentials credentials, string roleArn, int? sessionPeriodInSeconds = null);
}