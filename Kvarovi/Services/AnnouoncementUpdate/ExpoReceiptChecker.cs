namespace Kvarovi.Services.AnnouoncementUpdate;

using ExpoCommunityNotificationServer.Client;
using ExpoCommunityNotificationServer.Models;
using Repository;

public class ExpoReceiptChecker: IHostedService
{

    
    readonly IServiceProvider _services;
    // so they don't get garbage collected
    // ReSharper disable once NotAccessedField.Local
    Timer? _timer;
    static List<(List<PushTicketStatus>,DateTime)> _expoTickets = new();
    List<Task<PushReceiptResponse>> receiptRequests = new();
    static object monitor = new object();
    static string _accessToken = WebApplication.CreateBuilder().Configuration.GetSection("expo")["accessToken"] ?? throw new Exception("Missing expo access token in appsettings.json or environment.");
    static IPushApiClient _client = new PushApiClient(token:_accessToken,settings:null);
    static int connections = 0;
    readonly ILogger<AnnouncementUpdateService> _logger;
    readonly IAnnouncementRepository _repository;

    public ExpoReceiptChecker(IServiceProvider services)
    {
        _services = services;
        var scope = _services.CreateScope();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<AnnouncementUpdateService>>();
        _repository = scope.ServiceProvider.GetRequiredService<IAnnouncementRepository>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        _timer = new Timer(sendReceiptsAndValidate,
            null,
            0,
            6000);
        return Task.CompletedTask;
    }



    public async void sendReceiptsAndValidate(object? obj)
    {
        // https://docs.expo.dev/push-notifications/sending-notifications/

        var tickets = new List<(List<PushTicketStatus>, DateTime)>();
        
        lock (monitor)
        {
            // tickets = _expoTickets.Where(t => DateTime.Now.Subtract(t.Item2).Minutes >= 15).ToList();
            // _expoTickets.RemoveAll(t => DateTime.Now.Subtract(t.Item2).Minutes >= 15);
            tickets = _expoTickets.ToList();
            _expoTickets.Clear();
            
        }

        _logger.LogInformation($"Getting reciepts for {tickets.Count} tickets");
        while (tickets.Count != 0)
        {
            var req = new PushReceiptRequest()
            {
                PushTicketIds = tickets.Take(900).SelectMany(t => t.Item1.Select(i => i.TicketId)).ToList()
            };
            
            tickets = tickets.Skip(900).ToList();
            while (connections > 2)
            {
                await Task.Delay(2000);
            }
            Task justDoingThisToRemoveError = _client.GetReceiptsAsync(req).ContinueWith(
                res => validateReceipts(res.Result));
            connections += 1;
        }
        
    }

    private async void validateReceipts(PushReceiptResponse resp)
    {
        connections -= 1;
        foreach (var keyValuePair in resp.PushTicketReceipts)
        {
            var ticketId = keyValuePair.Key;
            _logger.LogInformation($"Validating receipt for {ticketId}");
            var ticketDeliveryStatus = keyValuePair.Value;
            _logger.LogInformation($"Status {ticketDeliveryStatus.DeliveryStatus}");
            string expoToken = ""; 
            if (UserNotifier.TicketExpoTokenCache.TryGetValue(ticketId,
                    out var token))
            {
               expoToken = (string) token!;
            }
            if (ticketDeliveryStatus.DeliveryStatus == "error")
            {
                
                if (ticketDeliveryStatus.Details.Error == "DeviceNotRegistered")
                {

                    if (!string.IsNullOrEmpty(expoToken))
                    {
                        _logger.LogWarning($"DeviceNotRegistered for token {expoToken}, removing it..");
                        await _repository.deleteDeviceByToken((string) token!);
                        await _repository.saveChangesAsync();
                    }
                    else  _logger.LogError("DeviceNotRegistered: Unable to find ExpoPushToken for a given ExpoPushTicket");
                    


                }

                if (ticketDeliveryStatus.Details.Error == "MessageRateExceeded")
                { 
                    _logger.LogError("MessageRateExceeded error for Receipt");
                    lock(monitor) _expoTickets.Add(
                        (new List<PushTicketStatus>(){ new PushTicketStatus(){ TicketId = ticketId }}, DateTime.Now) ); 
                }
                    
                    
            }
            UserNotifier.TicketExpoTokenCache.Remove(expoToken);
        }

        await _repository.saveChangesAsync();

    }
    public static void addTickets(List<PushTicketStatus> tickets)
    {
        lock (monitor)
        {
            _expoTickets.AddRange(
                new List<( List<PushTicketStatus>, DateTime )>()
                {
                    (tickets.Where(t => t.TicketStatus == "ok").ToList(),DateTime.Now)
                }
            );

        } 
    }
    

    
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
