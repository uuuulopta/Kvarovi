namespace Kvarovi.Repository;

using Entities;

public interface IUserRepository
{
    public Task<User?> getUserByApiKeyAsync(string value);
    





}
