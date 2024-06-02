namespace Kvarovi.Repository;

using Contexts;
using Entities;
using Microsoft.EntityFrameworkCore;

public class UserRepository(MySqlContext cont,ILogger<AnnouncementRepository> logger) : IUserRepository
{
    readonly MySqlContext _context = cont ?? throw new NullReferenceException(nameof(cont));
    readonly ILogger<AnnouncementRepository> _logger = logger ?? throw new NullReferenceException(nameof(logger));
    public async Task<User?> getUserByApiKeyAsync(string value)
    {
        return await _context.ApiKeys.Where(apk => apk.Value == value).Select(apk => apk.User).FirstOrDefaultAsync();
    }
}
