using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities
{
    /// <summary>
    /// Stupid marker to force re-evaluation of CanExecute when the state
    /// of Idle changes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequeryOnIdleAttribute : Attribute
    {
    }
}
