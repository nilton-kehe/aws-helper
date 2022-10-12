using Amazon.Runtime;
using AWS.Helper.Core;

namespace AWS.Helper.Credentials.AssumeRole;

internal sealed class CredentialStoreService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly object _lock = new object();
    private List<StoredCredentials> _storedCredentials = new List<StoredCredentials>();

    public CredentialStoreService(
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    private bool IsCredentialsExpired(StoredCredentials credentials) =>
        credentials is null || credentials.IsExpired(_dateTimeProvider.UtcNow);

    private Func<StoredCredentials, GetStoredCredentialsInput, bool> MatchInput = (cred, input) =>
            cred.RoleArn == input.RoleArn
            && cred.SessionName == input.SessionName
            && cred.AccessKeyId == input.AccessKeyId;

    private StoredCredentials GetCredentialsFromStore(GetStoredCredentialsInput input) =>
        _storedCredentials.FirstOrDefault(cred => MatchInput(cred, input))!;

    private void StoreCredentials(GetStoredCredentialsInput input, AWSCredentials credentials)
    {
        _storedCredentials.RemoveAll(cred => MatchInput(cred, input));

        _storedCredentials.Add(
            new StoredCredentials(input.RoleArn,
                                  input.AccessKeyId,
                                  input.SessionName,
                                  input.SessionPeriodInSeconds,
                                  _dateTimeProvider.UtcNow,
                                  credentials));
    }

    public AWSCredentials GetStoredCredentials(GetStoredCredentialsInput input)
    {
        AWSCredentials? credentials = null;
        var storedCredentials = GetCredentialsFromStore(input);

        if (!IsCredentialsExpired(storedCredentials))
        {
            credentials = storedCredentials.Credentials;
        }
        else
        {
            lock (_lock)
            {
                if (IsCredentialsExpired(storedCredentials))
                {
                    var newCredentials = input.GetCredentials();
                    credentials = newCredentials;
                    StoreCredentials(input, newCredentials);
                }
            }
        }

        return credentials!;
    }
}

