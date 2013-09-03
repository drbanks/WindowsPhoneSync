using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPhoneSync.Utilities.EventArgs
{
    /// <summary>
    /// A simple event argument block for conveying error messages
    /// </summary>
    public class ErrorMessageEventArgs : System.EventArgs
    {
        /// <summary>
        /// Default constructor, probably not used directly
        /// </summary>
        public ErrorMessageEventArgs()
        {
        }

        /// <summary>
        /// Main constructor, taking the message text and title
        /// </summary>
        /// <param name="message">The error message text</param>
        /// <param name="title">The title for the message box</param>
        public ErrorMessageEventArgs(string message = null, string title = "Error")
        {
            Message = message;
            Title = title;
        }

        /// <summary>
        /// The error message text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The error title
        /// </summary>
        public string Title { get; set; }
    }
}
