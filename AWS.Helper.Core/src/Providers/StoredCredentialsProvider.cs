using Amazon.Runtime;

namespace AWS.Helper.Core;

public sealed class StoredCredentialsProvider : IStoredCredentialsProvider
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly object _lock = new object();
    private List<StoredCredentials> _storedCredentials = new List<StoredCredentials>();

    public StoredCredentialsProvider(
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    private bool IsCredentialsExpired(StoredCredentials credentials) =>
        credentials is null || credentials.IsExpired(_dateTimeProvider.UtcNow);

    private Func<StoredCredentials, GetStoredCredentialsInput, bool> CredentialsFilter = (cred, input) =>
            cred.CredentialsId == input.CredentialsId && cred.AccessKeyId == input.AccessKeyId;

    private StoredCredentials GetCredentialsFromStore(GetStoredCredentialsInput input) =>
        _storedCredentials.FirstOrDefault(cred => CredentialsFilter(cred, input))!;

    private void StoreCredentials(GetStoredCredentialsInput input, AWSCredentials credentials)
    {
        _storedCredentials.RemoveAll(cred => CredentialsFilter(cred, input));

        _storedCredentials.Add(
            new StoredCredentials(input.CredentialsId,
                                  input.AccessKeyId,
                                  input.TimeoutInSeconds,
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

