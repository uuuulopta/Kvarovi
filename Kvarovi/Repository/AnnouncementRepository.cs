namespace Kvarovi.Repository;

using System.Data;
using Contexts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Utils.Mappers;

public class AnnouncementRepository(MySqlContext cont,ILogger<AnnouncementRepository> logger) : IAnnouncementRepository
{
    readonly MySqlContext _context = cont ?? throw new NullReferenceException(nameof(cont));
    readonly ILogger<AnnouncementRepository> _logger = logger ?? throw new NullReferenceException(nameof(logger));

    public async Task<List<Announcement>> getAnnouncementsByTitle(string title)
    { 
        return await _context.Announcements.Where(a => a.Title == title).OrderBy(a => a.Timestamp).ToListAsync();
        
    }

    public async Task<Announcement?> getAnnouncementByTitle(string title)
    {
        
        return await _context.Announcements.Where(a => a.Title == title)
            .Include(a => a.Users)
            .OrderBy(a => a.Timestamp).FirstOrDefaultAsync();
    }
    public async Task<Announcement?> getRootAnnouncement(string title)
    {
        
        return await _context.Announcements.Where(a => a.Title == title).OrderBy(a => a.Timestamp).AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<Announcement?> getAnnouncementByHash(Int32 hash)
    {
        return await _context.Announcements.Where(a => a.Hash == hash).OrderBy(a => a.Timestamp).AsNoTracking().FirstOrDefaultAsync();
    }


    public async Task<List<User>> getUsersByTheirKeywordsInText(string text)
    {
        return await _context.Users.Where(u => u.Keywords.Any(kw => text.Contains(kw.Word))).ToListAsync();
    }

    public async Task<Keyword?> getKeywordByWord(string word)
    {
        return await _context.Keywords.Where(kw => kw.Word == word).FirstOrDefaultAsync();
    }

    public async Task addAnnouncement(Announcement a)
    {
        await _context.AddAsync(a);
    }

    public async Task setUserAsNotifiedForAnnouncement(User u, Announcement a)
    {
        await _context.Notifications.AddAsync(new Notification() { User = u, Announcement = a });
    }

    public async Task setUsersAsNotifiedForAnnouncement(IEnumerable<User> u, Announcement a)
    {
        var notifs = new List<Notification>();
        foreach (var user in u)
        {
            notifs.Add(new Notification(){User = user,Announcement = a});    
        }
        
        await _context.Notifications.AddRangeAsync(notifs);
    }

    public async Task<User?> getUserByToken(string token)
    {
        
        return await _context.Users.Where(u => u.DeviceToken == token)
            .Include(u => u.Keywords)
            .FirstOrDefaultAsync();
    }

    public async Task<List<User>> getUsersNotNotified(Announcement a)
    {
        var usersSubscribed = await _context.Notifications.Where(n => n.Announcement.AnnouncementId == a.AnnouncementId)
            .AsNoTracking()
            .Select(u => u.User.UserId)
            .ToListAsync();
        return a.Users.Where(u => usersSubscribed.Contains(u.UserId) == false).ToList();
    }

    public async Task deleteDeviceByToken(string token)
    {
        var user = await getUserByToken(token); 
        if(user != null ) _context.Remove(user);
        
    }

    public async Task registerUser(string token)
    {
        var u = await getUserByToken(token);
        if (u != null) return;
        await _context.Users.AddAsync(new User() { DeviceToken = token });
    }

    public async Task EditKeywordsForUser(string token, IEnumerable<string> keywords)
    {
        var user = await _context.Users.Where(u => u.DeviceToken == token).Include(u => u.Keywords).FirstOrDefaultAsync();

        if (user == null)
        {
            _logger.LogCritical($"Tried to edit keywords for non-existent user {token}");
            return;
        }

        _context.AttachRange(_context.Keywords.Where(k => keywords.Contains(k.Word)));
        user.Keywords = await KeywordMapper.ToKeywordAsync(keywords,this);
        _context.Update(user);
    }

    public void UpdateEntity<T>(T Entity)
    {
        if (Entity == null) throw new ArgumentNullException();
        _context.Update(Entity);
    }

    public async Task saveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void clearTracker()
    {
        _context.ChangeTracker.Clear();
    }

    public string getTrackerChangesDebug()

    {
        var a = _context.ChangeTracker.Entries();
        return _context.ChangeTracker.DebugView.ShortView;
    }

    public void attachEntity<T>(T entity)
    {
        
        if (entity == null) throw new ArgumentNullException();
        _context.Attach(entity);
    }

    public void attachEntityRange(params object[] entities)
    {
        _context.AttachRange(entities);
    }

    public void deattachEntity<T>(T entity)
    {
        
        if (entity == null) throw new ArgumentNullException();
        _context.Entry(entity).State = EntityState.Detached;
    }


}
