using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WindowsPhoneSync.Utilities;

namespace WindowsPhoneSync.ViewModels
{
    /// <summary>
    /// A singleton viewmodel for the properties page
    /// </summary>
    public class PreferencesViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Our single instance
        /// </summary>
        private static readonly PreferencesViewModel instance;

        /// <summary>
        /// The number of minutes to keep iTunes alive (before killing it)
        /// </summary>
        private int iTunesIdleMinutes = 15;

        /// <summary>
        /// The web service password string to use for authentication
        /// </summary>
        private string password;

        /// <summary>
        /// The web service port to listen on
        /// </summary>
        private int port = 8001;

        /// <summary>
        /// The web service authentication username to use
        /// </summary>
        private string userName;

        #endregion

        #region Commands

        #endregion
        
        #region Constructors

        /// <summary>
        /// Make the constructor inaccessible outside us
        /// </summary>
        private PreferencesViewModel() 
        {
            // Force load our saved values:

            ApplicationViewModel.LoadFieldValues(this);
        }

        /// <summary>
        /// Static constructor:  On first reference, we'll instantiate the singleton instance
        /// </summary>
        static PreferencesViewModel()
        {
            instance = new PreferencesViewModel();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static PreferencesViewModel Instance { get { return instance; }  } 

        /// <summary>
        /// Get/set the number of minutes iTunes should idle before being killed
        /// </summary>
        [SavedDataProperty]
        public int ITunesIdleMinutes
        {
            get { return iTunesIdleMinutes; }
            set { SetValue(value, ref iTunesIdleMinutes); }
        }

        /// <summary>
        /// The web service password
        /// </summary>
        [SavedDataProperty]
        public string Password
        {
            get { return password; }
            set { SetValue(value, ref password); }
        }

        /// <summary>
        /// Get/set the listener port number
        /// </summary>
        [SavedDataProperty]
        public int Port
        {
            get { return port; }
            set { SetValue(value, ref port); }
        }

        /// <summary>
        /// The web service username to be used for authentication
        /// </summary>
        [SavedDataProperty]
        public string UserName
        {
            get { return userName; }
            set { SetValue(value, ref userName); }
        }

        #endregion

        #region Command Properties

        #endregion

        #region Command Handlers

        #endregion

        #region Public Methods

        #endregion
    }
}
