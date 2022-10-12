
using Amazon.Runtime;

namespace AWS.Helper.Core;

public sealed class GetStoredCredentialsInput
{
    public GetStoredCredentialsInput(string credentialsId,
                                     string accessKeyId,
                                     int timeoutInSeconds,
                                     Func<AWSCredentials> getCredentials)
    {
        CredentialsId = credentialsId;
        AccessKeyId = accessKeyId;
        TimeoutInSeconds = timeoutInSeconds;
        GetCredentials = getCredentials;
    }

    public string CredentialsId { get; private set; }
    public string AccessKeyId { get; private set; }
    public int TimeoutInSeconds { get; private set; }
    public Func<AWSCredentials>? GetCredentials { get; private set; }
}