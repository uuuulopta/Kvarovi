namespace Kvarovi.AnnouncementGetters.CurrentOrPlannetStrategies;

using Entities;

/// <summary>
/// Determines if the current announcement is 
/// </summary>
public enum AnnouncementWorkType
{
   Current,
   Planned
}
public  interface IWorkTypeStrategy
{
   public AnnouncementWorkType determineWorkType(Announcement a);

}
