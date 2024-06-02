namespace Kvarovi.Services.AnnouoncementUpdate;

using Contexts;
using Entities;
using Models;
using Repository;

public class UserNotifierFactory( IAnnouncementRepository repository, ILogger<UserNotifier> logger)
{

    public UserNotifier createUserNotifier(Announcement a)
    {
        return new UserNotifier(repository,logger,a);
    }
}
