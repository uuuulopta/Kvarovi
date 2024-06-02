namespace Kvarovi.AnnouncementGetters;

using System.Globalization;
using System.Net.Http.Headers;
using System.Numerics;
using Cyrillic.Convert;
using HtmlAgilityPack;
using Models;

// www.bvk.rs/planirani-radovi & www.bvks.rs/kvarovi-na-mrezi
// -----------------------------------------------------------
// p.toggler -> title data
// div.toggle_content ->  text data 
public class WaterAnnouncementGetter : AnnouncementGetter
{


    public WaterAnnouncementGetter(ILogger logger) : base(logger){}

    public override async Task<AnnouncementData> getAnnouncements(AnnouncementUrl url)
    {
     
        var html = await getPage(url);
        if (html == null) return new AnnouncementData(new List<(string, string)>(),null); 
        var titles = html.DocumentNode.SelectNodes("//p[contains(@class, 'toggler')]");
        var texts = html.DocumentNode.SelectNodes("//div[contains(@class, 'toggle_content')]");
        var res = new List<(string, string)>();
        for (int i = 0; i < titles.Count; i++)
        {
            res.Add(( titles[i].InnerText.ToSerbianLatin(), texts[i].InnerText.ToSerbianLatin()  ));
        }

        return new AnnouncementData(res,null); 


    }


    public override DateTime? parseAnnouncementDate(string title)
    {
        var dateStr = title.Substring(0,10);
        try
        {
            return DateTime.ParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return null;
        }
    }

}
