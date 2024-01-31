namespace Kvarovi.Services;

public class AnnouncementUpdateService: IHostedService
{

    
    readonly IServiceProvider _services;
    // so they don't get garbage collected
    // ReSharper disable once NotAccessedField.Local
    Timer? _timer;
    // ReSharper disable once NotAccessedField.Local
    Timer? _timerUpdate;
    readonly ILogger<AnnouncementUpdateService> _logger;

    public AnnouncementUpdateService(IServiceProvider services)
    {
        _services = services;
        var scope = _services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<AnnouncementUpdateService>>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        // _timer = new Timer(,
        //     null,
        //     0,
        //     60000);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
