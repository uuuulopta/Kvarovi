namespace Kvarovi.Services.AnnouoncementUpdate;

using ExpoCommunityNotificationServer.Models;

public class ObjectWithExpoPushToken<T>
{
    T Object { get; }
    string Token { get; }

    public ObjectWithExpoPushToken(T obj, string token)
    {
        Object = obj;
        Token = token;
    }
}
