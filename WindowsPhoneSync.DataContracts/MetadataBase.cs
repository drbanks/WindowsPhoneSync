using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsPhoneSync.DataContracts
{
    /// <summary>
    /// Base implementation for media library metadata information
    /// </summary>
    public abstract class MetadataBase : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Tracks all the properties whose values have changed
        /// </summary>
        [XmlIgnore]
        private Dictionary<string, object> changedProperties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The list of changed properties
        /// </summary>
        [XmlIgnore]
        public string[] ChangedProperties { get { return changedProperties.Keys.ToArray(); } }

        /// <summary>
        /// Get/set a property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [XmlIgnore]
        public object this[string propertyName]
        {
            get { return Value(propertyName); }
            set { SetValue(propertyName, value); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears any history of changes.  Doesn't back out the changes; just forgets that
        /// they were made
        /// </summary>
        public void ClearChanges()
        {
            changedProperties.Clear();
        }

        /// <summary>
        /// Gets a list of the properties whose value has changed
        /// </summary>
        /// <returns>Array of property names</returns>
        public string[] GetChangedProperties()
        {
            return changedProperties.Keys.ToArray();
        }

        /// <summary>
        /// Determines whether a property's value has changed
        /// </summary>
        /// <param name="propertyName">The property's name</param>
        /// <returns>True if it's changed; false if not</returns>
        public bool HasPropertyChanged(string propertyName)
        {
            return changedProperties.ContainsKey(propertyName);
        }

        /// <summary>
        /// Gets the pre-changed value of a property, if it's changed
        /// </summary>
        /// <param name="propertyName">The property's name</param>
        /// <returns>Null if unchanged; otherwise the old property value</returns>
        public object OldValue(string propertyName)
        {
            if (!changedProperties.ContainsKey(propertyName))
                return null;
            return changedProperties[propertyName];
        }

        /// <summary>
        /// Sets the value of a property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetValue(string propertyName, object value)
        {
            var prop = GetType().GetRuntimeProperty(propertyName);
            if (prop == null)
                return;
            prop.SetValue(this, value);
        }

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Gets the value of a property by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object Value(string propertyName)
        {
            var prop = GetType().GetRuntimeProperty(propertyName);
            if (prop == null)
                return null;
            return prop.GetValue(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Search through a list of artwork images, see if a test image matches anything
        /// in the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        protected Artwork FindMatch(IEnumerable<Artwork> list, Artwork item)
        {
            foreach (var art in list)
            {
                if (art.Description != item.Description)
                    continue;
                if (art.Image != item.Image)
                {
                    if (art.Image == null ||
                        item.Image == null ||
                        art.Image.Length != item.Image.Length)
                        continue;
                }
                return art;
            }
            return null;
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
            // Only write the old value out if it's never changed before:

            if (!changedProperties.ContainsKey(propertyName))
                changedProperties[propertyName] = oldValue;

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
