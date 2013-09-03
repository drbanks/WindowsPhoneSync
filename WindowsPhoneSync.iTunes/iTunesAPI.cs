using iTunesLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WindowsPhoneSync.iTunes
{
    /// <summary>
    /// Main iTunes API
    /// </summary>
    public class iTunesAPI
    {
        #region Static Fields

        /// <summary>
        /// Number of minutes since the library reference was used
        /// </summary>
        private static int idleTimer;

        /// <summary>
        /// Holds our iTunes library pointer
        /// </summary>
        private static iTunesApp library;

        private static object syncRoot = new object();

        #endregion

        #region Public Static Methods

        public static byte[] GetAlbumArtwork(IITFileOrCDTrack track)
        {
            if (track == null || track.Artwork == null || track.Artwork.Count <= 0)
                return null;

            string fileName = Path.Combine(Path.GetTempPath(), track.Name + ".image");
            try
            {
                foreach (IITArtwork image in track.Artwork)
                {
                    image.SaveArtworkToFile(fileName);
                    byte[] imageContents = File.ReadAllBytes(fileName);
                    return imageContents;
                }
                return null;
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Attempts to get a list of tracks for a specific album
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <returns></returns>
        public static IITFileOrCDTrack[] FindAlbumTracks(string artist, string album)
        {
            var library = GetLibrary();
            var allTracks = library.LibraryPlaylist;
            var tracks = allTracks.Search(album, ITPlaylistSearchField.ITPlaylistSearchFieldAlbums);
            if (tracks.Count == 0)
                return null;
            return (from IITTrack track in tracks
                    where string.Compare(artist, track.Artist, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                          string.Compare(album, track.Album, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                          track.Kind == ITTrackKind.ITTrackKindFile
                    orderby track.DiscNumber, track.TrackNumber
                    select track as IITFileOrCDTrack).ToArray();
        }

        /// <summary>
        /// Find a track given its name, artist name and album name
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="album"></param>
        /// <param name="trackName"></param>
        /// <returns></returns>
        public static IITFileOrCDTrack FindTrack(string artist, string album, string trackName, int? trackNumber = null)
        {
            var library = GetLibrary();
            var allTracks = library.LibraryPlaylist;
            var tracks = allTracks.Search(trackName, ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);
            if (tracks.Count == 0)
                return null;
            return (from IITTrack track in tracks
                    where string.Compare(artist, track.Artist, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                          string.Compare(album, track.Album, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                          (trackNumber.HasValue ? trackNumber.Value == track.TrackNumber : true) &&
                          track.Kind == ITTrackKind.ITTrackKindFile
                    select track as IITFileOrCDTrack).FirstOrDefault();
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Gets a reference to the iTunes library
        /// </summary>
        /// <returns></returns>
        private static iTunesApp GetLibrary()
        {
            lock (syncRoot)
            {
                if (library == null)
                {
                    library = new iTunesApp();
                    LibraryTimer(30);
                }
                idleTimer = 0;
                return library;
            }
        }

        /// <summary>
        /// Creates a timer to release the iTunes library after so many minutes of inactivity
        /// </summary>
        /// <param name="maximumMinutes"></param>
        private static void LibraryTimer(int maximumMinutes)
        {
            Timer timer = new Timer(60 * 1000.0) { AutoReset = true };
            timer.Elapsed += (sender, e) =>
                {
                    lock (syncRoot)
                    {
                        idleTimer++;
                        if (idleTimer >= maximumMinutes)
                        {
                            if (library != null)
                                library.Quit();
                            library = null;
                            timer.Stop();
                            timer.Dispose();
                            timer = null;
                        }
                    }
                };
            timer.Start();
        }

        #endregion
    }
}
