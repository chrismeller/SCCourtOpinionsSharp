using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SCCourtOpinions.Client.DTOs;

namespace SCCourtOpinions.Client
{
    public abstract class SCCourtOpinions
    {
        protected abstract string BaseUrl { get; }
        protected string OpinionsBaseUrl = "http://www.sccourts.org";

        public async Task<List<Opinion>> GetOpinions(int? year = null, int? month = null)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            if (!month.HasValue)
            {
                month = DateTime.Now.Month;
            }

            using (var http = new HttpClient())
            {
                var url = $"{BaseUrl}?year={year}&month={month}";
                var content = await http.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                var headers = doc.DocumentNode.SelectNodes("//div//b[ contains( text(), '- O' )]");

                var opinions = new List<Opinion>();

                if (headers == null)
                {
                    return opinions;
                }

                for(var i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];

                    var date = header.InnerHtml.Substring(0, header.InnerHtml.IndexOf(" - "));
                    var description = header.InnerHtml.Substring(header.InnerHtml.IndexOf(" - ") + " - ".Length);

                    var dateDt = DateTime.Parse(date);

                    // is there another header?
                    HtmlNode nextHeader;
                    string xpathQuery = "";
                    if (i < headers.Count - 1)
                    {
                        nextHeader = headers[i + 1];
                    }
                    else
                    {
                        nextHeader = null;
                    }

                    if (nextHeader != null)
                    {
                        xpathQuery =
                            $"//a[ @class='blueLink2' and preceding::b[ contains( text(), '{header.InnerHtml}' ) ] and following::b[ contains( text(), '{nextHeader.InnerHtml}' ) ] ]";
                    }
                    else
                    {
                        xpathQuery =
                            $"//a[ @class='blueLink2' and preceding::b[ contains( text(), '{header.InnerHtml}' ) ] ]";
                    }

                    var links = doc.DocumentNode.SelectNodes(xpathQuery);
                    foreach (var link in links)
                    {
                        var id = link.InnerHtml.Substring(0, link.InnerHtml.IndexOf(" - "));
                        var title = link.InnerHtml.Substring(link.InnerHtml.IndexOf(" - ") + " - ".Length);

                        var href = link.Attributes["href"].Value;

                        var opinionDescription = link.SelectSingleNode("./following::blockquote");

                        var opinion = new Opinion()
                        {
                            Id = id,
                            Title = title,
                            Date = dateDt,
                            Type = description.ToLower().Trim('s'),
                            Url = $"{OpinionsBaseUrl}{href}",
                            Description = opinionDescription.InnerText.Trim()
                        };

                        opinions.Add(opinion);
                    }
                }

                return opinions;
            }
        }
    }
}