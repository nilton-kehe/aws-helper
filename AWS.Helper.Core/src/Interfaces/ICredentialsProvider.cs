using Amazon.Runtime;

namespace AWS.Helper.Core;

public interface ICredentialsProvider
{
    static AWSCredentials GetCredentials() => throw new NotImplementedException();
}