namespace TopWords.Models
{
    /// <summary>
    /// Represents an artist.
    /// </summary>
    public class Artist
    {
        /// <summary>
        /// Artist id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the artist.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initialize a new artist.
        /// </summary>
        public Artist()
        {

        }

        /// <summary>
        /// Initialize a new artist.
        /// </summary>
        /// <param name="id">Artist id.</param>
        /// <param name="name">Artist name.</param>
        public Artist(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
