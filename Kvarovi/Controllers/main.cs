namespace gspAPI.Controllers;

using Kvarovi.AnnouncementGetters;
using Kvarovi.Entities;
using Kvarovi.EntityConfigs;
using Kvarovi.Repository;
using Kvarovi.Services.AnnouoncementUpdate;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


[ApiController]
[Route("Kvarovi")]
public class MainController : ControllerBase
{
    readonly IAnnouncementRepository _announcementRepository;
    readonly ILogger<MainController> _logger;
    readonly IServiceProvider _services;

    public MainController(IAnnouncementRepository announcementRepository,ILogger<MainController> logger,IServiceProvider services)
    {
        _services = services;
        _announcementRepository = announcementRepository;
        _logger = logger;
    }
    [HttpGet("/test")]
    public async Task<ActionResult> test()
    {
        var updater = new Updater(_services);
        var data = AnnouncementGetterFactory.getAnnouncements(AnnouncementUrl.EpsAllDays);
        await updater.Update(data,AnnouncementTypeEnum.eps);
        return Ok();

    }

    [HttpPost("/register")]
    public async Task<ActionResult> register([FromForm]string token,[FromForm]string device)
    {
        if (device == "android")
        {
            if (!token.Contains("ExponentPushToken[") || token.Last() != ']') return BadRequest("Wrong token format for android.");
            await _announcementRepository.registerUser(token);
            await _announcementRepository.saveChangesAsync();
            return Ok();
        }
        else return UnprocessableEntity("Unknown device.");
    }

    [HttpPost("/editKeywords")]
    public async Task<ActionResult> editKeywords([FromForm]string token, [FromForm]string keywords)
    {
        var kws = keywords.Split(",");
        var parsed = kws.Where(k => String.IsNullOrEmpty(k) == false).ToList();
        if (parsed.Count == 0) return BadRequest("Count of keywords is 0");
        if (parsed.Count > 20) return BadRequest("You can't have more than 20 keywords.");
        if (parsed.Any(k => k.Length > 100)) return BadRequest("Keyword is too long");
        await _announcementRepository.EditKeywordsForUser(token,parsed);
        await _announcementRepository.saveChangesAsync();
        return Ok();
    }

  
}
