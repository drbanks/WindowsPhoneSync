using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindowsPhoneSync.DataContracts;
using WindowsPhoneSync.iTunes;

namespace WindowsPhoneSync.Library
{
    public class Library : INotifyPropertyChanged
    {
        #region Fields

        #endregion

        #region Constructors, Factories

        /// <summary>
        /// Factory to create an interface to whatever back-end we're using
        /// </summary>
        /// <param name="libraryType"></param>
        /// <returns></returns>
        public static ILibrary LibraryFactory(string libraryType)
        {
            switch (libraryType)
            {
                case "iTunes":
                    return new iTunesLibrary();
            }
            throw new NotImplementedException("Unsupported library type " + libraryType);
        }

        #endregion

        #region Properties

        #endregion

        #region Public Static Methods

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
