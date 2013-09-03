using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities
{
    /// <summary>
    /// A simple attribute to hang off UI related properties so that 
    /// their values can be saved/restored
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SavedDataPropertyAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public SavedDataPropertyAttribute() { }

        /// <summary>
        /// Constructor, supplying an alternate name for the property
        /// </summary>
        /// <param name="propertyName">The name under which the property should be saved</param>
        public SavedDataPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// The name of the sub-property to save/restore
        /// </summary>
        public string PropertyName { get; set; }
    }
}
