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
    /// Base class for all request objects
    /// </summary>
    [DataContract]
    public abstract class RequestBase : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The web service authentication password
        /// </summary>
        private string password;

        /// <summary>
        /// The name of the user, for authentication
        /// </summary>
        private string userName;

        #endregion

        #region Constructors

        /// <summary>
        /// Main constructor, supplying user name and password
        /// </summary>
        /// <param name="userName">The web authentication user name</param>
        /// <param name="password">The web authentication password</param>
        public RequestBase(string userName = null, string password = null)
        {
            UserName = userName;
            Password = password;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Get/set the web service authentication password
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return password; }
            set { SetValue(value, ref password); }
        }

        /// <summary>
        /// Get/set the web service authentication user name
        /// </summary>
        [DataMember]
        public string UserName
        {
            get { return userName; }
            set { SetValue(value, ref userName); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Request, user name: " + UserName;
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
