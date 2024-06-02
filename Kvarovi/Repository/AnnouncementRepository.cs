namespace Kvarovi.Repository;

using System.Data;
using System.Diagnostics.CodeAnalysis;
using AnnouncementGetters.CurrentOrPlannetStrategies;
using Bogus.DataSets;
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
        
        return await _context.Announcements.Where(a => a.Title == title).OrderBy(a => a.Timestamp).FirstOrDefaultAsync();
    }

    public async Task<Announcement?> getAnnouncementByHash(Int32 hash)
    {
        return await _context.Announcements.Where(a => a.Hash == hash).OrderBy(a => a.Timestamp).AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Announcement>> GetAnnouncementsByUserId(int userId)
    {
        return await _context.Announcements.Where(a => a.Users.Select(u => u.UserId).Contains(userId)).ToListAsync();
    }

    public async Task<IEnumerable<ClientData>> GetAppDataForUser(int userId)
    {
        
        // Example: {Eps} : { Current : { ClientData[]} }

        var anns = await _context.Users
            .Where(u => u.UserId == userId)
            .Include(u => u.Announcements)
            .ThenInclude(a => a.AnnouncementType)
            .Select(u => u.Announcements.Where(a => a.Date != null && DateTime.Today <= a.Date).ToList())
            .FirstOrDefaultAsync();
     

        var res = new List<ClientData>();
        if (anns == null || anns.Count == 0) return res;
        foreach (var a in anns)
        {
         
            var workType = new DateWorkTypeStrategy().determineWorkType(a);
            var ad = new ClientData()
            {
                Id = a.AnnouncementId.Value,
                Title = a.Title,
                Date = a.Date.HasValue ? $"{a.Date.Value.Day}.{a.Date.Value.Month}.{a.Date.Value.Year}" : "",
                Text = a.Text,
                WorkType = Enum.GetName(workType)!,
                AnnouncementType = a.AnnouncementType.AnnouncementTypeName
            };
            res.Add(ad);
        }

        return res;
    }


    public async Task<List<User>> getUsersByTheirKeywordsInTextOrAnnouncementKeywords(string text,IEnumerable<string> announcementKeywords)
    {
        return await _context.Users.Where(u => 
            u.Keywords.Any(
                kw => text.ToLower().Contains(kw.Word.ToLower()) 
                      || announcementKeywords.Contains(kw.Word.ToLower())
            ))
            .Include(u => u.Announcements)
            .Include(u => u.Keywords)
            .Include(u => u.ApiKeys)
            .ToListAsync();
    }

    public async Task<Keyword?> getKeywordByWord(string word)
    {
        return await _context.Keywords.Where(kw => kw.Word == word).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Keyword>> GetKeywordsByWords(IEnumerable<string> words)
    {
        
        words = words.Select(word => word.ToLower());
        var a = await _context.Keywords
            .Where(k => words.Contains(k.Word.ToLower()))
            .Include(k => k.Users)
            .Include(k => k.Announcements)
            .ToListAsync();
        return a;
    }

    public async Task<IEnumerable<Keyword>> getKeywordsByUserIdAsync(int userId)
    {
        var k =  await _context.Users.Where(u => u.UserId == userId)
            .Include(u => u.Keywords)
            .Select(u => u.Keywords).FirstOrDefaultAsync();
        
        return k ?? new List<Keyword>();
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
        //TODO used while testing, remove it
        return;
        var user = await getUserByToken(token); 
        if(user != null ) _context.Remove(user);
        
    }

    /// <summary>
    /// Returns true if registered a new user, false if it added another api key for the user.
    /// </summary>
    /// <param name="token">A token for given device ( ExponentPushToken for android )</param>
    /// <param name="key">An api key</param>
    /// <returns></returns>
    public async Task<bool> registerUser(string token,string key)
    {
        var u = await getUserByToken(token);
        if (u != null)
        {
            u.ApiKeys.Add(new ApiKey(key)) ;
            return false;
        }
        await _context.Users.AddAsync(new User() { DeviceToken = token,ApiKeys = new List<ApiKey>() {new ApiKey(key)} });
        return true;
    }

    public async Task EditKeywordsForUser(int userId, IEnumerable<string> keywords)
    {
        var user = await _context.Users.Where(u => u.UserId == userId).Include(u => u.Keywords)
            .Include(u => u.Announcements)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            _logger.LogCritical($"Tried to edit keywords for non-existent userId={userId}");
            return;
        }

        _context.AttachRange(_context.Keywords.Where(k => keywords.Contains(k.Word)));
        user.Keywords = await KeywordMapper.ToKeywordAsync(keywords,this);
        _context.Update(user);


        // return await _context.Users.Where(u => 
        //     u.Keywords.Any(
        //         kw => text.ToLower().Contains(kw.Word.ToLower()) 
        //               || announcementKeywords.Contains(kw.Word.ToLower())
        //     ))
        //     .Include(u => u.Announcements)
        //     .Include(u => u.Keywords)
        //     .Include(u => u.ApiKeys)
        //     .ToListAsync();
       var anns =  user.Announcements.Where(a => keywords.Any(k => a.Text.ToLower().Contains(k.ToLower())
                                 || a.Keywords.Select(kw => kw.Word).Contains(k.ToLower())))
                                 .ToList();
       user.Announcements = anns;
       
       



    }

    [SuppressMessage("ReSharper.DPA",
        "DPA0006: Large number of DB commands",
        MessageId = "count: 1066")]
    public async Task AddMissingKeywordRange(List<string> keywords)
    {
        
        
        _logger.LogInformation($"Getting missing keywords for announcement...");
        var missingRecords = keywords.Where(keyword => !_context.Keywords.Any(z => z.Word.ToLower().Equals(keyword.ToLower()))).ToList();
        _logger.LogInformation($"Missing keywords: {missingRecords.Count} / {keywords.Count} ");
        var entities = missingRecords.Select(k => new Keyword() { Word = k.ToLower() });
        await _context.Keywords.AddRangeAsync(entities);
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

    
    public void attachKeywordsRange(params object[] entities)
    {
        _context.AttachRange(entities);
    }
    public void deattachEntity<T>(T entity)
    {
        
        if (entity == null) throw new ArgumentNullException();
        _context.Entry(entity).State = EntityState.Detached;
    }


}
