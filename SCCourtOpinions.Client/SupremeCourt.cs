namespace SCCourtOpinions.Client
{
    public class SupremeCourt : SCCourtOpinions
    {
        protected override string BaseUrl => "http://www.sccourts.org/opinions/indexSCPub.cfm";

        public SupremeCourt(string proxyHost = null, int proxyPort = 80, string proxyUser = null,
            string proxyPass = null) : base(proxyHost, proxyPort, proxyUser, proxyPass)
        {
        }
    }
}