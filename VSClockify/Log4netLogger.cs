using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSClockify
{
    public static class Log4netLogger<T>
    {
        #region Private Declarations

        /// <summary>
        /// Private static property _log.
        /// </summary>
        private static log4net.ILog _log { get; set; }

        /// <summary>
        /// Private static property _rand
        /// </summary>
        private static Random _random { get; set; }

        #endregion

        /// <summary>
        /// Static Constructor.
        /// </summary>
        static Log4netLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
            _log = log4net.LogManager.GetLogger(typeof(T));
            _random = new Random();
        }

        public static string LogError(string message, Exception ex)
        {
            var random = _random.Next(100000).ToString();
            var errorMessage = string.Format("System Error Occured({0})", random);
            _log.Error(string.Format("Error: " + message + " {0}: ", random), ex);
            return errorMessage;
        }
        public static void LogInfo(string message)
        {

            _log.Info(("Info: " + message));

        }
    }
}
