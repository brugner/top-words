namespace TopWords.Models
{
    /// <summary>
    /// Represents the artist subjected to the analysis.
    /// </summary>
    public class TopWordsParams
    {
        /// <summary>
        /// Artist Id.
        /// </summary>
        public long ArtistId { get; set; }

        /// <summary>
        /// SignalR client connection Id.
        /// </summary>
        public string ConnectionId { get; set; }
    }
}
