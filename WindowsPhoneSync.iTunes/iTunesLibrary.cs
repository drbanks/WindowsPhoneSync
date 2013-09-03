using iTunesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindowsPhoneSync.DataContracts;

namespace WindowsPhoneSync.iTunes
{
    public class iTunesLibrary : ILibrary, INotifyPropertyChanged
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region ILibrary Members

        /// <summary>
        /// Get the tracks associated with an artist/album name pair
        /// </summary>
        /// <param name="artist">The artist name</param>
        /// <param name="album">The album name</param>
        /// <returns></returns>
        public SongMetadata[] FindAlbumTracks(string artist, string album)
        {
            var tracks = iTunesAPI.FindAlbumTracks(artist, album);
            return tracks.Select(track => SongMetadataFactory(track)).ToArray();
        }

        /// <summary>
        /// Find a specific song given the artist, album and track name; optionally track number
        /// </summary>
        /// <param name="artist">The artist name</param>
        /// <param name="album">The album name</param>
        /// <param name="trackName">The song's name</param>
        /// <param name="trackNumber">Optional track number</param>
        /// <returns>Generic Song data</returns>
        public SongMetadata FindTrack(string artist, string album, string trackName, int? trackNumber = null)
        {
            return SongMetadataFactory(iTunesAPI.FindTrack(artist, album, trackName, trackNumber));
        }

        public DataContracts.Artwork GetArtwork(DataContracts.SongMetadata song)
        {
            throw new NotImplementedException();
        }

        public DataContracts.Artwork GetArtwork(DataContracts.AlbumMetadata album)
        {
            throw new NotImplementedException();
        }

        public DataContracts.Artwork GetArtwork(DataContracts.ArtistMetadata artist)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generic method to set a property value, raising the PropertyChanged event as we go
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="newValue">The new value to set</param>
        /// <param name="oldValue">Reference to the value storage</param>
        /// <param name="propertyName">The name of the property being changed</param>
        protected void SetValue<t>(t newValue, ref t oldValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(newValue, oldValue))
                return;

            oldValue = newValue;
            if (!string.IsNullOrWhiteSpace(propertyName))
                OnPropertyChanged(propertyName);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new SongMetadata from the track information
        /// </summary>
        /// <param name="song">The iTunes track information</param>
        /// <returns>New Song</returns>
        private static SongMetadata SongMetadataFactory(IITFileOrCDTrack song)
        {
            var ret = new SongMetadata
            {
                AlbumArtist = song.AlbumArtist,
                AlbumName = song.Album,
                ArtistName = song.Artist,
                BitRate = song.BitRate == 0 ? (int?)null : song.BitRate,
                BPM = song.BPM == 0 ? (int?)null : song.BPM,
                Comments = song.Comment,
                IsCompilation = song.Compilation,
                Composer = song.Composer,
                DateAdded = song.DateAdded,
                DiscCount = song.DiscCount == 0 ? (int?)null : song.DiscCount,
                DiscNumber = song.DiscNumber == 0 ? (int?)null : song.DiscCount,
                Duration = song.Duration <= 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(song.Duration),
                IsEnabled = song.Enabled,
                IsExcludedFromShuffle = song.ExcludeFromShuffle,
                Genre = song.Genre,
                Grouping = song.Grouping,
                LibraryIndex = song.Index < 0 ? (int?)null : song.Index,
                Kind = song.KindAsString,
                FileLocation = song.Location,
                Modified = song.ModificationDate,
                TrackName = song.Name,
                PlayCount = song.PlayedCount,
                LastPlayed = song.PlayedDate,
                Rating = song.Rating <= 0 ? (int?)null : song.Rating,
                SampleRate = song.SampleRate <= 0 ? (int?)null : song.SampleRate,
                TrackCount = song.TrackCount <= 0 ? (int?)null : song.TrackCount,
                TrackNumber = song.TrackNumber <= 0 ? (int?)null : song.TrackNumber,
                Year = song.Year <= 0 ? (int?)null : song.Year
            };
            ret.ClearChanges();
            return ret;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised after the value of a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Come here when a property value has changed.  Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">The name of the property that just changed</param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
