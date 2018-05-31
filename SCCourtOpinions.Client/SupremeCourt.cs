namespace SCCourtOpinions.Client
{
    public class SupremeCourt : SCCourtOpinions
    {
        protected override string BaseUrl => "http://www.sccourts.org/opinions/indexSCPub.cfm";
    }
}