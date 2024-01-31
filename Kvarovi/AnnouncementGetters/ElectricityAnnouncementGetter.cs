namespace Kvarovi.AnnouncementGetters;

using System.Globalization;
using Cyrillic.Convert;
using Microsoft.AspNetCore.Components.Web;

public class ElectricityAnnouncementGetter : AnnouncementGetter
{

    
/*

https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_0_Iskljucenja.htm
(ide od dan 0-3)
----------------------------------------------------------------------------------
table -> tr sa 3 td (opstina,vreme,ulice)
znaci pretrazujes po tr preskocis 1.

string elektro = "https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_0_Iskljucenja.htm";
*/


    private async Task<(string? title, List<string>? data)> getAnnouncement(AnnouncementUrl url)
    {
        var html = await getPage(url);
        if (html == null) return (null,null);
        var title = html.DocumentNode.SelectSingleNode("//td").InnerText.ToSerbianLatin();
        var tr = html.DocumentNode.SelectNodes("//table[2]//tr");
        List<string> data = new();
        foreach (var row in tr.Skip(1))
        {
            string rowText = "";
            foreach (var column in row.SelectNodes("./td"))
            {
                rowText += " " + column.InnerText.ToSerbianLatin();
            } 
            data.Add(rowText);
        }
        

        return (title, data);

    }
  
    public override async Task<Dictionary<string, List<string>>> getAnnouncements(AnnouncementUrl url)
    {
        string? title;
        List<string>? data;
        var res = new Dictionary<string, List<string>>();
        if (url.ToString() != AnnouncementUrl.EpsAllDays.ToString() )
        {
            (title, data ) = (await getAnnouncement(url));
            addToDictionaryIfNotNull(ref res,title,data);
            return res;
        }
        (title, data ) = (await getAnnouncement(AnnouncementUrl.EpsToday));
        addToDictionaryIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.EpsTommorow));
        addToDictionaryIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.Eps2days));
        addToDictionaryIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.Eps3days));
        addToDictionaryIfNotNull(ref res,title,data);
        return res;
    }

    /// <summary>
    /// Parses a date in yyyy-mM-dd format 
    /// </summary>
    /// <param name="title">yyyy-MM-dd string</param>
    /// <returns></returns>
    public override DateTime? parseAnnouncementDate(string title)
    {
        var dateStr = title.Substring(title.Length - 10);
        try
        {
            return DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private void addToDictionaryIfNotNull(ref Dictionary<string, List<string>> dict,string? title,List<string>? data)
    {
        if (title == null) return;
        else
            dict.Add(title, data!);
    }

   
}
