using Amazon.Runtime;

namespace AWS.Helper.Core;

public interface IAssumeRoleCredentialsProvider
{
    public AWSCredentials GetCredentials(string roleArn);
    public AWSCredentials GetCredentials(string accessKeyId, string secretKey, string roleArn);
    public AWSCredentials GetCredentials(AWSCredentials credentials, string roleArn);
}