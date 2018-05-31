using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SCCourtOpinions.Atom;
using SCCourtOpinions.Client;

namespace SCCourtOpinions.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAppeals().ConfigureAwait(false).GetAwaiter().GetResult();
            RunSupreme().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task RunAppeals()
        {
            var client = new AppealsCourt();
            var opinions = await client.GetOpinions();

            Console.WriteLine("Got {0} opinions for Appeals.", opinions.Count);

            var items = opinions.Select(x => new AtomFeedItem()
            {
                Id = Guid.Parse(x.Type == "order" ? Md5(x.Url) : Md5(x.Id)),
                Link = new Uri(x.Url),
                Summary = x.Description,
                Title = x.Title,
                LastUpdated = x.Date,
                AuthorName = "Appeals Court of South Carolina",
                Category = x.Type,
            }).ToList();

            var f = new AtomFeed()
            {
                AuthorName = "Appeals Court of South Carolina",
                Id = Guid.Parse("9b0ef8e6-c5f5-b370-c165-0ed110eab7e3"),
                LastUpdated = DateTimeOffset.UtcNow,
                Link = new Uri("http://www.sccourts.org/opinions/indexCOAPub.cfm"),
                //LinkSelf = new Uri("http://www.sccourts.org/opinions/indexCOAPub.cfm"),
                Title = "South Carolina Appeals Court Published Opinions",
                Items = items
            };

            var feed = AtomFeedGenerator.GenerateFeed(f);

            await UploadFile("appeals.xml", feed, "text/xml");
        }

        public static async Task RunSupreme()
        {
            var client = new SupremeCourt();
            var opinions = await client.GetOpinions();

            Console.WriteLine("Got {0} opinions for Supreme.", opinions.Count);

            var items = opinions.Select(x => new AtomFeedItem()
            {
                Id = Guid.Parse(x.Type == "order" ? Md5(x.Url) : Md5(x.Id)),
                Link = new Uri(x.Url),
                Summary = x.Description,
                Title = x.Title,
                LastUpdated = x.Date,
                AuthorName = "Supreme Court of South Carolina",
                Category = x.Type,
            }).ToList();

            var f = new AtomFeed()
            {
                AuthorName = "Supreme Court of South Carolina",
                Id = Guid.Parse("062c1964-e9f2-ca37-8c51-6f4ed3e0cee1"),
                LastUpdated = DateTimeOffset.UtcNow,
                Link = new Uri("http://www.sccourts.org/opinions/indexCOAPub.cfm"),
                //LinkSelf = new Uri("http://www.sccourts.org/opinions/indexCOAPub.cfm"),
                Title = "South Carolina Supreme Court Published Opinions",
                Items = items
            };

            var feed = AtomFeedGenerator.GenerateFeed(f);

            await UploadFile("supreme.xml", feed, "text/xml");
        }

        private static async Task UploadFile(string filename, string contents, string contentType)
        {
            var storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference("opinions");

            var blob = blobContainer.GetBlockBlobReference(filename);

            await blob.UploadTextAsync(contents);

            if (blob.Properties.ContentType != contentType)
            {
                blob.Properties.ContentType = contentType;

                await blob.SetPropertiesAsync();
            }
        }

        private static string Md5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(bytes);

                var bc = BitConverter.ToString(hashBytes);

                // lowercase it and get a standardized 8-4-4-4-12 grouping
                // in opposite order so we don't have to offset for the -'s we've already added
                bc = bc.ToLower().Replace("-", "").Insert(20, "-").Insert(16, "-").Insert(12, "-").Insert(8, "-");

                return bc;
            }
        }
    }
}
