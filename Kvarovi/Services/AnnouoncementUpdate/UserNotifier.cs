namespace Kvarovi.Services.AnnouoncementUpdate;

using EntityConfigs;
using ExpoCommunityNotificationServer.Client;
using ExpoCommunityNotificationServer.Models;
using Entities;
using Repository;
using Utils;

public class UserNotifier
{
    readonly IAnnouncementRepository _repository;
    readonly ILogger<Updater> _logger;
    List<string> _userTokens = new();
    static string _accessToken = WebApplication.CreateBuilder().Configuration.GetSection("expo")["accessToken"] 
                                 ?? throw new Exception("Missing expo access token in appsettings.json or environment.");
    static IPushApiClient _client = new PushApiClient(token:_accessToken,settings:null);
    public static readonly object monitor = new object();
    IServiceProvider _provider;
    Announcement _announcement;
    public UserNotifier(IServiceProvider provider,Announcement announcement)
    {
        this._provider = provider;
        var scope = provider.CreateScope();
        this._repository = scope.ServiceProvider.GetRequiredService<IAnnouncementRepository>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<Updater>>();
        _announcement = announcement ?? throw new NullReferenceException();

    }
    public async Task sendUserNotificationsForAnnouncement()
    {
        
        _repository.attachEntityRange(_announcement.Users.Cast<object>().ToArray());
        IEnumerable<User> users; 
        if (!_announcement.Update)
        {
            users = _announcement.Users;
            _logger.LogInformation($"Announcement {_announcement.Title} is not an update");
            foreach (var user in _announcement.Users)
            {
                _userTokens.Add(user.DeviceToken);
            }
        }
        else
        {
            _logger.LogInformation($"Announcement {_announcement.Title} is an update / same");
            
            var sameAnnouncement = await _repository.getAnnouncementByTitle(_announcement.Title);
            if (sameAnnouncement == null) throw new Exception("Failed to find root announcement");
            sameAnnouncement.Users = _announcement.Users;
            await _repository.saveChangesAsync();
            _announcement = sameAnnouncement;
            users = await _repository.getUsersNotNotified(_announcement);
            foreach (var user in users)
            {
                _userTokens.Add(user.DeviceToken);
            }
        }
        
        await _repository.setUsersAsNotifiedForAnnouncement(users, _announcement);
         
        await _repository.saveChangesAsync();
        await sendNotifications();

    }

    public async Task sendNotifications()
    {
        List<Task<PushTicketResponse>> tasks = new(); 
        lock(monitor)
        {
            int notificationsSent = 0;
            while (_userTokens.Count != 0)
            {
                // Expo will get mad if we send over 600 requests per second 
                if (notificationsSent >= 500)
                {
                    Thread.Sleep(1500);
                    notificationsSent = 0;
                }

                // Expo will also get mad if we send more than 100 tickets in one request, so we do 50 to be safe.
                var deviceTokens = get50orLessUserTokens();
                var ticket = createPushTicket(deviceTokens);

                // Exponential backoff if the tickets fail to be sent
                tasks.Add(Retry.DoAsync<PushTicketResponse>( () => _client.SendPushAsync(ticket), _ => true));
                
                notificationsSent += deviceTokens.Count;
            }
        }

        var res = (await Task.WhenAll(tasks));
        foreach (var pushTicketResponse in res)
        {
            var errored = pushTicketResponse.PushTicketStatuses.Where(t => t.TicketStatus == "error").ToList();
            foreach (var ticket in errored)
            {
                if (ticket.Details.Error == "DeviceNotRegistered")
                {
                    var token = ExpoReceiptChecker.GetTokenFromMessage(ticket.TicketMessage);
                    await _repository.deleteDeviceByToken(token);
                    await _repository.saveChangesAsync();
                    _logger.LogInformation($"DeviceNotRegistered TICKET, deleted: {token} ");
                }
                else
                {
                    _logger.LogCritical($"UNKNOWN TICKET ERROR: {ticket.Details.Error} {ticket.TicketMessage} ");
                }
            }
            ExpoReceiptChecker.addTickets(pushTicketResponse.PushTicketStatuses); 
        }

    }

    private PushTicketRequest createPushTicket(List<string> users)
    {
        
        return  new PushTicketRequest()
        {
            PushTo = users,
            PushTitle = _announcement.Title,
            PushBody = $"Imate obaveštenje o kvaru za {Enum.GetName(typeof(AnnouncementTypeEnum),_announcement.AnnouncementTypeId)}!",
        };
    }

    private List<string> get50orLessUserTokens()
    {
        List<string> res = new();
        res.AddRange(_userTokens.Take(50));
        _userTokens = _userTokens.Skip(res.Count).ToList();
        return res;
    }
}
