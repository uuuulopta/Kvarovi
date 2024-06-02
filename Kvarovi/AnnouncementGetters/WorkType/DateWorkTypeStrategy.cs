namespace Kvarovi.AnnouncementGetters.CurrentOrPlannetStrategies;

using Entities;

public class DateWorkTypeStrategy : IWorkTypeStrategy
{
    /// <summary>
    /// Determines the work type of announcement
    /// </summary>
    /// <param name="a"></param>
    /// <returns>If the day in the date matches today, returns current, else return planned</returns>
    public AnnouncementWorkType determineWorkType(Announcement a)
    {
        if (a.Date != null && a.Date.Value.Month == DateTime.Now.Month && a.Date.Value.Day == DateTime.Now.Day) return AnnouncementWorkType.Current;
        else return AnnouncementWorkType.Planned;
    }
}
