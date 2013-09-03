using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.DataContracts
{
    /// <summary>
    /// Request object for getting the information on a single track/song
    /// </summary>
    [DataContract]
    public class TrackInfoRequest : RequestBase
    {
        #region Fields

        /// <summary>
        /// The album's name
        /// </summary>
        private string albumName;

        /// <summary>
        /// The artist's name
        /// </summary>
        private string artistName;

        /// <summary>
        /// The song's name
        /// </summary>
        private string songName;

        /// <summary>
        /// The track's number
        /// </summary>
        private int? trackNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="userName">Web authentication username</param>
        /// <param name="password">Web authentication password</param>
        /// <param name="songName">The song's name to find</param>
        /// <param name="artistName">The artist's name</param>
        /// <param name="albumName">The album's name</param>
        /// <param name="trackNumber">The (optional) track number</param>
        public TrackInfoRequest(string userName = null, string password = null, string songName = null, string artistName = null, string albumName = null, int? trackNumber = null)
            : base(userName, password)
        {
            SongName = songName;
            ArtistName = artistName;
            AlbumName = albumName;
            TrackNumber = trackNumber;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the album's name
        /// </summary>
        [DataMember]
        public string AlbumName { get { return albumName; } set { SetValue(value, ref albumName); } }

        /// <summary>
        /// Get/set the artist's name
        /// </summary>
        [DataMember]
        public string ArtistName { get { return artistName; } set { SetValue(value, ref artistName); } }

        /// <summary>
        /// Get/set the song's name
        /// </summary>
        [DataMember]
        public string SongName { get { return songName; } set { SetValue(value, ref songName); } }

        /// <summary>
        /// Get/set the optional track number
        /// </summary>
        [DataMember]
        public int? TrackNumber { get { return trackNumber; } set { SetValue(value, ref trackNumber); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var additionalInfo = new List<string>();
            if (!string.IsNullOrWhiteSpace(SongName))
                additionalInfo.Add("Song Name: " + SongName);
            if (!string.IsNullOrWhiteSpace(ArtistName))
                additionalInfo.Add("Artist: " + ArtistName);
            if (!string.IsNullOrWhiteSpace(AlbumName))
                additionalInfo.Add("Album: " + AlbumName);
            if (TrackNumber.HasValue)
                additionalInfo.Add("Track Number: " + TrackNumber.Value.ToString());
            return string.Format(CultureInfo.CurrentCulture,
                                 "Track info {0}, {1}",
                                 base.ToString(),
                                 string.Join(", ", additionalInfo));
        }

        #endregion
    }
}
