namespace Kvarovi.AnnouncementGetters;

using System.Net.Http.Headers;
using HtmlAgilityPack;

public abstract class AnnouncementGetter
{
    
    private HttpClient _client = new();
    protected AnnouncementGetter(){

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
    }
    protected  async Task<HtmlDocument?> getPage(AnnouncementUrl url)
        {
            var resp = await _client.GetAsync(url.ToString());
            if (!resp.IsSuccessStatusCode) return null;
            var content = await resp.Content.ReadAsStringAsync();
            content = content.ToLower();
            // Console.WriteLine(content);
             var html = new HtmlDocument();
             html.LoadHtml(content);
             return html;
        }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns>A dictionary where key is the title (of the day) and value is the data/text</returns>
    public abstract Task<Dictionary<string,List<string>>> getAnnouncements(AnnouncementUrl url);

    public abstract DateTime? parseAnnouncementDate(string title);

}
