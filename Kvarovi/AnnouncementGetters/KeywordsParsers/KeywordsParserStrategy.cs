namespace Kvarovi.AnnouncementGetters.KeywordsParsers;

using Entities;

public interface KeywordsParserStrategy
{
    /// <summary>
    ///  Parses the text for street names, returning a list of strings that are in format  "StreetName Number"
    /// </summary>
    /// <returns>A list of street names with a number at the end</returns>
    public IEnumerable<string> getKeywordsFromText(string data);
}
