namespace Kvarovi.Repository;

using Entities;

public interface IAnnouncementRepository
{
    public Task<List<Announcement>> getAnnouncementsByTitle(string title);

    public Task<Announcement?> getAnnouncementByTitle(string title);
    public Task<Announcement?> getRootAnnouncement(string title);
    public Task<Announcement?> getAnnouncementByHash(Int32 hash);
    
    public Task<List<User>> getUsersByTheirKeywordsInText(string text);
    public Task<Keyword?> getKeywordByWord(string word);
    public Task addAnnouncement(Announcement a);
    public Task setUserAsNotifiedForAnnouncement(User u, Announcement a);
    public Task setUsersAsNotifiedForAnnouncement(IEnumerable<User> u, Announcement a);
    public Task<User?> getUserByToken(string token);
    public Task<List<User>> getUsersNotNotified(Announcement a);
    
    public Task deleteDeviceByToken(string token);
    public Task registerUser(string token);
    public Task EditKeywordsForUser(string token, IEnumerable<string> keywords);
    public void UpdateEntity<T>(T Entity);
    public Task saveChangesAsync();
    public void clearTracker();

    public string getTrackerChangesDebug();
    
    public void attachEntity<T>(T entity);
    public void attachEntityRange(params object[] entities);
    public void deattachEntity<T>(T entity);


}
