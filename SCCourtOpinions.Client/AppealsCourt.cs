namespace SCCourtOpinions.Client
{
    public class AppealsCourt : SCCourtOpinions
    {
        protected override string BaseUrl => "http://www.sccourts.org/opinions/indexCOAPub.cfm";
    }
}