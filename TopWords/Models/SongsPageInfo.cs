using System.Collections.Generic;

namespace TopWords.Models
{
    public class SongsPageInfo
    {
        public string ArtistName { get; set; }
        public List<string> SongsUrls { get; set; } = new List<string>();
        public static SongsPageInfo Invalid => new SongsPageInfo("Invalid url");

        public SongsPageInfo(string artistName)
        {
            ArtistName = artistName;
        }
    }
}
