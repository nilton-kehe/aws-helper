using Amazon.Runtime;

namespace AWS.Helper.Credentials.AssumeRole;

public sealed class StoredCredentials
{
    public StoredCredentials(string roleArn,
                             string accessKeyId,
                             string sessionName,
                             int sessionPeriodInSeconds,
                             DateTimeOffset createdAt,
                             AWSCredentials credentials)
    {
        RoleArn = roleArn;
        AccessKeyId = accessKeyId;
        SessionName = sessionName;
        SessionPeriodInSeconds = sessionPeriodInSeconds;
        CreatedAt = createdAt;
        Credentials = credentials;
    }

    public string RoleArn { get; private set; }
    public string AccessKeyId { get; set; }
    public string SessionName { get; private set; }
    public int SessionPeriodInSeconds { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public AWSCredentials Credentials { get; set; }

    public bool IsExpired(DateTimeOffset now) =>
        (now - CreatedAt).TotalSeconds >= SessionPeriodInSeconds - 30;
}