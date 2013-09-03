using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPhoneSync.ViewModels;

namespace WindowsPhoneSync.Views
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : WindowViewBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Preferences()
            : base(PreferencesViewModel.Instance, preLoadFields: false)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Here when the user clicks the OK button. Closes this dialog
        /// </summary>
        private void OkClick(object sender, RoutedEventArgs e)
        {
            // When the preferences are updated, force save the values:

            ApplicationViewModel.SaveFields();
            Close();
        }
    }
}
