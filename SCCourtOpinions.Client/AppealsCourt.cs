namespace SCCourtOpinions.Client
{
    public class AppealsCourt : SCCourtOpinions
    {
        protected override string BaseUrl => "http://www.sccourts.org/opinions/indexCOAPub.cfm";

        public AppealsCourt(string proxyHost = null, int proxyPort = 80, string proxyUser = null,
            string proxyPass = null) : base(proxyHost, proxyPort, proxyUser, proxyPass)
        {
        }
    }
}