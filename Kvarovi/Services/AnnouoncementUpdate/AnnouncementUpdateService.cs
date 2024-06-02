namespace Kvarovi.Services;

using AnnouncementGetters;
using AnnouoncementUpdate;
using EntityConfigs;

public class AnnouncementUpdateService : IHostedService
{


    readonly IServiceProvider _services;

    // so they don't get garbage collected
    // ReSharper disable once NotAccessedField.Local
    Timer? _timer;

    // ReSharper disable once NotAccessedField.Local
    Timer? _timerUpdate;
    readonly ILogger<AnnouncementUpdateService> _logger;
    AnnouncementGetterFactory _announcementGetterFactory;

    public AnnouncementUpdateService(IServiceProvider services, AnnouncementGetterFactory announcementGetterFactory)
    {
        _services = services;
        _announcementGetterFactory = announcementGetterFactory;
        var scope = _services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<AnnouncementUpdateService>>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {

        _timer = new Timer(update,
            null,
            0,
            600000);
        return Task.CompletedTask;
    }


    async void update(object? obj)
    {

        var eps = _announcementGetterFactory.getAnnouncements(AnnouncementUrl.EpsAllDays);
        await new Updater(_services,_announcementGetterFactory).Update(eps,
            AnnouncementTypeEnum.eps);
        var vodovodPlanned = _announcementGetterFactory.getAnnouncements(AnnouncementUrl.vodovodplanned);
        await new Updater(_services,_announcementGetterFactory).Update(vodovodPlanned,
            AnnouncementTypeEnum.vodovod);
        var vodovodCurrent = _announcementGetterFactory.getAnnouncements(AnnouncementUrl.vodovodkvar);
        await new Updater(_services,_announcementGetterFactory).Update(vodovodCurrent,
            AnnouncementTypeEnum.vodovod);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}