namespace Kvarovi.AnnouncementGetters;

using EntityConfigs;

public class AnnouncementGetterFactory
{
   
   public static Task<Dictionary<string, List<string>>> getAnnouncements(AnnouncementUrl url)
   {
       
       if (url.ToString() == AnnouncementUrl.vodovodplanned.ToString() || url.ToString() == AnnouncementUrl.vodovodkvar.ToString())
       {
           return new WaterAnnouncementGetter().getAnnouncements(url);
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
           return new ElectricityAnnouncementGetter().getAnnouncements(url);
       }
       throw new ArgumentException("Such AnnouncementUrl doesn't exist");
   }
    public static AnnouncementGetter getAnnouncementGetter(AnnouncementTypeEnum ate)
      {
          if (ate == AnnouncementTypeEnum.vodovod)
          {
              return new WaterAnnouncementGetter();
          }
          
          if (ate == AnnouncementTypeEnum.eps)
          {
              return new ElectricityAnnouncementGetter();
          }

          throw new NotImplementedException();
      }
}
