namespace Kvarovi.Services.AnnouoncementUpdate;

using AnnouncementGetters;
using Entities;
using EntityConfigs;
using Models;
using Repository;
using Utils;

public class Updater(IServiceProvider services, AnnouncementGetterFactory announcementGetterFactory)
{

    AnnouncementGetterFactory _announcementGetterFactory = announcementGetterFactory;

    public async Task Update(Task<AnnouncementData> asyncData,AnnouncementTypeEnum ate)
    {
        
        var data = await asyncData;
       
        
        foreach (var (title,text) in data.TitlesTexts )
        {
            if (string.IsNullOrEmpty(title)) continue; 
            
            using (var scope = services.CreateScope() )
            { 
                var _logger = scope.ServiceProvider.GetRequiredService<ILogger<Updater>>();
                var _repository = scope.ServiceProvider.GetRequiredService<IAnnouncementRepository>();
                var _userNotifierFactory = scope.ServiceProvider.GetRequiredService<UserNotifierFactory>();
           
                var root = await _repository.getRootAnnouncement(title);
                bool updateFlag = root != null;
                var keywords = data.getKeywords(text);
                await _repository.AddMissingKeywordRange(keywords);
                await _repository.saveChangesAsync();
                var kwEntities = await _repository.GetKeywordsByWords(keywords);
                Announcement a;
                if (updateFlag) a = root!;
                else a = new Announcement();
                a.AnnouncementTypeId = (int)ate;
                a.Date = _announcementGetterFactory.getAnnouncementGetter(ate).parseAnnouncementDate(title);
                a.Hash = text.GetDeterministicHashCode();
                a.Title = title;
                a.Text = text ;
                a.Keywords = kwEntities.ToList();
                a.Users = await _repository.getUsersByTheirKeywordsInTextOrAnnouncementKeywords(text, keywords);
                if (!updateFlag) await _repository.addAnnouncement(a);
                await _repository.saveChangesAsync();
                await _userNotifierFactory.createUserNotifier(a).sendUserNotificationsForAnnouncement(updateFlag);
             
            }


            
        }
    }

}
