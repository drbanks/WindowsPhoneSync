using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Windows;
using WindowsPhoneSync.ViewModels;

namespace WindowsPhoneSync.Views
{
    public class WindowViewBase : Window, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Text for the message box replacement
        /// </summary>
        private string messageText;

        /// <summary>
        /// Title text for the message box replacement
        /// </summary>
        private string messageTitle;

        /// <summary>
        /// The visibility of the message box replacement
        /// </summary>
        private Visibility visible = Visibility.Hidden;

        /// <summary>
        /// Our list of child windows to close when we close
        /// </summary>
        private List<Window> windowList = new List<Window>();

        #endregion

        #region Commands

        /// <summary>
        /// Command to dismiss the message box
        /// </summary>
        private RelayCommand dismiss;

        #endregion

        #region Construtors

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowViewBase()
        {
            Closing += WindowClosing;
        }

        /// <summary>
        /// Constructor that supplies our ViewModel.  Allows us to intercept reschedule and message box requests
        /// </summary>
        /// <param name="viewModel">The view model to act as this view's data context</param>
        /// <param name="preLoadFields">(Optional, defaults to true).  If false, the SavedDataProperty properties won't be loaded from the saved value file</param>
        public WindowViewBase(ViewModelBase viewModel, bool preLoadFields = true) : this()
        {
            DataContext = viewModel;
            viewModel.UIAction = uiAction => Dispatcher.BeginInvoke(uiAction);

            viewModel.DisplayError += ((sender, e) =>
            {
                MessageText = e.Message;
                MessageTitle = e.Title;
                MessageBoxVisibility = Visibility.Visible;
            });


            // The first time we get the control loaded event, go reread all the saved field
            // values related to the view:

            bool first = true;
            Loaded += (sender, e) =>
            {
                if (first && preLoadFields)
                    ApplicationViewModel.LoadFieldValues(viewModel);
                first = false;
                WindowLoaded(sender, e);
            };
            Unloaded += WindowUnloaded;
        }

        #endregion

        #region Properties

        /// <summary>
        /// User clicks the dismiss button on the message box
        /// </summary>
        public RelayCommand Dismiss
        {
            get
            {
                if (dismiss == null)
                {
                    dismiss = new RelayCommand(param => MessageBoxVisibility = Visibility.Hidden);
                }
                return dismiss;
            }
        }

        /// <summary>
        /// Gets the message box visibility
        /// </summary>
        public Visibility MessageBoxVisibility
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    OnPropertyChanged("MessageBoxVisibility");
                }
            }
        }

        /// <summary>
        /// Gets the message box text
        /// </summary>
        public string MessageText
        {
            get { return messageText; }
            set
            {
                if (messageText != value)
                {
                    messageText = value;
                    OnPropertyChanged("MessageText");
                }
            }
        }

        /// <summary>
        /// Gets the message title text
        /// </summary>
        public string MessageTitle
        {
            get { return messageTitle; }
            set
            {
                if (messageTitle != value)
                {
                    messageTitle = value;
                    OnPropertyChanged("MessageTitle");
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Add a window to our list of things to be closed when we shut down
        /// </summary>
        /// <param name="window">The window to be added to the list</param>
        protected void NewWindow(Window window)
        {
            lock (windowList)
            {
                windowList.Add(window);
            }
            window.Closing += (sender, e) =>
            {
                lock (windowList)
                {
                    if (windowList.Contains(window))
                        windowList.Remove(window);
                }
            };
        }

        #endregion

        #region Window Events

        /// <summary>
        /// Here on window load.  Clear our list of child windows
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (windowList != null)
            {
                lock (windowList)
                    windowList.Clear();
            }
            else
                windowList = new List<Window>();
        }

        /// <summary>
        /// Come here when this window is unloaded.  Close all child windows
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {
            lock (windowList)
            {
                windowList.ForEach(window => window.Close());
                windowList.Clear();
            }
        }

        /// <summary>
        /// Come here on window close.  Close all our child windows
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            lock (windowList)
            {
                windowList.ForEach(window => window.Close());
                windowList.Clear();
            }
        }

        #endregion

        #region ViewModel Events

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property's value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property's value has changed.  Raises the property changed event
        /// </summary>
        /// <param name="propertyName">The name of the property that's changed</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
