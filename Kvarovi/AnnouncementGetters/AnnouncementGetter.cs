namespace Kvarovi.AnnouncementGetters;

using System.Net.Http.Headers;
using Entities;
using HtmlAgilityPack;
using Models;
using Utils;

public abstract class AnnouncementGetter
{

    protected ILogger _logger;
    private HttpClient _client = new();
    protected AnnouncementGetter(ILogger logger){

        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        _logger = logger;
    }
    /// <summary>
    ///  Tries to get an html page with exponential backoff.
    /// </summary>
    /// <returns>HTMLDocument or null if it failed to get it </returns>
    protected  async Task<HtmlDocument?> getPage(AnnouncementUrl url)
    {
        HttpResponseMessage? resp;
        try{
            resp = await Retry.DoAsync<HttpResponseMessage>(
                () => _client.GetAsync(url.ToString()),
                validateResult: _ => true,
                maxDelayMilliseconds: 3000,
                delayMilliseconds: 1000,
                onEachFail: ((i, exception) =>
                    _logger.LogError($"Failed to get {url.ToString()} {i}/10, Error: {exception.Message} "))
            );
        }
        catch
        {
            resp = null;
        }

        if (resp == null || !resp.IsSuccessStatusCode) return null;
        var content = await resp.Content.ReadAsStringAsync();
        // Console.WriteLine(content);
        var html = new HtmlDocument();
        html.LoadHtml(content);
        return html;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A list of tuples where 1st is title, and 2nd is text</returns>
    public abstract Task<AnnouncementData> getAnnouncements(AnnouncementUrl url);

    public abstract DateTime? parseAnnouncementDate(string title);

}
