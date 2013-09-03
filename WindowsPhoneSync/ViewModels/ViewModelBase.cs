using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using WindowsPhoneSync.Utilities;
using WindowsPhoneSync.Utilities.EventArgs;

namespace WindowsPhoneSync.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Fields

        /// <summary>
        /// List of commands that need to be requeried when the state of IsIdle
        /// changes
        /// </summary>
        private List<Tuple<RelayCommand, MethodInfo>> commandRequeryList = new List<Tuple<RelayCommand, MethodInfo>>();

        /// <summary>
        /// A once a second timer for the elapsed time
        /// </summary>
        private Timer refreshTimer;

        #endregion

        #region Bound Fields

        /// <summary>
        /// True if we don't have a web service call in progress
        /// </summary>
        private bool idle = true;

        /// <summary>
        /// When the web service call started
        /// </summary>
        private DateTime? started;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.  Just makes our presence known
        /// </summary>
        /// <param name="doNotRegister">If true, the instance will not be registered with the viewmodel framework</param>
        public ViewModelBase(bool doNotRegister = false)
        {
            UIAction = uiAction => uiAction();
            if (!doNotRegister)
                ApplicationViewModel.Register(this);

            // TODO: Add capabilities check

            IsReadEnabled = IsWriteEnabled = true;
        }

        /// <summary>
        /// Destructor.  Remove us from the list of active viewModels.
        /// </summary>
        ~ViewModelBase()
        {
            ApplicationViewModel.DeRegister(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the elapsed time we've been waiting on the web service
        /// </summary>
        public virtual string ElapsedTime
        {
            get
            {
                if (!started.HasValue)
                    return string.Empty;
                int seconds = (int)(DateTime.Now - started.Value).TotalSeconds;
                return (string.Format("{0} second{1}",
                                      seconds,
                                      seconds == 1 ? "" : "s"));
            }
        }

        /// <summary>
        /// True if we have any clearable fields
        /// </summary>
        public bool HasClearableFields
        {
            get
            {
                return (from property in GetType().GetProperties()
                        let attrib = property.GetCustomAttributes(typeof(SavedDataPropertyAttribute), false).FirstOrDefault() as SavedDataPropertyAttribute
                        where attrib != null
                        select property).Count() > 0;

            }
        }

        /// <summary>
        /// Gets the name of this instance.  For most UI related ViewModels,
        /// we only have one instance, so we can just default to the class name.
        /// Otherwise, interested ViewModels should override this
        /// </summary>
        public virtual string InstanceName
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// True if we're not executing anything right now
        /// </summary>
        public virtual bool IsIdle
        {
            get { return idle; }
            set
            {
                if (idle != value)
                {
                    NotifyPropertyChanging("IsIdle");
                    NotifyPropertyChanging("IsRuning");
                    idle = value;
                    NotifyPropertyChanged("IsIdle");
                    NotifyPropertyChanged("IsRunning");

                    RequeryCommands();
                }
            }
        }

        /// <summary>
        /// True if we're allowed to read data
        /// </summary>
        public virtual bool IsReadEnabled { get; set; }

        /// <summary>
        /// The inverse of IsIdle
        /// </summary>
        public virtual bool IsRunning
        {
            get { return !IsIdle; }
            set { IsIdle = !value; }
        }

        /// <summary>
        /// True if we're allowed to write data
        /// </summary>
        public virtual bool IsWriteEnabled { get; set; }

        /// <summary>
        /// The date/time when the web service call was initiated
        /// </summary>
        public virtual DateTime? Started
        {
            get { return started; }
            set
            {
                if (started != value)
                {
                    started = value;
                    NotifyPropertyChanged("Started");
                    NotifyPropertyChanged("ElapsedTime");

                    // If we just set this to null, clear the refresh timer:

                    if (!value.HasValue)
                    {
                        IsIdle = true;
                        if (refreshTimer != null)
                        {
                            refreshTimer.Stop();
                            refreshTimer.Dispose();
                            refreshTimer = null;
                        }
                    }
                    else if (refreshTimer == null)
                    {
                        // New value, set a new timer if necessary:

                        IsRunning = true;
                        refreshTimer = new Timer(1000)
                        {
                            AutoReset = true,
                            Enabled = true
                        };
                        refreshTimer.Elapsed += (sender, e) => NotifyPropertyChanged("ElapsedTime");
                        refreshTimer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Simple exposed delegate for rescheduling to the UI dispatcher
        /// </summary>
        public Action<Action> UIAction { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clear all the savable form fields
        /// </summary>
        /// <param name="param"></param>
        public void ClearFields()
        {
            var fields = from property in GetType().GetProperties()
                         let attrib = property.GetCustomAttributes(typeof(SavedDataPropertyAttribute), false).FirstOrDefault() as SavedDataPropertyAttribute
                         where attrib != null
                         select new { Property = property, Attribute = attrib };
            foreach (var field in fields)
                field.Property.SetValue(this, null, new object[0]);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Check our read enabled status; display error and return false if not
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckReadEnabled()
        {
            if (IsReadEnabled)
                return true;

            // Not enabled:

            OnDisplayError("You are not authorized to execute this function", "Not Authorized");
            return false;
        }

        /// <summary>
        /// Check our write enabled status; display error and return false if not enabled
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckWriteEnabled()
        {
            if (IsWriteEnabled)
                return true;

            // Not enabled

            OnDisplayError("You are not authorized to execute this function", "Not Authorized");
            return false;
        }

        /// <summary>
        /// Read a list of string values from app.config
        /// </summary>
        /// <param name="listName">The list's name</param>
        /// <param name="storage">Where to store the list</param>
        protected virtual void GetAppConfigList<CollectionType>(string listName, ICollection<CollectionType> storage)
        {
            var types = ConfigurationManager.GetSection(listName) as IEnumerable<CollectionType>;
            if (types == null ||
                types.Count() == 0)
                return;
            {
                foreach (var type in types)
                    storage.Add(type);
            }
        }

        /// <summary>
        /// Compute a date range given a string range name
        /// </summary>
        /// <param name="range">User meaningful range ("Today," "Yesterday," etc)</param>
        /// <returns></returns>
        protected static Tuple<DateTime?, DateTime?> PresetDateRange(string range)
        {
            if (string.IsNullOrWhiteSpace(range))
                return null;

            var ret = new Tuple<DateTime?, DateTime?>(DateTime.MinValue, DateTime.MaxValue);
            DateTime today = DateTime.UtcNow.Date;

            switch (range.ToLower())
            {
                case "clear":
                    ret = new Tuple<DateTime?, DateTime?>(null, null);
                    break;

                case "today":
                    ret = new Tuple<DateTime?, DateTime?>(today, today.AddDays(1));
                    break;

                case "yesterday":
                    ret = new Tuple<DateTime?, DateTime?>(today.AddDays(-1), today);
                    break;

                case "week":
                    var start = today - TimeSpan.FromDays((int)today.DayOfWeek);
                    ret = new Tuple<DateTime?, DateTime?>(start, start.AddDays(7));
                    break;

                case "month":
                    var monthStart = today - TimeSpan.FromDays(today.Day - 1);
                    ret = new Tuple<DateTime?, DateTime?>(monthStart, monthStart.AddMonths(1));
                    break;

                case "year":
                    var yearStart = today - TimeSpan.FromDays(today.DayOfYear - 1);
                    ret = new Tuple<DateTime?, DateTime?>(yearStart, yearStart.AddYears(1));
                    break;

                default:
                    return null;
            }

            return ret;
        }

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

            if (!string.IsNullOrWhiteSpace(propertyName))
                NotifyPropertyChanging(propertyName);
            oldValue = newValue;
            if (!string.IsNullOrWhiteSpace(propertyName))
                NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Set us as running
        /// </summary>
        /// <remarks>Sets IsIdle to false, starts the elapsed time clock</remarks>
        protected virtual void StartRunning()
        {
            Started = DateTime.Now;
        }

        /// <summary>
        /// Signal that we're no longer running
        /// </summary>
        /// <returns></returns>
        protected virtual TimeSpan StopRunning()
        {
            TimeSpan elapsed = TimeSpan.Zero;
            if (Started.HasValue)
                elapsed = DateTime.Now - Started.Value;
            Started = null;
            return elapsed;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Requery all the commands waiting for IsIdle to change
        /// </summary>
        internal void RequeryCommands()
        {
            foreach (var command in GetRequeryList())
                command.Item2.Invoke(command.Item1, new object[0]);
        }

        /// <summary>
        /// Maintain a most recently used list.  The selection will be added to the top of the list,
        /// and if the list is too long, it will be truncated
        /// </summary>
        /// <param name="selection">The selected/new item</param>
        /// <param name="list">The list</param>
        /// <param name="length">The max length of the list</param>
        internal void UpdateMruList(string selection, ObservableCollection<string> list, int length)
        {
            // Get the default view on the collection:

            var view = CollectionViewSource.GetDefaultView(list);

            // If it's already in the list, just move it to the top:

            if (list.Contains(selection))
            {
                list.Remove(selection);
                list.Insert(0, selection);
                view.MoveCurrentToFirst();
                return;
            }

            // If the list is already at max length, delete the last item:

            while (list.Count >= length)
                list.RemoveAt(length - 1);

            // Insert the new item at the top of the list:

            list.Insert(0, selection);
            view.MoveCurrentToFirst();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the list of commands associated with this class that
        /// must be requeried when the state of IsIdle changes
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Tuple<RelayCommand, MethodInfo>> GetRequeryList()
        {
            lock (commandRequeryList)
            {
                if (commandRequeryList.Count == 0)
                    ReflectCommands();

                return commandRequeryList.ToArray();
            }
        }

        /// <summary>
        /// Reflect out all the requeryable commands
        /// </summary>
        private void ReflectCommands()
        {
            lock (commandRequeryList)
            {
                commandRequeryList.Clear();
                commandRequeryList.AddRange(from command in GetType().GetProperties()
                                            let attrib = command.GetCustomAttributes(typeof(RequeryOnIdleAttribute), true).FirstOrDefault()
                                            let commandValue = command.GetValue(this, new object[0]) as RelayCommand
                                            where attrib != null &&
                                                  commandValue != null
                                            let requery = commandValue.GetType().GetMethod("RaiseCanExecuteChanged")
                                            where requery != null
                                            select new Tuple<RelayCommand, MethodInfo>(commandValue, requery));
            }
        }

        #endregion
        
        #region Control Events

        /// <summary>
        /// Called when a datagrid is auto-generating columns.  Fix up the alignment,
        /// etc.
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">The columns event arguments</param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            ColumnAttribute colDef = ((PropertyDescriptor)e.PropertyDescriptor).Attributes[typeof(ColumnAttribute)] as ColumnAttribute;
            if (colDef != null)
            {
                if (colDef.Hide)
                    e.Cancel = true;
                else
                {
                    var style = new Style();
                    if (!string.IsNullOrWhiteSpace(colDef.TextAlignment))
                    {
                        HorizontalAlignment horiz = HorizontalAlignment.Stretch;
                        Enum.TryParse<HorizontalAlignment>(colDef.TextAlignment, out horiz);
                        TextAlignment textAlign = TextAlignment.Left;
                        Enum.TryParse<TextAlignment>(colDef.TextAlignment, out textAlign);
                        style.Setters.Add(new Setter(TextBox.HorizontalContentAlignmentProperty, horiz));
                        style.Setters.Add(new Setter(Label.HorizontalContentAlignmentProperty, horiz));
                        style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, textAlign));
                    }
                    if (colDef.IsReadOnly)
                    {
                        style.Setters.Add(new Setter(DataGridTextColumn.IsReadOnlyProperty, true));
                        style.Setters.Add(new Setter(TextBoxBase.IsReadOnlyProperty, true));
                    }
                    e.Column.CellStyle = style;
                    e.Column.HeaderStyle = style;
                    e.Column.Header = colDef.Name;
                }
            }
            else
                e.Cancel = true;
        }

        /// <summary>
        /// Called after column autogeneration is complete.  Fix the column ordering
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">The columns event arguments</param>
        private void DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid mainDataGrid = sender as DataGrid;
            if (mainDataGrid == null)
                return;
            var recordType = mainDataGrid.ItemsSource.GetType().BaseType.GetGenericArguments()[0];
            var cols = (from property in recordType.GetProperties()
                        let attribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault()
                        where attribute != null && !attribute.Hide
                        orderby attribute.Ordinal
                        select attribute);
            var colDict = cols.ToDictionary(col => col.Name);
            foreach (var col in mainDataGrid.Columns)
            {
                // Fix nullable data types:

                if (col is DataGridTextColumn)
                {
                    DataGridTextColumn textCol = col as DataGridTextColumn;
                    Binding binding = textCol.Binding as Binding;
                    var propertyType = recordType.GetProperty(binding.Path.Path);
                    if (propertyType != null &&
                        propertyType.PropertyType.IsGenericType &&
                        propertyType.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition())
                    {

                        if (binding.TargetNullValue == null ||
                            binding.TargetNullValue == DependencyProperty.UnsetValue)
                            binding.TargetNullValue = string.Empty;
                    }
                }
                col.DisplayIndex = colDict[col.Header.ToString()].Ordinal;
            }
        }

        #endregion

        #region Common ViewModel Events

        /// <summary>
        /// Event that gets raised when we need to display some error message
        /// </summary>
        public event EventHandler<ErrorMessageEventArgs> DisplayError;

        /// <summary>
        /// Here when we need to display an error message.  Raise the DisplayError and let the view
        /// handle it.
        /// </summary>
        /// <param name="message">The error message text</param>
        /// <param name="title">The error message title/header</param>
        protected void OnDisplayError(string message = null, string title = "Error")
        {
            var handler = DisplayError;
            if (handler != null)
                handler(this, new ErrorMessageEventArgs(message, title));
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property's value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify subscribers that the value of a property has potentially changed
        /// </summary>
        /// <param name="propertyName">The name of the property that's changed</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        #endregion

        #region INotifyPropertyChanging Members

        /// <summary>
        /// Raised when a property's value is about to change
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Notify subscribers that the value of a property is about to change
        /// </summary>
        /// <param name="propertyName">The name of the property that's changing</param>
        protected void NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
                handler(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion
    }
}
