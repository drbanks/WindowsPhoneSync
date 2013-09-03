using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WindowsPhoneSync.Library;
using WindowsPhoneSync.DataContracts;

namespace WindowsPhoneSync.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class iTunesServiceInfo : IiTunesInfoService
    {
        /// <summary>
        /// Get a single song's metadata
        /// </summary>
        public SongMetadata GetMetaData(string artist, string album, string trackName)
        {
            var library = Library.Library.LibraryFactory("iTunes");
            var track = library.FindTrack(artist, album, trackName);
            return track;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
