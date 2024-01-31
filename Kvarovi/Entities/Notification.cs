namespace Kvarovi.Entities;

public class Notification
{
   public int NotificationId;
   public User User { get; set; } = null!;
   public Announcement Announcement { get; set; } = null!;
}
