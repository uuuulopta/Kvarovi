namespace Kvarovi.AnnouncementGetters;

using System.Globalization;
using System.Text;
using Cyrillic.Convert;
using Entities;
using KeywordsParsers;
using Microsoft.AspNetCore.Components.Web;
using Models;
using HostingEnvironmentExtensions = Microsoft.Extensions.Hosting.HostingEnvironmentExtensions;

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
    public ElectricityAnnouncementGetter(ILogger logger) : base(logger)
    {
    }


    public async Task<(string,string)> getAnnouncement(AnnouncementUrl url)
    {


        var (title,dataText) = await parseHTML(url);
        if (string.IsNullOrWhiteSpace(dataText)) return ("", "");
       
        return (title, dataText);

    }


    protected virtual async Task<( string,string )> parseHTML(AnnouncementUrl url)
    {
        var html = await getPage(url);
        if (html == null) return ("","");
        var title = html.DocumentNode.SelectSingleNode("//td").InnerText.ToSerbianLatin();
        var tr = html.DocumentNode.SelectNodes("//table[2]//tr");
        string dataText = "";
        foreach (var row in tr.Skip(1))
        {
            string rowText = "";
            foreach (var column in row.SelectNodes("./td"))
            {
                rowText += " " + column.InnerText.ToSerbianLatin();
            } 
            dataText += rowText;
        }

        return ( title,dataText );
    }
    public override async Task<AnnouncementData> getAnnouncements(AnnouncementUrl url)
    {
        string title;
        string data;
        var res = new List<(string, string)>();
        if (url.ToString() != AnnouncementUrl.EpsAllDays.ToString() )
        {
            (title, data ) = (await getAnnouncement(url));
            addToListIfNotNull(ref res,title,data);
            return new AnnouncementData(res,new ElectricityKeywordsParserStrategy());
        }
        (title, data ) = (await getAnnouncement(AnnouncementUrl.EpsToday));
        addToListIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.EpsTommorow));
        addToListIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.Eps2days));
        addToListIfNotNull(ref res,title,data);
        
        (title, data ) = (await getAnnouncement(AnnouncementUrl.Eps3days));
        addToListIfNotNull(ref res,title,data);
        return new AnnouncementData(res,new ElectricityKeywordsParserStrategy());
    }

    /// <summary>
    /// Parses a date in yyyy-mM-dd format 
    /// </summary>
    /// <param name="title">yyyy-MM-dd string</param>
    /// <returns></returns>
    public override DateTime? parseAnnouncementDate(string title)
    {
        try
        {
            
            var dateStr = title.Substring(title.Length - 10);
            return DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private void addToListIfNotNull(ref List<(string,string)> l,string? title,string data)
    {
        if (title == null) return;
        else l.Add(( title, data ));
    }


    
}
