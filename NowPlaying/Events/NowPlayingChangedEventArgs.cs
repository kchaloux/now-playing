using System;
using NowPlaying.Model;

namespace NowPlaying.Events
{
    /// <summary>
    /// Event arguments for when the Now Playing information has changed.
    /// </summary>
    public class NowPlayingChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the old <see cref="NowPlayingInfo"/>.
        /// </summary>
        public NowPlayingInfo OldInfo { get; }

        /// <summary>
        /// Gets the new <see cref="NowPlayingInfo"/>.
        /// </summary>
        public NowPlayingInfo NewInfo { get; }

        /// <summary>
        /// Gets whether or not the artist changed between the old and new info.
        /// </summary>
        public bool ArtistChanged { get; }

        /// <summary>
        /// Gets whether or not the song changed between the old and new info.
        /// </summary>
        public bool SongChanged { get; }

        /// <summary>
        /// Gets whether or not the image URL changed between the old and new info.
        /// </summary>
        public bool ImageChanged { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldInfo">The old <see cref="NowPlayingInfo"/></param>
        /// <param name="newInfo">The new <see cref="NowPlayingInfo"/></param>
        public NowPlayingChangedEventArgs(NowPlayingInfo oldInfo, NowPlayingInfo newInfo)
        {
            OldInfo = oldInfo;
            NewInfo = newInfo;
            ArtistChanged = oldInfo?.Artist != newInfo?.Artist;
            SongChanged = oldInfo?.Song != newInfo?.Song;
            ImageChanged = oldInfo?.ImageUrl != newInfo?.ImageUrl;
        }
    }
}
