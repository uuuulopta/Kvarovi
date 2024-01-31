namespace Kvarovi.Contexts;

using Bogus;
using Entities;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;


public class Fakes
{
    static List<string> shared = new();
    static Random r = new();
    static int chance = 33;
    static Faker faker = new Faker(locale: "hr");



    public static string generateText(int sharedWordCount)
    {
        string text = faker.Random.Words(100);
        for (int i = 0; i < sharedWordCount; i++)
        {
            text += " " + faker.PickRandom(shared);
        }

        text += " ";
        
        text += faker.Random.Words(100);
        return text;
    }
    static string getRandomOrShared()
    {

        if (r.Next(100) < chance)
            return shared.Count == 0 ? faker.Random.Word() : faker.PickRandom<string>(shared);
        else return faker.Random.Word();

    }
    
    
    public sealed class KeywordFaker : Faker<Keyword>
    {
        List<string> toShare = new();
        public static int countGen = 1 ;
        public KeywordFaker(): base(locale:"hr")
        {
            RuleFor(k => k.KeywordId, _ => countGen++);
            RuleFor(k => k.Word, f =>
            {
                var word = "";
                if (r.Next(100) < chance) word = getRandomOrShared();
                toShare.Add(word);
                return word;
            });

        }

        public override List<Keyword> Generate(int count, string ruleSets = null)
        {
        
            var gen =  base.Generate(count,
                ruleSets);
        
            shared = shared.Union(toShare).ToList();
           return gen; 
        }
    }

    public class UserFaker : Faker<User>
    {
        public static int countGen = 1;

        public UserFaker() : base(locale: "hr")
        {
            RuleFor(u => u.DeviceToken, f => f.Random.Guid().ToString());
            RuleFor(u => u.UserId, _ => countGen++);
        } 
    }

    public sealed class KeywordUserFaker : Faker<KeywordUser>
    {
        public static int countGen = 1;

        public KeywordUserFaker() : base(locale: "hr")
        {
            RuleFor(kwu => kwu.KeywordUserId,
                _ => countGen++);
            RuleFor(kwu => kwu.UserId, f => f.Random.Number(1,UserFaker.countGen));
            RuleFor(kwu => kwu.KeywordId, f => f.Random.Number(1,KeywordFaker.countGen));
        }
    }
    


}
