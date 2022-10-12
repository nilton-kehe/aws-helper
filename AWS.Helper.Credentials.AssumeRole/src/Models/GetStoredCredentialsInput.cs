
using Amazon.Runtime;

namespace AWS.Helper.Credentials.AssumeRole;

public sealed class GetStoredCredentialsInput
{
    public GetStoredCredentialsInput(string roleArn,
                                     string accessKeyId,
                                     string sessionName,
                                     int sessionPeriodInSeconds,
                                     Func<AWSCredentials> getCredentials)
    {
        RoleArn = roleArn;
        AccessKeyId = accessKeyId;
        SessionName = sessionName;
        SessionPeriodInSeconds = sessionPeriodInSeconds;
        GetCredentials = getCredentials;
    }

    public string RoleArn { get; private set; }
    public string AccessKeyId { get; set; }
    public string SessionName { get; private set; }
    public int SessionPeriodInSeconds { get; private set; }
    public Func<AWSCredentials> GetCredentials { get; private set; }
}