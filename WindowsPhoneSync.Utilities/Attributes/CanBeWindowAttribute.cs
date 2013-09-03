using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities
{
    /// <summary>
    /// Flags a user control class as able to stand alone in
    /// its own window
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CanBeWindowAttribute : Attribute
    {
        /// <summary>
        /// Main constructor, supplying the window name/title
        /// </summary>
        /// <param name="name">The name/type/title of the window</param>
        public CanBeWindowAttribute(string name) : this(name, name) { }

        /// <summary>
        /// Alternate constructor, supplying window type
        /// </summary>
        /// <param name="name">The name/title of the window</param>
        /// <param name="type">The type name of the window</param>
        public CanBeWindowAttribute(string name, string type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// The window name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The window type
        /// </summary>
        public string Type { get; set; }
    }
}
