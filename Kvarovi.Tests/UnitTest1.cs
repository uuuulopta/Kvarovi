namespace Kvarovi.Tests;

using System.Globalization;
using System.Text;
using AnnouncementGetters;
using Bogus;
using Contexts;
using Entities;
using EntityConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Repository;
using Services;
using Services.AnnouoncementUpdate;
using Utils;
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

        dbContext.AddRange(kwsFaker.Generate((int)gencount/2));
        dbContext.AddRange(kwsFaker.Generate(gencount));
        dbContext.AddRange(userFaker.Generate(gencount));
        dbContext.AddRange(kwusFaker.Generate(gencount));
        dbContext.SaveChanges();
        return dbContext;

    }

    private static MySqlContext getRealDbContext()
    {
        var options = new DbContextOptionsBuilder<MySqlContext>();
        string connString = "Server=127.0.0.1;Port=3306;Database=kvaroviTest;Uid=root;CharSet=utf8;";
        options.UseMySql(connString, ServerVersion.AutoDetect(connString));
        var dbContext = new MySqlContext(options.Options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
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
        var _context = getDbContext(1);
        var _repository = getRepository(_context);
        var text = Fakes.generateText(30);
        string secret = "blablabla";
        text += secret;
        text += " " + secret;
        _context.Users.Add(new User(){DeviceToken = "sometoken",UserId = 100});
        _context.Keywords.Add(new Keyword() { Word = secret, KeywordId = 100 });
        _context.KeywordUsers.Add(new KeywordUser() { KeywordUserId = 100, KeywordId = 100, UserId = 100 });
        _context.SaveChanges();
        var res = await _repository.getUsersByTheirKeywordsInText(text);
        foreach (var re in res)
        {
            if (re.UserId != 100) continue;
            foreach (var k in re.Keywords)
            {
            
                _testOutputHelper.WriteLine(k.Word);
            }

            return;
        }
        Assert.Fail("");
        
    
    }
    


    [Fact]
    public void sandbox()
    {

    }

}
