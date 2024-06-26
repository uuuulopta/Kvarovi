﻿namespace Kvarovi.Entities;

using AnnouncementGetters;

public class Keyword
{
    
    public int KeywordId; 
    public string Word { get; set; } = null!;
    
    public IEnumerable<User> Users { get; set; } = null!;
    public List<Announcement> Announcements { get; set; } = null!;

    public int[] UserIds { get { return Users.Select(c => c.UserId).ToArray(); } }

}
