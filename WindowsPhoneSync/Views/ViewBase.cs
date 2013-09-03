using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics.Contracts;
using System.Windows.Media;
using WindowsPhoneSync.ViewModels;

namespace WindowsPhoneSync.Views
{
    public class ViewBase : UserControl, INotifyPropertyChanged
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
        /// A token routed command for clearing pages
        /// </summary>
        public static RoutedUICommand Clear = new RoutedUICommand("Clear", "Clear", typeof(ViewBase));

        /// <summary>
        /// Command to dismiss the message box
        /// </summary>
        private RelayCommand dismiss;

        #endregion

        #region Constructors

        /// <summary>
        /// Empty default constructor, here for completeness
        /// </summary>
        public ViewBase()
        {
            // The first time we get the control loaded event, go reread all the saved field
            // values related to the view:

            bool first = true;
            Loaded += (sender, e) =>
            {
                ViewModelBase viewModel = DataContext as ViewModelBase;
                if (viewModel == null)
                    return;
                if (first)
                    ApplicationViewModel.LoadFieldValues(viewModel);
                first = false;

                InitializeViewModel(viewModel);
            };
            Unloaded += ControlUnloaded;

            this.CommandBindings.Add(new CommandBinding(command: Clear,
                                                        executed: ClearViewModel,
                                                        canExecute: (sender, e) =>
                                                            {
                                                                if (DataContext == null ||
                                                                    !(DataContext is ViewModelBase))
                                                                    e.CanExecute = false;
                                                                else
                                                                    e.CanExecute = true;
                                                            }));
        }

        /// <summary>
        /// Constructor naming the ViewModel.  Allows us to do our stuff
        /// </summary>
        /// <param name="viewModel"></param>
        public ViewBase(ViewModelBase viewModel, bool preLoadFields = true)
        {
            DataContext = viewModel;
            InitializeViewModel(viewModel);

            // The first time we get the control loaded event, go reread all the saved field
            // values related to the view:

            bool first = true;
            Loaded += (sender, e) =>
                {
                    if (first && preLoadFields)
                        ApplicationViewModel.LoadFieldValues(viewModel);
                    first = false;
                    ControlLoaded(sender, e);
                };
            Unloaded += ControlUnloaded;

            this.CommandBindings.Add(new CommandBinding(command: Clear,
                                                        executed: ClearViewModel,
                                                        canExecute: (sender, e) =>
                                                        {
                                                            if (DataContext == null ||
                                                                !(DataContext is ViewModelBase))
                                                                e.CanExecute = false;
                                                            else
                                                                e.CanExecute = true;
                                                        }));
        }

        /// <summary>
        /// Initialize our view model's callbacks
        /// </summary>
        /// <param name="viewModel"></param>
        private void InitializeViewModel(ViewModelBase viewModel)
        {
            viewModel.UIAction = uiAction => Dispatcher.BeginInvoke(uiAction);

            viewModel.DisplayError += ((sender, e) =>
            {
                MessageText = e.Message;
                MessageTitle = e.Title;
                MessageBoxVisibility = Visibility.Visible;
            });
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

        #region Command Handlers

        /// <summary>
        /// Here on a bubble "Clear" command. Do so
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearViewModel(object sender, RoutedEventArgs e)
        {
            ViewModelBase vm = DataContext as ViewModelBase;
            if (vm == null)
                return;

            // Call the view model to clear all its input fields:

            vm.ClearFields();

            // Recurse, if necessary:

            e.Handled = false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Add a window to our list of things to be closed when we shut down
        /// </summary>
        /// <param name="window"></param>
        protected virtual void NewWindow(Window window)
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

        #region Control Events

        private void ControlLoaded(object sender, RoutedEventArgs e)
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
        /// Come here when this control is unloaded.  Close all child windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlUnloaded(object sender, RoutedEventArgs e)
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
