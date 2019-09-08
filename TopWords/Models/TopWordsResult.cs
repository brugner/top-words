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

        public TopWordsResult(string message, IDictionary<string, int> words)
        {
            Message = message;
            Words = words;
        }
    }
}
