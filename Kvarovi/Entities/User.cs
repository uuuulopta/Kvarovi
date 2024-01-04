namespace Kvarovi.Entities;

public class User
{
    public int UserId;
    public string DeviceToken { get; set; } = null!;
    public IEnumerable<Keyword> Keywords { get; set; } = null!;
}
