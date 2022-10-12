using Amazon.Runtime;

namespace AWS.Helper.Core;

public interface IStoredCredentialsProvider
{
    AWSCredentials GetStoredCredentials(GetStoredCredentialsInput input);
}