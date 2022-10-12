using Amazon.Runtime;

namespace AWS.Helper.Core;

public sealed class StoredCredentials
{
    public StoredCredentials(string credentialsId,
                             string accessKeyId,
                             int timeoutInSeconds,
                             DateTimeOffset createdAt,
                             AWSCredentials credentials)
    {
        CredentialsId = credentialsId;
        AccessKeyId = accessKeyId;
        TimeoutInSeconds = timeoutInSeconds;
        CreatedAt = createdAt;
        Credentials = credentials;
    }

    public string CredentialsId { get; private set; }
    public string AccessKeyId { get; set; }
    public int TimeoutInSeconds { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public AWSCredentials Credentials { get; set; }

    public bool IsExpired(DateTimeOffset now) =>
        (now - CreatedAt).TotalSeconds >= TimeoutInSeconds - 30;
}