using Abot2.Core;
using Abot2.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TopWords.Services.Interfaces;

namespace TopWords.Services
{
    public class CrawlerService : ICrawlerService
    {
        private readonly PageRequester _pageRequester = new PageRequester(_crawlConfiguration, new WebContentExtractor());

        private static readonly CrawlConfiguration _crawlConfiguration = new CrawlConfiguration
        {
            MinCrawlDelayPerDomainMilliSeconds = 3000,
            IsRespectRobotsDotTextEnabled = true,
            IsRespectMetaRobotsNoFollowEnabled = true,
            IsRespectHttpXRobotsTagHeaderNoFollowEnabled = true,
            IsRespectAnchorRelNoFollowEnabled = true,
            MaxCrawlDepth = 0
        };

        public async Task<List<string>> GetSongsUrls(long artistId)
        {
            var url = "https://songmeanings.com/artist/view/songs/" + artistId;

            if (!IsValidUrl(url))
            {
                return new List<string>();
            }

            var crawledPage = await _pageRequester.MakeRequestAsync(new Uri(url));
            var htmlLinks = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll("#songslist tr td:first-child a");
            var urls = new List<string>();

            foreach (var link in htmlLinks)
            {
                urls.Add($"https:{link.GetAttribute("href")}");
            }

            return urls;
        }

        public async Task<string> GetSongLyrics(string songUrl)
        {
            var crawledPage = await _pageRequester.MakeRequestAsync(new Uri(songUrl));
            var lyricsBox = crawledPage.AngleSharpHtmlDocument.Body.QuerySelector("div.holder.lyric-box");

            string songLyrics = string.Empty;

            foreach (var item in lyricsBox.ChildNodes.Where(x => x.NodeType == AngleSharp.Dom.NodeType.Text))
            {
                songLyrics += item.TextContent;
            }

            songLyrics = songLyrics.Trim().ToLower();
            Regex rgx = new Regex("[^a-zA-Z0-9 ']");

            return rgx.Replace(songLyrics, "");
        }

        private bool IsValidUrl(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
