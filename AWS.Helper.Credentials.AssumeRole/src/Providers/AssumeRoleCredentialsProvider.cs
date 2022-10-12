using Amazon.Runtime;
using Amazon.SecurityToken;
using AWS.Helper.Core;

namespace AWS.Helper.Credentials.AssumeRole;

public sealed class AssumeRoleCredentialsProvider : IAssumeRoleCredentialsProvider
{
    private const string DEFAULT_SESSION_NAME = "assume-role-temporary-session";
    private const int DEFAULT_SESSION_PERIOD = 900;

    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IStoredCredentialsProvider _storedCredentialsProvider;

    private GetStoredCredentialsInput? Request { get; set; }
    private AmazonSecurityTokenServiceClient? StsClient { get; set; }

    private bool IsRequestReady(string roleArn, string accessKey) =>
        Request != null
        && Request.CredentialsId == roleArn
        && Request.AccessKeyId == accessKey
        && Request.GetCredentials != null;

    private bool IsStsClientReady(string accessKey) =>
        Request != null
        && Request.AccessKeyId == accessKey
        && StsClient != null;

    private void Initialize(string roleArn, AWSCredentials userCredentials, int? sessionPeriodInSeconds = null)
    {
        var immutableUserCredentials = userCredentials.GetCredentials();

        if (!IsStsClientReady(immutableUserCredentials.AccessKey))
        {
            StsClient = new AmazonSecurityTokenServiceClient(userCredentials);
        }

        if (IsRequestReady(roleArn, immutableUserCredentials.AccessKey))
        {
            return;
        }

        Request = new GetStoredCredentialsInput(
            roleArn,
            immutableUserCredentials.AccessKey,
            sessionPeriodInSeconds ?? DEFAULT_SESSION_PERIOD,
            () =>
            {
                var assumeRoleTask = AssumeRoleService.AssumeRoleAsync(StsClient!, Request!.CredentialsId, DEFAULT_SESSION_NAME, 900);
                Task.WaitAll(assumeRoleTask);
                return assumeRoleTask.Result;
            }
        );
    }

    public AssumeRoleCredentialsProvider()
        : this(new StoredCredentialsProvider(new DateTimeProvider()), new DateTimeProvider()) { }

    public AssumeRoleCredentialsProvider(IStoredCredentialsProvider storedCredentialsProvider, IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _storedCredentialsProvider = storedCredentialsProvider;
    }

    public AWSCredentials GetCredentials(string roleArn, int? sessionPeriodInSeconds = null)
    {
        return GetCredentials(EnvironmentCredentialsProvider.GetCredentials(), roleArn);
    }

    public AWSCredentials GetCredentials(string accessKeyId, string secretKey, string roleArn, int? sessionPeriodInSeconds = null)
    {
        return GetCredentials(new BasicAWSCredentials(accessKeyId, secretKey), roleArn);
    }

    public AWSCredentials GetCredentials(AWSCredentials credentials, string roleArn, int? sessionPeriodInSeconds = null)
    {
        Initialize(roleArn, credentials);

        return _storedCredentialsProvider.GetStoredCredentials(Request!);
    }
}
