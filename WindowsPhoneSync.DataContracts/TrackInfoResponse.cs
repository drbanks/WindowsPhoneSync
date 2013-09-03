using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.DataContracts
{
    /// <summary>
    /// Response object returned by the "GetTrackInfo" service request
    /// </summary>
    [DataContract]
    public class TrackInfoResponse : ResponseBase
    {
        #region Fields

        /// <summary>
        /// The song information returned
        /// </summary>
        private SongMetadata song;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The returned song information
        /// </summary>
        [DataMember]
        public SongMetadata Song { get { return song; } set { SetValue(value, ref song); } }

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

        #region Private Methods

        #endregion
    }
}
