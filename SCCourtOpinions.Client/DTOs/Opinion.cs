using System;

namespace SCCourtOpinions.Client.DTOs
{
    public class Opinion
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Title}";
        }
    }
}