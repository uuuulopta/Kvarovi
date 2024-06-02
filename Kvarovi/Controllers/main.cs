namespace Kvarovi.Controllers;

using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Middleware.ApiKeyAuthentication;
using Models;
using Repository;
using Services;

[ApiController]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationOptions.DefaultScheme)]
[Route("Kvarovi")]
public class MainController : ControllerBase
{
    readonly IAnnouncementRepository _announcementRepository;
    readonly ILogger<MainController> _logger;
    readonly IServiceProvider _services;
    readonly IMemoryCache _memoryCache;
    
    public MainController(IAnnouncementRepository announcementRepository,ILogger<MainController> logger,
        IServiceProvider services,ApiKeyUserMemoryCache memoryCache)
    {
        _services = services;
        _memoryCache = memoryCache.Cache;
        _announcementRepository = announcementRepository;
        _logger = logger;
    }
    [HttpPost("/register")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> register( RegisterData data)
    {

        var device = data.device;
        var token = data.token;
        _logger.LogInformation($"Register: token: {token} device: {device}");
        
        if (device == "android")
        {
            if (!token.Contains("ExponentPushToken[") || token.Last() != ']') return BadRequest("Wrong token format for android.");
            var key = new ApiKeyService().GenerateApiKey();
            
            await _announcementRepository.registerUser(token, key);
            await _announcementRepository.saveChangesAsync();
            
            return Ok(key);
        }

        return UnprocessableEntity("Unknown device.");
    }

    [HttpPost("/editKeywords")]
    public async Task<ActionResult> editKeywords([FromForm]string? keywords)
    {
        List<string> kws;
        if (keywords != null)
        {
            kws = keywords.Split(",").ToList();
            kws = kws.Select(kw => kw.Trim().ToLower()).ToList();
    
        }
        else kws = new List<string>();
        _logger.LogInformation($"Received keywords: {keywords} Split: {string.Join(',',kws)}");
        var parsed = kws.Where(k => String.IsNullOrEmpty(k) == false).ToList();
        if (parsed.Count > 20) return BadRequest("You can't have more than 20 keywords.");
        if (parsed.Any(k => k.Length > 100)) return BadRequest("Keyword is too long");
        await _announcementRepository.EditKeywordsForUser(int.Parse(User.Claims.FirstOrDefault()!.Value),parsed);
        await _announcementRepository.saveChangesAsync();
        return Ok();
    }

    [HttpGet("/getData")]
    public async Task<ActionResult<IEnumerable<ClientData>>> getData()
    {
        var res = await _announcementRepository.GetAppDataForUser(int.Parse(User.Claims.FirstOrDefault()!.Value));
        if (!res.Any()) return Ok(new List<ClientData>());
        return Ok(res);
    }
    [HttpGet("/getKeywords")]
    public async Task<ActionResult<IEnumerable<ClientData>>> getKeywords()
    {
        var res = await _announcementRepository.getKeywordsByUserIdAsync(int.Parse(User.Claims.FirstOrDefault()!.Value));
        return Ok(res.Select(k => new {id=k.KeywordId,word=k.Word}));
    }
    

  
}
