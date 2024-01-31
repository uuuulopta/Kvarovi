namespace Kvarovi.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class Announcement
{
    public int AnnouncementId; 
    public AnnouncementType AnnouncementType { get; set; } = null!;
    
    public int AnnouncementTypeId { get; set; }
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime? Date { get; set; }
    public Int32 Hash { get; set; }
    
    public bool Update { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "TIMESTAMP")] 
    public DateTime Timestamp { get; set; }

    public List<User> Users { get; set; } = new List<User>();
}
