namespace NowPlaying.Model
{
    /// <summary>
    /// Contains information about the Now Playing artist, song and album image.
    /// </summary>
    public class NowPlayingInfo
    {
        #region Properties

        /// <summary>
        /// Gets the name of the artist for the current song playing.
        /// </summary>
        public string Artist { get; }

        /// <summary>
        /// Gets the song title for the current song playing.
        /// </summary>
        public string Song { get; }

        /// <summary>
        /// Gets the URL to the album image.
        /// </summary>
        public string ImageUrl { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="artist">The name of the artist for the current song playing.</param>
        /// <param name="song">The song title for the current song playing.</param>
        /// <param name="imageUrl">The URL to the album image.</param>
        public NowPlayingInfo(string artist, string song, string imageUrl)
        {
            Artist = artist;
            Song = song;
            ImageUrl = imageUrl;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compare the value equality of this <see cref="NowPlayingInfo"/> and another.
        /// </summary>
        /// <param name="other">The other <see cref="NowPlayingInfo"/> to compare.</param>
        /// <returns>True if both this and the other <see cref="NowPlayingInfo"/> have the same values.</returns>
        public bool IsEquivalentTo(NowPlayingInfo other)
        {
            return Artist == other?.Artist
                && Song == other?.Song
                && ImageUrl == other?.ImageUrl;
        }

        #endregion
    }
}
