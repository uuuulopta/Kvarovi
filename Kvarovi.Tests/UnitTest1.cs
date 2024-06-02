namespace Kvarovi.Tests;

using Contexts;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Repository;
using Xunit.Abstractions;

public class UnitTest1
{
    readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static MySqlContext getDbContext(int gencount)
    {
        var options = new DbContextOptionsBuilder<MySqlContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var dbContext = new MySqlContext(options);
        dbContext.Database.EnsureCreated();
        
        var kwsFaker = new Fakes.KeywordFaker();
        var userFaker = new Fakes.UserFaker();
        var kwusFaker = new Fakes.KeywordUserFaker();

        dbContext.AddRange(kwsFaker.Generate(gencount));
        dbContext.AddRange(userFaker.Generate(gencount));
        dbContext.AddRange(kwusFaker.Generate(gencount));
        dbContext.SaveChanges();
        return dbContext;

    }

    private static MySqlContext getRealDbContext()
    {
        var options = new DbContextOptionsBuilder<MySqlContext>();
        string connString = "Server=127.0.0.1;Port=3306;Database=kvaroviTest;Uid=root;CharSet=utf8;default command timeout=0;";
        options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        var dbContext = new MySqlContext(options.Options);
        // dbContext.Database.EnsureDeleted();
        // dbContext.Database.EnsureCreated();
        return dbContext;
    }

    private static IAnnouncementRepository getRepository(MySqlContext context)
    {
        return new AnnouncementRepository(context,new NullLogger<AnnouncementRepository>());
    }
    static MySqlContext context = null!;
    static IAnnouncementRepository repository = null!;
    [Fact]
    public void InitialEnvironment()
    {
        context = getDbContext(100);
        repository = getRepository(context);
    }

    [Fact]
    public async void getUsersByTheirKeywordsInTextTest()
    {
        var _context = getDbContext(0);
        var _repository = getRepository(_context);
        var text = "";
        string secret = "blablabla";
        text += secret;
        _context.Users.Add(new User(){DeviceToken = "sometoken",UserId = 100});
        _context.Users.Add(new User(){DeviceToken = "sometoken2",UserId = 200});
        _context.Keywords.Add(new Keyword() { Word = secret, KeywordId = 100 });
        _context.Keywords.Add(new Keyword() { Word = "boris", KeywordId = 200 });
        _context.KeywordUsers.Add(new KeywordUser() { KeywordUserId = 100, KeywordId = 100, UserId = 100 });
        _context.KeywordUsers.Add(new KeywordUser() { KeywordUserId = 200, KeywordId = 200, UserId = 200 });
        _context.SaveChanges();
        var res = await _repository.getUsersByTheirKeywordsInTextOrAnnouncementKeywords(text,new List<string>(){"boris"});
        Assert.Equal(2,res.Count()); 
        res = await _repository.getUsersByTheirKeywordsInTextOrAnnouncementKeywords("",new List<string>(){"boris"});
        Assert.Equal(200,res[0].UserId); 
        res = await _repository.getUsersByTheirKeywordsInTextOrAnnouncementKeywords(text,new List<string>());
        Assert.Equal(100,res[0].UserId); 
    
    }
    

