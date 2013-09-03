using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace WindowsPhoneSync.DataContracts
{
    [DataContract]
    public class SongMetadata : MetadataBase
    {
        #region Fields

        /// <summary>
        /// The album's artist name
        /// </summary>
        private string albumArtist;

        /// <summary>
        /// The album's name
        /// </summary>
        private string albumName;

        /// <summary>
        /// The artist's name
        /// </summary>
        private string artistName;

        /// <summary>
        /// The song's bit-rate
        /// </summary>
        private int? bitRate;

        /// <summary>
        /// The song's beats per minute
        /// </summary>
        private int? bpm;

        /// <summary>
        /// Track comments
        /// </summary>
        private string comment;

        /// <summary>
        /// Composer(s)
        /// </summary>
        private string composer;

        /// <summary>
        /// The number of discs in the album
        /// </summary>
        private int? discCount;

        /// <summary>
        /// The disc number within the set
        /// </summary>
        private int? discNumber;

        /// <summary>
        /// The track duration
        /// </summary>
        private TimeSpan? duration;

        /// <summary>
        /// Date the track was added to the library
        /// </summary>
        private DateTime? dateAdded;

        /// <summary>
        /// The location of the track on disk or in the cloud
        /// </summary>
        private string fileLocation;

        /// <summary>
        /// The music genre
        /// </summary>
        private string genre;

        /// <summary>
        /// The music's group name
        /// </summary>
        private string grouping;

        /// <summary>
        /// Indicates that the track is part of a compilation album
        /// </summary>
        private bool isCompilation;

        /// <summary>
        /// Indicates that the track is checked in the library
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Indicates that the track should not be included in a shuffle play
        /// </summary>
        private bool isExcludedFromShuffle;

        /// <summary>
        /// The kind of media file this is
        /// </summary>
        private string kind;

        /// <summary>
        /// The track's index in the original library, if any
        /// </summary>
        private long? libraryIndex;

        /// <summary>
        /// UTC when this track was last played
        /// </summary>
        private DateTime? lastPlayed;

        /// <summary>
        /// The song's lyrics, if any
        /// </summary>
        private string lyrics;

        /// <summary>
        /// UTC when last modified
        /// </summary>
        private DateTime? modificationDate;

        /// <summary>
        /// The number of times this track has been played
        /// </summary>
        private int? playCount;

        /// <summary>
        /// User rating, from 0 to 100
        /// </summary>
        private int? rating;

        /// <summary>
        /// The song's original sample rate (usually 44.1KHz)
        /// </summary>
        private int? sampleRate;

        /// <summary>
        /// The number of tracks in the album
        /// </summary>
        private int? trackCount;

        /// <summary>
        /// The track's number
        /// </summary>
        private int? trackNumber;

        /// <summary>
        /// The track's name
        /// </summary>
        private string trackName;

        /// <summary>
        /// The year of the performance
        /// </summary>
        private int? year;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SongMetadata() { }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the album artist name
        /// </summary>
        [DataMember]
        public string AlbumArtist
        {
            get { return albumArtist; }
            set { SetValue(value, ref albumArtist); }
        }

        /// <summary>
        /// Get/set the album's name string
        /// </summary>
        [DataMember]
        public string AlbumName
        {
            get { return albumName; }
            set { SetValue(value, ref albumName); }
        }

        /// <summary>
        /// Get/set the full name of the artist
        /// </summary>
        [DataMember]
        public string ArtistName
        {
            get { return artistName; }
            set { SetValue(value, ref artistName); }
        }

        /// <summary>
        /// Gets the track's bit rate
        /// </summary>
        [DataMember]
        public int? BitRate
        {
            get { return bitRate; }
            set { SetValue(value, ref bitRate); }
        }

        /// <summary>
        /// Get/set the track's beats per minute
        /// </summary>
        [DataMember]
        public int? BPM
        {
            get { return bpm; }
            set { SetValue(value, ref bpm); }
        }

        /// <summary>
        /// Get/set the track comments
        /// </summary>
        [DataMember]
        public string Comments
        {
            get { return comment; }
            set { SetValue(value, ref comment); }
        }

        /// <summary>
        /// Get/set the names of the people who composed the track
        /// </summary>
        [DataMember]
        public string Composer
        {
            get { return composer; }
            set { SetValue(value, ref composer); }
        }

        /// <summary>
        /// Get/set the date/time (UTC) when this was added to the library
        /// </summary>
        [DataMember]
        public DateTime? DateAdded
        {
            get { return dateAdded; }
            set { SetValue(value, ref dateAdded); }
        }

        /// <summary>
        /// Get/set the number of discs in the set
        /// </summary>
        [DataMember]
        public int? DiscCount
        { 
            get { return discCount; }
            set { SetValue(value, ref discCount); }
        }

        /// <summary>
        /// Get/set the number of the current disc
        /// </summary>
        [DataMember]
        public int? DiscNumber
        {
            get { return discNumber; }
            set { SetValue(value, ref discNumber); }
        }

        /// <summary>
        /// Get/set the track duration
        /// </summary>
        [DataMember]
        public TimeSpan? Duration
        {
            get { return duration; }
            set { SetValue(value, ref duration); }
        }

        /// <summary>
        /// Get/set the location of the file on disk
        /// </summary>
        [DataMember]
        public string FileLocation
        {
            get { return fileLocation; }
            set { SetValue(value, ref fileLocation); }
        }

        /// <summary>
        /// Get/set the track's genre
        /// </summary>
        [DataMember]
        public string Genre
        {
            get { return genre; }
            set { SetValue(value, ref genre); }
        }

        /// <summary>
        /// Get/set the group's name
        /// </summary>
        [DataMember]
        public string Grouping
        {
            get { return grouping; }
            set { SetValue(value, ref grouping); }
        }

        /// <summary>
        /// Get/set whether this track is a part of a compilation
        /// </summary>
        [DataMember]
        public bool IsCompilation
        {
            get { return isCompilation; }
            set { SetValue(value, ref isCompilation); }
        }

        /// <summary>
        /// Get/set whether this track is checked in the library
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetValue(value, ref isEnabled); }
        }

        /// <summary>
        /// Get/set whether this track is excluded from shuffle play
        /// </summary>
        [DataMember]
        public bool IsExcludedFromShuffle
        {
            get { return isExcludedFromShuffle; }
            set { SetValue(value, ref isExcludedFromShuffle); }
        }

        /// <summary>
        /// Get/set the kind of track
        /// </summary>
        [DataMember]
        public string Kind
        {
            get { return kind; }
            set { SetValue(value, ref kind); }
        }

        /// <summary>
        /// Get/set the UTC when the track was last played
        /// </summary>
        [DataMember]
        public DateTime? LastPlayed
        {
            get { return lastPlayed; }
            set { SetValue(value, ref lastPlayed); }
        }

        /// <summary>
        /// Get the track's index within the main library
        /// </summary>
        [DataMember]
        public long? LibraryIndex
        {
            get { return libraryIndex; }
            set { SetValue(value, ref libraryIndex); }
        }

        /// <summary>
        /// Get/set the track's lyrics
        /// </summary>
        [DataMember]
        public string Lyrics
        {
            get { return lyrics; }
            set { SetValue(value, ref lyrics); }
        }

        /// <summary>
        /// Get/set the UTC when the track was last modified
        /// </summary>
        [DataMember]
        public DateTime? Modified
        {
            get { return modificationDate; }
            set { SetValue(value, ref modificationDate); }
        }

        /// <summary>
        /// Get/set the number of times the track has been played
        /// </summary>
        [DataMember]
        public int? PlayCount
        {
            get { return playCount; }
            set { SetValue(value, ref playCount); }
        }

        /// <summary>
        /// Get/set the user's rating (1-100)
        /// </summary>
        [DataMember]
        public int? Rating
        {
            get { return rating; }
            set { SetValue(value, ref rating); }
        }

        /// <summary>
        /// Get/set the song's sample rate
        /// </summary>
        [DataMember]
        public int? SampleRate
        {
            get { return sampleRate; }
            set { SetValue(value, ref sampleRate); }
        }

        /// <summary>
        /// Get/set the number of tracks in the album
        /// </summary>
        [DataMember]
        public int? TrackCount
        {
            get { return trackCount; }
            set { SetValue(value, ref trackCount); }
        }

        /// <summary>
        /// Get/set the track/song name
        /// </summary>
        [DataMember]
        public string TrackName
        {
            get { return trackName; }
            set { SetValue(value, ref trackName); }
        }

        /// <summary>
        /// Get/set the track's number
        /// </summary>
        [DataMember]
        public int? TrackNumber
        {
            get { return trackNumber; }
            set { SetValue(value, ref trackNumber); }
        }

        /// <summary>
        /// Get/set the year when the track was performed
        /// </summary>
        [DataMember]
        public int? Year
        {
            get { return year; }
            set { SetValue(value, ref year); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var info = new List<string>();
            info.Add("Track: " + string.Join(" / ", ArtistName, AlbumName, TrackName));
            if (TrackNumber.HasValue)
                info.Add("Track " + TrackNumber.Value.ToString() +
                         (TrackCount.HasValue ? (" of " + TrackCount.Value.ToString()) : ""));
            if (DiscNumber.HasValue)
                info.Add("Disc " + DiscNumber.Value.ToString() +
                         (DiscCount.HasValue ? (" of " + DiscCount.Value.ToString()) : ""));
            if (Duration.HasValue)
                info.Add(Duration.Value.ToString());
            if (Rating.HasValue)
                info.Add("Rating: " + Rating.Value.ToString());
            if (PlayCount.HasValue)
                info.Add("Played " + PlayCount.Value.ToString() + " time" + (PlayCount.Value == 1 ? "" : "s"));
            if (LastPlayed.HasValue)
                info.Add("Last played: " + LastPlayed.Value.ToLocalTime().ToString());

            return string.Join(", ", info);
        }

        #endregion
    }
}
