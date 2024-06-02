namespace Kvarovi.AnnouncementGetters;

using EntityConfigs;
using Models;

public class AnnouncementGetterFactory(ILoggerFactory loggerFactory)
{


    ILoggerFactory _loggerFactory = loggerFactory ?? throw new NullReferenceException();

    public  Task<AnnouncementData> getAnnouncements(AnnouncementUrl url)
   {
       
       if (url.ToString() == AnnouncementUrl.vodovodplanned.ToString() || url.ToString() == AnnouncementUrl.vodovodkvar.ToString())
       {
           return new WaterAnnouncementGetter(_loggerFactory.CreateLogger<WaterAnnouncementGetter>()).getAnnouncements(url);
       }

       List<string> epsurls = new() {
           AnnouncementUrl.EpsToday.ToString(),
           AnnouncementUrl.EpsTommorow.ToString(),
           AnnouncementUrl.Eps2days.ToString(),
           AnnouncementUrl.Eps3days.ToString(),
           AnnouncementUrl.EpsAllDays.ToString(),
       };
       if(epsurls.Contains(url.ToString()))
       {
           return new ElectricityAnnouncementGetter(_loggerFactory.CreateLogger<ElectricityAnnouncementGetter>()).getAnnouncements(url);
       }
       throw new ArgumentException("Such AnnouncementUrl doesn't exist");
   }
    public AnnouncementGetter getAnnouncementGetter(AnnouncementTypeEnum ate)
      {
          if (ate == AnnouncementTypeEnum.vodovod)
          {
              return new WaterAnnouncementGetter(_loggerFactory.CreateLogger<WaterAnnouncementGetter>());
          }
          
          if (ate == AnnouncementTypeEnum.eps)
          {
              return new ElectricityAnnouncementGetter(_loggerFactory.CreateLogger<ElectricityAnnouncementGetter>());
          }

          throw new NotImplementedException();
      }
}
