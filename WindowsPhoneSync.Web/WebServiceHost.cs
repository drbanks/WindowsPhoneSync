using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsPhoneSync.Utilities;
using WindowsPhoneSync.Utilities.EventArgs;

namespace WindowsPhoneSync.Web
{
    /// <summary>
    /// Our self-hosted service
    /// </summary>
    public static class WebServiceHost
    {
        #region Subscriptions

        /// <summary>
        /// Raised to signal an event log
        /// </summary>
        public static event EventHandler<LogEventArgs> WebServiceLogger;

        #endregion

        #region Fields

        /// <summary>
        /// The thread we're running the listener on
        /// </summary>
        private static Thread serviceThread;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets the list of IP addresses we'll be listening on
        /// </summary>
        /// <returns></returns>
        public static string[] GetListenerAddressList()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from address in host.AddressList
                    where address.AddressFamily == AddressFamily.InterNetwork &&
                          !address.ToString().StartsWith("169")
                    select address.ToString()).ToArray();
        }

        /// <summary>
        /// Start the web service listener
        /// </summary>
        /// <param name="port">The port number to listen on</param>
        public static void StartWebService(int port)
        {
            serviceThread = new Thread(ServiceListener) { Name = "Web Service", IsBackground = true };
            serviceThread.Start(port);
        }

        /// <summary>
        /// Stop the web service listener
        /// </summary>
        public static void StopWebService()
        {
            if (serviceThread == null)
                return;
            serviceThread.Abort();
            serviceThread = null;
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Raise a log event
        /// </summary>
        /// <param name="sender">The instance of the object raising the event</param>
        /// <param name="text">The text of the logged event</param>
        /// <param name="severity">The event severity</param>
        /// <param name="timeStamp">The (optional) event timestamp (UTC)</param>
        internal static void LogAnEvent(object sender, string text, LogSeverity severity = LogSeverity.Information, DateTime? timeStamp = null)
        {
            if (sender == null)
                sender = Thread.CurrentThread;
            LogAnEvent(sender, new LogEventArgs("Service", text, severity, timeStamp));
        }

        /// <summary>
        /// Raise a log event
        /// </summary>
        /// <param name="sender">The instance of the object raising the event</param>
        /// <param name="e">The event arguments</param>
        internal static void LogAnEvent(object sender, LogEventArgs e)
        {
            var handler = WebServiceLogger;
            if (handler != null)
                handler(sender, e);
        }

        #endregion

        #region Service Host Thread

        /// <summary>
        /// Infinite running service listener for the GetIdentity web service
        /// </summary>
        /// <param name="uninteresting"></param>
        /// <param name="portNumber">The port number to listen on</param>
        static void ServiceListener(object portNumber)
        {
            if (portNumber == null ||
                (portNumber is int && (int)portNumber <= 0))
                portNumber = 8001;

            Uri baseAddress = new Uri("http://localhost:" + portNumber.ToString() + "/iTunesServiceInfo");
            ServiceHost serviceHost = new ServiceHost(typeof(iTunesServiceInfo), baseAddress);

            serviceHost.AddServiceEndpoint(typeof(IiTunesInfoService), new WSHttpBinding(), "iTunesServiceInfo");

            // Enable Mex

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(smb);

            try
            {
                // Run indefinitely:

                serviceHost.Open();
                LogAnEvent(serviceHost, "Started listening on port " + portNumber.ToString());
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    LogAnEvent(serviceHost, "Exception received by the web service: " + ex.ToString(), LogSeverity.Error);
                }
            }
            finally
            {
                LogAnEvent(serviceHost, "Stopped web listener");
                serviceHost.Abort();
            }
        }

        #endregion
    }
}
