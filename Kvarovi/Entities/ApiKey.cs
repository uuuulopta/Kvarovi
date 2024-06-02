namespace Kvarovi.Entities;

public class ApiKey
{
    public int ApiKeyId;

    public ApiKey(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public User User { get; } = null!;

}
