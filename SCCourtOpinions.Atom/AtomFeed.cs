using System;
using System.Collections.Generic;

namespace SCCourtOpinions.Atom
{
    public class AtomFeed
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public Uri LinkSelf { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        public string AuthorName { get; set; }
        public Guid Id { get; set; }

        public List<AtomFeedItem> Items { get; set; }
    }

    public class AtomFeedItem
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string Summary { get; set; }
        public string AuthorName { get; set; }
        public string Category { get; set; }
    }
}