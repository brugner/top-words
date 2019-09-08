namespace TopWords.Models
{
    /// <summary>
    /// Represents an artist.
    /// </summary>
    public class Artist
    {
        /// <summary>
        /// Artist Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The name of the artist.
        /// </summary>
        public string Name { get; set; }

        public Artist()
        {

        }

        public Artist(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
