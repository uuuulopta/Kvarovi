namespace Kvarovi.AnnouncementGetters;

using System.Globalization;
using System.Net.Http.Headers;
using System.Numerics;
using Cyrillic.Convert;
using HtmlAgilityPack;

// www.bvk.rs/planirani-radovi & www.bvks.rs/kvarovi-na-mrezi
// -----------------------------------------------------------
// p.toggler -> title data
// div.toggle_content ->  text data 
public class WaterAnnouncementGetter : AnnouncementGetter
{

    public WaterAnnouncementGetter() : base(){}
   
    public override async Task<Dictionary<string, List<string>>> getAnnouncements(AnnouncementUrl url)
    {
     
        var html = await getPage(url);
        var titles = html.DocumentNode.SelectNodes("//p[contains(@class, 'toggler')]");
        var texts = html.DocumentNode.SelectNodes("//div[contains(@class, 'toggle_content')]");
        var res = new Dictionary<string, List<string>>();
        for (int i = 0; i < titles.Count; i++)
        {
            res.Add(titles[i].InnerText.ToSerbianLatin(),
                new List<string>{ texts[i].InnerText.ToSerbianLatin() } );
        }

        return res;


    }


    public override DateTime? parseAnnouncementDate(string title)
    {
        var dateStr = title.Substring(0,10);
        try
        {
            return DateTime.ParseExact(dateStr, "yyyy.MM.dd", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
