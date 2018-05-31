using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SCCourtOpinions.Atom
{
    public class AtomFeedGenerator
    {
        public static XmlDocument GenerateDocument(AtomFeed f)
        {
            var doc = new XmlDocument();

            // create the root feed node
            var feed = doc.CreateElement("feed", "http://www.w3.org/2005/Atom");

            // create the title node
            var titleNode = doc.CreateTextNode(f.Title);
            var title = doc.CreateElement("title", feed.NamespaceURI);
            title.AppendChild(titleNode);

            // append the title to the feed
            feed.AppendChild(title);

            // create the link node
            var link = doc.CreateElement("link", feed.NamespaceURI);
            link.SetAttribute("href", f.Link.ToString());

            // add it to the feed node
            feed.AppendChild(link);

            if (f.LinkSelf != null)
            {
                // create the required "self" link node
                var selfLink = doc.CreateElement("link", feed.NamespaceURI);
                selfLink.SetAttribute("href", f.LinkSelf.ToString());
                selfLink.SetAttribute("rel", "self");

                // append it to the feed
                feed.AppendChild(selfLink);
            }

            // figure out the last updated date - it should be the date of the last item in the list. if there aren't any, it's just today
            var lastUpdated = DateTimeOffset.UtcNow;
            if (f.LastUpdated.HasValue)
            {
                lastUpdated = f.LastUpdated.Value;
            }
            else if (f.Items.Any())
            {
                lastUpdated = f.Items.OrderByDescending(x => x.LastUpdated).First().LastUpdated;
            }

            var updatedNode =
                doc.CreateTextNode(XmlConvert.ToString(lastUpdated.DateTime, XmlDateTimeSerializationMode.Utc));
            var updated = doc.CreateElement("updated", feed.NamespaceURI);
            updated.AppendChild(updatedNode);

            // append it to the feed
            feed.AppendChild(updated);

            var idNode = doc.CreateTextNode(String.Format("urn:uuid:{0}", f.Id.ToString()));
            var id = doc.CreateElement("id", feed.NamespaceURI);
            id.AppendChild(idNode);

            // append it to the feed
            feed.AppendChild(id);

            foreach (var item in f.Items)
            {
                var entry = doc.CreateElement("entry", feed.NamespaceURI);

                var entryTitleNode = doc.CreateTextNode(item.Title);
                var entryTitle = doc.CreateElement("title", feed.NamespaceURI);
                entryTitle.AppendChild(entryTitleNode);

                // append it to the entry
                entry.AppendChild(entryTitle);

                var entryLink = doc.CreateElement("link", feed.NamespaceURI);
                entryLink.SetAttribute("href", item.Link.ToString());

                // append it to the entry
                entry.AppendChild(entryLink);

                var entryIdNode = doc.CreateTextNode(String.Format("urn:uuid:{0}", item.Id.ToString()));
                var entryId = doc.CreateElement("id", feed.NamespaceURI);
                entryId.AppendChild(entryIdNode);

                // append it to the entry
                entry.AppendChild(entryId);

                var entryUpdatedNode =
                    doc.CreateTextNode(XmlConvert.ToString(item.LastUpdated.DateTime, XmlDateTimeSerializationMode.Utc));
                var entryUpdated = doc.CreateElement("updated", feed.NamespaceURI);
                entryUpdated.AppendChild(entryUpdatedNode);

                // append it to teh entry
                entry.AppendChild(entryUpdated);

                var entrySummaryNode = doc.CreateTextNode(item.Summary);
                var entrySummary = doc.CreateElement("summary", feed.NamespaceURI);
                entrySummary.AppendChild(entrySummaryNode);
                entrySummary.SetAttribute("type", "html");

                // append it to the entry
                entry.AppendChild(entrySummary);

                var entryAuthorNameNode = doc.CreateTextNode(item.AuthorName);
                var entryAuthorName = doc.CreateElement("name", feed.NamespaceURI);
                entryAuthorName.AppendChild(entryAuthorNameNode);

                var entryAuthor = doc.CreateElement("author", feed.NamespaceURI);
                entryAuthor.AppendChild(entryAuthorName);

                // append it to the entry
                entry.AppendChild(entryAuthor);

                if (String.IsNullOrWhiteSpace(item.Category) == false)
                {
                    var entryCategory = doc.CreateElement("category", feed.NamespaceURI);
                    entryCategory.SetAttribute("term", item.Category);

                    // append it to the entry
                    entry.AppendChild(entryCategory);
                }

                // finally, append the entry itself to the feed
                feed.AppendChild(entry);
            }

            // last, but most importantly, add the feed we've built to the base document
            doc.AppendChild(feed);

            return doc;
        }

        public static string GenerateFeed(AtomFeed f)
        {
            var doc = GenerateDocument(f);

            using (var stringWriter = new StringWriter())
            using (var writer = new XmlTextWriter(stringWriter))
            {
                doc.WriteTo(writer);

                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}