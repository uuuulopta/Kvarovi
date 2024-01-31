namespace Kvarovi.Utils;

using System.Runtime.CompilerServices;
using Entities;
using Repository;

public static class AnnouncementExtensions
{
    public static bool isTimestampYoungerThan(this Announcement a, Announcement b)
    {
        if (a.Timestamp > b.Timestamp) return true;
        return false;
    }
}
