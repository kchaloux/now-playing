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
        /// Constructor.
        /// </summary>
        /// <param name="oldInfo">The old <see cref="NowPlayingInfo"/></param>
        /// <param name="newInfo">The new <see cref="NowPlayingInfo"/></param>
        public NowPlayingChangedEventArgs(NowPlayingInfo oldInfo, NowPlayingInfo newInfo)
        {
            OldInfo = oldInfo;
            NewInfo = newInfo;
        }
    }
}
