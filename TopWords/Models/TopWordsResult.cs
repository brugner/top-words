using System.Collections.Generic;

namespace TopWords.Models
{
    /// <summary>
    /// A search's result.
    /// </summary>
    public class TopWordsResult
    {
        /// <summary>
        /// Search result's message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The artist used in the search.
        /// </summary>
        public Artist Artist { get; set; }

        /// <summary>
        /// A dictionary with the top words and its frequency.
        /// </summary>
        public IDictionary<string, int> Words { get; set; }

        public TopWordsResult()
        {
            Words = new Dictionary<string, int>();
        }

        public TopWordsResult(string message)
        {
            Message = message;
        }

        public TopWordsResult(Artist artist, string message, IDictionary<string, int> words)
        {
            Artist = artist;
            Message = message;
            Words = words;
        }
    }
}
