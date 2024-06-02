namespace Kvarovi.Entities;

public class User
{
    public int UserId;
    public string DeviceToken { get; set; } = null!;

    public List<Keyword> Keywords { get; set; } = new List<Keyword>();
    public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
}
