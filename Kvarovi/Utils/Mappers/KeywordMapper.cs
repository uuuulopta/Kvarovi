namespace Kvarovi.Utils.Mappers;

using Entities;
using Repository;

public static class KeywordMapper
{
   public static async Task<Keyword> ToKeywordAsync(string keyword,IAnnouncementRepository repo)
   {

      return await repo.getKeywordByWord(keyword) ?? new Keyword() { Word = keyword };
   } 
   
   public static async Task<List<Keyword>> ToKeywordAsync(IEnumerable<string> keywords,IAnnouncementRepository repo)
   {
      var res = new List<Keyword>();
      foreach (var keyword in keywords)
      {
         res.Add(await ToKeywordAsync(keyword,repo));
      }

      return res;
   } 
}
