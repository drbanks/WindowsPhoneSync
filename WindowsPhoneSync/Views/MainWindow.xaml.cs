using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPhoneSync.ViewModels;
using WindowsPhoneSync.Web;

namespace WindowsPhoneSync.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowViewBase
    {
        #region Fields

        /// <summary>
        /// The system tray icon
        /// </summary>
        private System.Windows.Forms.NotifyIcon myNotifyIcon;

        /// <summary>
        /// Our view model
        /// </summary>
        private MainWindowViewModel vm;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
            : base(new MainWindowViewModel())
        {
            InitializeComponent();

            vm = DataContext as MainWindowViewModel;
            CreateTrayIcon();

            SetTrayIconText(PreferencesViewModel.Instance.Port);
            PreferencesViewModel.Instance.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Port")
                        SetTrayIconText(PreferencesViewModel.Instance.Port);
                };
            Closing += (sender, e) =>
                {
                    if (vm != null &&
                        vm.Close.CanExecute(null))
                        vm.Close.Execute(null);
                    ApplicationViewModel.SaveFields();
                };
        }

        /// <summary>
        /// Creates our tray icon and it associated context menu
        /// </summary>
        private void CreateTrayIcon()
        {

            myNotifyIcon = new System.Windows.Forms.NotifyIcon();
            myNotifyIcon.Icon = new System.Drawing.Icon(@"Images\sync.ico");

            // Enable our double-click handler, which will re-open the window if
            // minimized

            myNotifyIcon.MouseDoubleClick += TrayMouseDoubleClick;
            
            // Create the context menu

            myNotifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            myNotifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Windows Phone Sync Tool") { Enabled = false });
            myNotifyIcon.ContextMenu.MenuItems.Add("-");
            myNotifyIcon.ContextMenu.MenuItems.Add("Open", (sender, e) => WindowState = WindowState.Normal);
            myNotifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("E&xit", (sender, e) => Close()));
            
            // Turn the icon on

            myNotifyIcon.Visible = true;
        }

        /// <summary>
        /// Sets the tray icon hover tool tip text
        /// </summary>
        /// <param name="port">The port number we're listening on</param>
        private void SetTrayIconText(int port)
        {
            string[] addresses = (from address in WebServiceHost.GetListenerAddressList()
                                  select address + ":" + port.ToString()).ToArray();
            myNotifyIcon.Text = string.Format(CultureInfo.CurrentCulture,
                                              "Windows Phone Sync listening on {0}",
                                              string.Join(", ", addresses));
        }

        /// <summary>
        /// The user has double clicked the tray icon
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        void TrayMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Here when the window's minimize/restore state has changed
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                myNotifyIcon.Visible = true;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// Event handler for the window closing event.  Will signal the view model that things are shutting down
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Cancel event arguments, in case we want to abort the shutdown</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myNotifyIcon.Visible = false;
        }

        /// <summary>
        /// Here when the user clicks the Preferences menu item.  This will open the preferences window as an owned form
        /// </summary>
        /// <param name="sender">Uninteresting</param>
        /// <param name="e">Uninteresting</param>
        private void PreferencesClick(object sender, RoutedEventArgs e)
        {
            var prefs = new Preferences();
            prefs.Owner = this;
            NewWindow(prefs);
            prefs.Show();
        }
    }
}
