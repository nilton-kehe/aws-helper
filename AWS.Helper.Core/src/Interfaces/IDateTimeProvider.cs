namespace AWS.Helper.Core;

public interface IDateTimeProvider
{
    public DateTimeOffset Now { get; }
    public DateTimeOffset UtcNow { get; }
}