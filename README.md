About
=====
The South Carolina Judicial Department publishes all opinions issued by the Appeals and Supreme Courts of South Carolina on their website as PDFs, but there is no convenient way to follow those updates.

In the spirit of the growing trend, I decided to fix that by page-scraping the data and pushing it to a Twitter account. [@SCCourtOpinions](http://twitter.com/SCCourtOpinions) was born!

How
---
Grab the contents, parse it with [HtmlAgilityPack](http://html-agility-pack.net/), generate a rough [Atom](http://en.wikipedia.org/wiki/Atom_(standard)) feed, and upload the feed to [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/). You can, of course, just get the opinions and spit them out any way you like.

Instructions
------------
Be sure to replace the `StorageConnectionString` value in SCCourtOpinions.Host/App.config with your Azure Storage connection string.