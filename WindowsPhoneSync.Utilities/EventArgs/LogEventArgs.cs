using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPhoneSync.Utilities.EventArgs
{
    /// <summary>
    /// Severity level for a log event
    /// </summary>
    public enum LogSeverity
    {
        Debug = 0,
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// Immutable event arguments class for logging
    /// </summary>
    public class LogEventArgs : System.EventArgs
    {
        #region Fields

        /// <summary>
        /// The log text category
        /// </summary>
        private readonly string category;

        /// <summary>
        /// The text of the log
        /// </summary>
        private readonly string logText;

        /// <summary>
        /// The log severity
        /// </summary>
        private readonly LogSeverity severity;

        /// <summary>
        /// The date/time when the event occurred
        /// </summary>
        private readonly DateTime timeStamp;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new log event arguments block
        /// </summary>
        /// <param name="category">Category of the log event</param>
        /// <param name="logText">Text of the log event</param>
        /// <param name="severity">Event severity (default = Information)</param>
        /// <param name="timeStamp">Event timestamp, UTC (default = now)</param>
        public LogEventArgs(string category, string logText, LogSeverity severity = LogSeverity.Information, DateTime? timeStamp = null)
        {
            this.logText = logText;
            this.severity = severity;
            this.category = category;
            this.timeStamp = timeStamp == null ? DateTime.UtcNow : timeStamp.Value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the log entry category
        /// </summary>
        public string Category { get { return category; } }

        /// <summary>
        /// Gets the timestamp, converted to local time
        /// </summary>
        public DateTime LocalTimeStamp { get { return timeStamp.ToLocalTime(); } }

        /// <summary>
        /// Gets the text of the log event
        /// </summary>
        public string LogText { get { return logText; } }

        /// <summary>
        /// Gets the severity level of this log entry
        /// </summary>
        public LogSeverity Severity { get { return severity; } }

        /// <summary>
        /// Gets the date/time of the event
        /// </summary>
        public DateTime TimeStamp { get { return timeStamp; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generic stringer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "{0} log event: {1} at {2}",
                                 Severity,
                                 LogText,
                                 TimeStamp);
        }

        #endregion
    }
}