    [Fact]
    public async void addMissingKeywords()
    {
        var context = getDbContext(0);
        var repo = getRepository(context);
        context.Keywords.AddRange(new Keyword(){Word = "1"},new Keyword(){Word = "2"},new Keyword(){Word = "3"} );
        context.SaveChanges();
        var stringKws = new List<string>() { "1", "2", "3", "4", "5" };
        await repo.AddMissingKeywordRange(stringKws);
        await repo.saveChangesAsync();
        Assert.Equal(5,context.Keywords.Count());
        var kws = context.Keywords.ToList();
        var kwsShallow = new List<string>(stringKws);
        foreach (var keyword in kws)
        {
            _testOutputHelper.WriteLine(keyword.Word);
            if( !kwsShallow.Remove(keyword.Word) ) Assert.Fail("Keywords in the database dont match those that were added");
        }
        await repo.AddMissingKeywordRange(stringKws);
        Assert.Equal(5,context.Keywords.Count());
        



    }
    // [Fact]
    // public async void electricityTextParser()
    // {
    //     string test = "Zvezdara 09:00 - 14:00 BILCE: 2-6, Naselje V.MOKRI LUG:   CVETANOVA ĆUPRIJA: 62-76,155-173A,  STARI VINOGRADI: 24-26,41G-43A,47-47P, Zvezdara 08:30 - 13:00 MILANA GLIGORIJEVIĆA: 78,  STRAŽARSKA KOSA: 26-42A,64-66A,11Lj-19,23-25A,29-39, Naselje V.MOKRI LUG:   CVETANOVA ĆUPRIJA: 112-116B,189A-191,  JOVIJANOVA: 38-46,31,37-41,  MITKOV KLADENAC: 36-40,  VLADIMIRA ĆOROVIĆA: 28-32,31-41, Palilula 10:00 - 16:00  Naselje Kovilovo: KOVILOVO: 10,46,9,49A, Palilula 08:30 - 11:30 DANTEOVA: 11G,  GARSIJE LORKE: 2A-2V, Palilula 09:00 - 15:00  Naselje BORČA: J N A: 71,  RATNIH VOJNIH INVALIDA: 44-58,62A-62B,70, Voždovac 10:00 - 11:30 KUMODRAŠKA: 153, Voždovac 08:30 - 13:00 VOJVODE STEPE: 600-636,642,646-650,678-678B,623A,627-687, Naselje KUMODRAŽ:   BOŽIĆNA: 2-8B,3-25A,  JASMINA: 2-20,1-15,  KOVAČKA: 2-8,16-20,1-7A,  PROLEĆNA: 4A-12B,16-32A,36-38,5-15,  REMETINSKA: 2-22,1-9,13-19V,  STARE PORTE: 2-6,10-12A,1-27,  TEKERIŠKA: 2-8,1-7,  TOPOLA: 36-42,46-64,27-49,53,59-67,  UBSKA: 2-4,8-16,1,5-13,  VOĆARSKA: 2-4,1-3,7, Voždovac 08:30 - 15:00  Naselje RIPANj: BELA ZEMLjA: 4A-6,10-12B,18A-30B,34,50,5-7A,13,21,25-31,35A-37,43A,47-59,67,  BRĐANSKA: 1,  PUT ZA GRUJIČIĆE: 2A,  PUT ZA KALAJDžIĆE: 16-32,9,15-23,27,31-33,37,41,  PUT ZA KLENjE: 2-20A,24-30,34,1-13B,17-21,29,  PUT ZA KOVANLUK: 2-12,16,22,28,1-5,11-13,  PUT ZA PAVIĆEVAC: 2,1-3D,7-13Đ,  PUT ZA TREŠNjU: 2-28,32-34B,1-11,15-19,23,27A-35,39-47,53-55A,  PUT ZA TREŠNjU 1 DEO: 30,  SREDNjI PRNjAVOR: 2-4,1-1Ž,  STANOJLOVIĆA KRAJ: 2-6,1A-13,  ŽEL STANICA KLENjE: bb, Voždovac 08:30 - 15:00 OZRENSKA: 4-6,1-5,  VIDSKA: 24, Rakovica 13:00 - 14:30 BORSKA: 15A, Zemun 09:00 - 14:00  Naselje ALTINA: RATARSKI PUT: 24-26,36,23-25A,31-37B, Zemun 10:30 - 11:30 NIKOLE SUKNjAREVIĆA PRIKE: 2-18,1-17, Naselje BATAJNICA:   DALMATINSKE ZAGORE : 8,30-70,74,1-21,  STANKA TIŠME: 2-40,31A-31C, Zemun 08:30 - 10:00 BRANKA PEŠIĆA : 6-10A,5-11,  CARA DUŠANA: 136-136A,140,105A-105b,119-123B,  SLAVONSKA: 16A-18V, Surčin 09:00 - 14:00  Naselje PROGAR: VLADE OBRADOVIĆA-KAMENOG: 102A,114-118,124-144,148,81,85,89-91,97,101-101C, Grocka 09:00 - 15:00 29.Novembra 1,1b,4,6,7,8,8/1,9,10,10a,11,12,12a,12b,13,14,15,16,17,18,18a,18b,19,19a,21,23,23a,23c,23/1,23/4,24,28,30,32,34,36,40,42,46,48. Maršala Tita br.1,3. Živojina Stevanovića 1,3,5,6,6d,8,10,10a,12,14,15,17,18,19,20,21,22,23,24.";
    //     var parser = new Mock<ElectricityKeywordsParserStrategy>();
    //     var res = parser.Object.getKeywordsFromText(test);
    //     _testOutputHelper.WriteLine(string.Join('\n',res));
    // }


    [Fact]
    public async void sandbox()
    {

        // var timer = new Stopwatch();
        // timer.Start();
        // var dbContext = getRealDbContext();
        // var dbtime = timer.Elapsed;
        // var _repository = getRepository(dbContext);
        // var eps = await AnnouncementGetterFactory.getAnnouncements(AnnouncementUrl.EpsAllDays);
        //
        // _testOutputHelper.WriteLine("Getting users by their blabal");
        // foreach (var (title, text) in eps.TitlesTexts)
        // {
        //     _testOutputHelper.WriteLine(text);
        //     var getTimer = new Stopwatch();
        //     getTimer.Start();
        //     await _repository.getUsersByTheirKeywordsInTextOrAnnouncementKeywords(text, new List<string>());
        //     getTimer.Stop();
        //     _testOutputHelper.WriteLine("one cycle took" + ( getTimer.Elapsed).ToString(@"m\:ss\.fff"));
        // }
        // _testOutputHelper.WriteLine("Got users" + ( timer.Elapsed - dbtime ).ToString(@"m\:ss\.fff"));
    }

}
