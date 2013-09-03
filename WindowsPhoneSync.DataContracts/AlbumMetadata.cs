using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.DataContracts
{
    /// <summary>
    /// Information associated with a specific album
    /// </summary>
    [DataContract]
    public class AlbumMetadata : MetadataBase, INotifyPropertyChanged
    {
        #region Fields

        private string artistName;

        private Artwork[] artwork;

        private string comments;

        private int? discCount;

        private bool isCollection;

        private string name;

        private int? rating;

        private string sortName;

        private int? trackCount;

        private int? year;

        #endregion

        #region Constructors, Factories

        /// <summary>
        /// Create an album from a bunch of songs
        /// </summary>
        /// <param name="songs"></param>
        /// <returns></returns>
        public static AlbumMetadata Factory(IEnumerable<SongMetadata> songs = null)
        {
            // If no songs, create an empty album

            if (songs == null)
                return new AlbumMetadata();
            
            // Compute the number of tracks in the album:

            int? trackCount = (from song in songs
                               where song.TrackCount.HasValue
                               select song.TrackCount).Max();
            int? trackMax = songs.Max(song => song.TrackNumber);
            trackCount = trackCount ?? trackMax ?? songs.Count();
            int? discCount = (from song in songs
                              where song.DiscCount.HasValue
                              select song.DiscCount).Max();
            int? discMax = songs.Max(song => song.DiscNumber);
            discCount = discCount ?? discMax;

            string albumArtist = (from song in songs
                                  where !string.IsNullOrWhiteSpace(song.AlbumArtist)
                                  select song.AlbumArtist).FirstOrDefault();

            var ret = new AlbumMetadata
            {
                ArtistName = albumArtist,
                DiscCount = discCount,
            };
            ret.ClearChanges();
            return ret;
        }

        #endregion

        #region Properties

        [DataMember]
        public string ArtistName { get { return artistName; } set { SetValue(value, ref artistName); } }

        [DataMember]
        public Artwork[] Artwork { get { return artwork; } set { SetValue(value, ref artwork); } }

        [DataMember]
        public string Comments { get { return comments; } set { SetValue(value, ref comments); } }

        [DataMember]
        public int? DiscCount { get { return discCount; } set { SetValue(value, ref discCount); } }

        [DataMember]
        public bool IsCollection { get { return isCollection; } set { SetValue(value, ref isCollection); } }

        [DataMember]
        public string Name { get { return name; } set { SetValue(value, ref name); } }

        [DataMember]
        public int? Rating { get { return rating; } set { SetValue(value, ref rating); } }

        [DataMember]
        public string SortName { get { return sortName; } set { SetValue(value, ref sortName); } }

        [DataMember]
        public int? TrackCount { get { return trackCount; } set { SetValue(value, ref trackCount); } }

        [DataMember]
        public int? Year { get { return year; } set { SetValue(value, ref year); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Merge two album metadata instances
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static AlbumMetadata Merge(AlbumMetadata first, AlbumMetadata second)
        {
            // Merge the artwork:

            var outputList = new List<Artwork>(first.Artwork ?? new Artwork[0]);;
            foreach (var image in second.Artwork)
            {
                if (outputList.Contains(image))
                    continue;
                if (first.FindMatch(outputList, image) != null)
                    continue;
                outputList.Add(image);
            }
            int? rating = first.Rating ?? second.rating;
            if (first.Rating.HasValue && second.rating.HasValue)
                rating = (first.rating.Value + second.Rating.Value) / 2;

            return new AlbumMetadata
            {
                ArtistName = string.IsNullOrWhiteSpace(first.ArtistName) ? second.ArtistName : first.ArtistName,
                Artwork = outputList.ToArray(),
                Comments = string.IsNullOrWhiteSpace(first.Comments) ? second.Comments : first.Comments,
                DiscCount = first.DiscCount < second.DiscCount ? second.DiscCount : first.DiscCount,
                IsCollection = first.IsCollection | second.IsCollection,
                Name = string.IsNullOrWhiteSpace(first.Name) ? second.Name : first.Name,
                Rating = rating,
                SortName = string.IsNullOrWhiteSpace(first.SortName) ? second.SortName : first.sortName,
                TrackCount = first.TrackCount < second.TrackCount ? second.TrackCount : first.TrackCount,
                Year = first.Year ?? second.Year
            };
        }

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Album: {0}, Artist: {1}", Name, ArtistName);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
