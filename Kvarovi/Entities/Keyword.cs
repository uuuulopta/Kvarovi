namespace Kvarovi.Entities;

public class Keyword
{
    
    public int KeywordId; 
    public string Word { get; set; } = null!;
    public IEnumerable<User> Users { get; set; } = null!;
    public IEnumerable<Announcement> Announcements { get; set; } = null!;
    
}
