namespace Kvarovi.Entities;

public class Announcement
{
    public int AnnouncementId; 
    public AnnouncementType AnnouncementType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }
    public IEnumerable<Keyword> Keywords { get; set; } = null!;
}
