using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.DataContracts
{
    /// <summary>
    /// Possible response codes for the web service execution
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// The hopeful return
        /// </summary>
        Success = 0,

        /// <summary>
        /// Mostly successful, with issues
        /// </summary>
        Warning,

        /// <summary>
        /// Failure
        /// </summary>
        Failure
    }

    [DataContract]
    public abstract class ResponseBase : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Indicates success or failure of the web service execution
        /// </summary>
        private ResponseCode code;

        /// <summary>
        /// The human readable explanation of what happened
        /// </summary>
        private string description;

        #endregion

        #region Constructors

        /// <summary>
        /// Main constructor; just initializes with the code and description
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        public ResponseBase(ResponseCode code = ResponseCode.Success, string description = null)
        {
            Code = code;
            Description = description;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the response code
        /// </summary>
        [DataMember]
        public ResponseCode Code { get { return code; } set { SetValue(value, ref code); } }

        [DataMember]
        public string Description { get { return description; } set { SetValue(value, ref description); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "Response: {0}, {1}",
                                 Code,
                                 Description);
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
