using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CapabilitiesAttribute : Attribute
    {
        /// <summary>
        /// Default constructor, probably not used directly
        /// </summary>
        public CapabilitiesAttribute() { }

        /// <summary>
        /// Constructor for read-only
        /// </summary>
        /// <param name="readCapability">The name of the capability required for read access</param>
        public CapabilitiesAttribute(string readCapability)
        {
            ReadCapability = readCapability;
        }

        /// <summary>
        /// Read/write constructor
        /// </summary>
        /// <param name="readCapability">The name of the capability required for read access</param>
        /// <param name="writeCapability">The name of the capability required for write access</param>
        public CapabilitiesAttribute(string readCapability, string writeCapability)
        {
            ReadCapability = readCapability;
            WriteCapability = writeCapability;
        }

        /// <summary>
        /// List of other capability names
        /// </summary>
        public string OtherCapabilities { get; set; }

        /// <summary>
        /// Read capability name
        /// </summary>
        public string ReadCapability { get; set; }

        /// <summary>
        /// Write capability name
        /// </summary>
        public string WriteCapability { get; set; }
    }
}
