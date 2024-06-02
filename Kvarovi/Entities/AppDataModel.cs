namespace Kvarovi.Entities;

using AnnouncementGetters.CurrentOrPlannetStrategies;

public class ClientData
{
    public int Id;
    public string Title;
    public string Text;
    public string Date;
    public string WorkType;
    public string AnnouncementType;


}
public class AppDataModel
{
    /// <summary>
    /// Example: {Eps} : { Current : { ClientData[]} }
  /// </summary>
    Dictionary<string, Dictionary<string, ClientData[]>> data = new();
}
