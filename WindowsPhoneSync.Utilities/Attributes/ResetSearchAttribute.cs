using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities
{
    /// <summary>
    /// Simple property attribute.  If the attached property's
    /// value changes, the search results should be reset
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResetSearchAttribute : Attribute
    {
    }
}
