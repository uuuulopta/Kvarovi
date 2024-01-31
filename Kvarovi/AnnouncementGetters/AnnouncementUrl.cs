namespace Kvarovi.AnnouncementGetters;

public class AnnouncementUrl
{
    
    private AnnouncementUrl(string value) { Value = value; }

    public  string Value { get; private set; }
    public static AnnouncementUrl vodovodplanned  { get { return new AnnouncementUrl("https://www.bvk.rs/planirani-radovi"); } }
    public static AnnouncementUrl vodovodkvar  { get { return new AnnouncementUrl("https://www.bvk.rs/kvarovi-na-mrezi"); } }
    public static AnnouncementUrl EpsAllDays  { get { return new AnnouncementUrl("EPSALLDAYS"); } }
    public static AnnouncementUrl EpsToday  { get { return new AnnouncementUrl("https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_0_Iskljucenja.htm"); } }
    public static AnnouncementUrl EpsTommorow  { get { return new AnnouncementUrl("https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_1_Iskljucenja.htm"); } }
    public static AnnouncementUrl Eps2days  { get { return new AnnouncementUrl("https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_2_Iskljucenja.htm"); } }
    public static AnnouncementUrl Eps3days  { get { return new AnnouncementUrl("https://elektrodistribucija.rs/planirana-iskljucenja-beograd/Dan_3_Iskljucenja.htm"); } }
    public override string ToString()
    {
        return Value;
    }
    

}
