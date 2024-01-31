namespace Kvarovi.Services.AnnouoncementUpdate;

using Kvarovi.AnnouncementGetters;
using Kvarovi.Entities;
using Kvarovi.EntityConfigs;
using Kvarovi.Repository;
using Kvarovi.Utils;
public class Updater
{
    IAnnouncementRepository _repository;
    ILogger<Updater> _logger;
    IServiceProvider _provider;
    public Updater(IServiceProvider services)
    {
        _provider = services;
        var scope = services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<Updater>>();
        _repository = scope.ServiceProvider.GetRequiredService<IAnnouncementRepository>();
    }

    public async Task Update(Task<Dictionary<string, List<string>>> asyncData,AnnouncementTypeEnum ate)
    {
        var data = await asyncData;
        foreach (var entry in data )
        {
            var title = entry.Key;
            var texts = entry.Value;
            var root = await _repository.getRootAnnouncement(title);
            foreach (var text in texts)
            {
                var a = new Announcement()
                {
                   AnnouncementTypeId = (int)ate,
                    Date = AnnouncementGetterFactory.getAnnouncementGetter(ate).parseAnnouncementDate(title),
                    Hash = text.GetDeterministicHashCode(),
                    Title = title,
                    Text = text,
                    Update = await isAnnouncementAnUpdate(title),
                    Users = (await _repository.getUsersByTheirKeywordsInText(text)),
                };
                
                _repository.clearTracker();
                await new UserNotifier(_provider,a).sendUserNotificationsForAnnouncement();
                
                
                if( ( await _repository.getAnnouncementByHash(a.Hash) == null ))
                     _repository.attachEntity(a);
                await _repository.saveChangesAsync();


            }
        }
    }

    public async Task<bool> isAnnouncementAnUpdate(string title)
    {
        var root = await _repository.getRootAnnouncement(title);
        return root != null;

    }
}
