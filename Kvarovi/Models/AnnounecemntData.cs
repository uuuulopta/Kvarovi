namespace Kvarovi.Models;

using AnnouncementGetters.KeywordsParsers;

public class AnnouncementData
{
    public List<( string, string )> TitlesTexts { get; }
    KeywordsParserStrategy? Parser { get; set; }

    public AnnouncementData(List<(string,string)> titlesText, KeywordsParserStrategy? parser)
    {
        TitlesTexts = titlesText;
        Parser = parser;
    }

    /// <summary>
    /// Returns keywords if a KeywordsParserStrategy has been set
    /// </summary>
    /// <returns></returns>
    public List<string> getKeywords(string text)
    {
        var kws = new List<string>();
        if(Parser == null) return kws;
        kws.AddRange(Parser.getKeywordsFromText(text.ToLower()));

        return kws;
    } 
}
