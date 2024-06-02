namespace Kvarovi.Repository;

using Entities;

public interface IAnnouncementRepository
{
    public Task<List<Announcement>> getAnnouncementsByTitle(string title);

    public Task<Announcement?> getAnnouncementByTitle(string title);
    public Task<Announcement?> getRootAnnouncement(string title);
    public Task<Announcement?> getAnnouncementByHash(Int32 hash);

    public Task<IEnumerable<Announcement>> GetAnnouncementsByUserId(int userId);

    public Task<IEnumerable<ClientData>> GetAppDataForUser(int userId);

    public Task<List<User>> getUsersByTheirKeywordsInTextOrAnnouncementKeywords(string text,
        IEnumerable<string> announcementKeywords);
    public Task<Keyword?> getKeywordByWord(string word);
    public Task<IEnumerable<Keyword>> GetKeywordsByWords(IEnumerable<string> words);
    public Task<IEnumerable<Keyword>> getKeywordsByUserIdAsync(int userId);
    public Task addAnnouncement(Announcement a);
    public Task setUserAsNotifiedForAnnouncement(User u, Announcement a);
    public Task setUsersAsNotifiedForAnnouncement(IEnumerable<User> u, Announcement a);
    public Task<User?> getUserByToken(string token);
    public Task<List<User>> getUsersNotNotified(Announcement a);
    
    public Task deleteDeviceByToken(string token);
    public Task<bool> registerUser(string token,string key);
    public Task EditKeywordsForUser(int userId, IEnumerable<string> keywords);
    public Task AddMissingKeywordRange(List<string> keywords);
    public void UpdateEntity<T>(T Entity);
    public Task saveChangesAsync();
    public void clearTracker();

    public string getTrackerChangesDebug();
    
    public void attachEntity<T>(T entity);
    public void attachEntityRange(params object[] entities);
    public void deattachEntity<T>(T entity);
    
    


}