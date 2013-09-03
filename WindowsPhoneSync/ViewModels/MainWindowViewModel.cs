using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPhoneSync.Utilities.EventArgs;
using WindowsPhoneSync.Web;

namespace WindowsPhoneSync.ViewModels
{
    /// <summary>
    /// Backing viewmodel for the main window
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Bound Fields

        /// <summary>
        /// The activity log storage
        /// </summary>
        private ObservableCollection<LogEventArgs> log = new ObservableCollection<LogEventArgs>();

        /// <summary>
        /// Our main properties page
        /// </summary>
        private PreferencesViewModel properties;

        #endregion

        #region Commands

        /// <summary>
        /// Here on a close event
        /// </summary>
        private RelayCommand close;

        #endregion

        #region Constructors

        /// <summary>
        /// Sole constructor.  Just grabs the properties page
        /// </summary>
        public MainWindowViewModel() : base()
        {
            Properties = PreferencesViewModel.Instance;

            Log.Clear();
            Log.Add(new LogEventArgs("UI", "Started", LogSeverity.Debug));

            // Start the listener:

            WebServiceHost.WebServiceLogger += (sender, e) => UIAction(() => Log.Add(e));
            WebServiceHost.StartWebService(Properties.Port);

            // If the port numer changes in the preferences, restart the listener:

            Properties.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName != "Port")
                        return;
                    WebServiceHost.StopWebService();
                    WebServiceHost.StartWebService(Properties.Port);
                };
        }

        #endregion

        #region Properties

        /// <summary>
        /// The activity log
        /// </summary>
        public ObservableCollection<LogEventArgs> Log
        {
            get { return log; }
            set { SetValue(value, ref log); }
        }

        /// <summary>
        /// Get/set our properties page
        /// </summary>
        public PreferencesViewModel Properties
        {
            get { return properties; }
            set { SetValue(value, ref properties); }
        }

        #endregion

        #region Command Properties

        /// <summary>
        /// Command to notify us that the application is closing
        /// </summary>
        public RelayCommand Close
        {
            get
            {
                if (close == null)
                    close = new RelayCommand(param => OnClose(param));
                return close;
            }
        }

        #endregion

        #region Command Handlers

        /// <summary>
        /// Called when the application is closing
        /// </summary>
        private void OnClose(object param)
        {
            WebServiceHost.StopWebService();
        }

        #endregion
    }
}
