using System;
using System.Diagnostics;
using System.IO;

namespace NowPlaying.Model
{
    public class Logger
    {
        /// <summary>
        /// Gets the <see cref="Config"/> to use for this logger.
        /// </summary>
        public Configuration Config { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">The <see cref="Config"/> to use for this logger.</param>
        public Logger(Configuration config)
        {
            Config = config;
        }

        /// <summary>
        /// Log the given message to the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            AppendToLogFile(message);
        }

        /// <summary>
        /// Log the given <see cref="Exception"/> information to the log file.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public void Log(Exception ex)
        {
            AppendToLogFile(ex.ToString());
        }

        private void AppendToLogFile(string text)
        {
            try
            {
                var logMessage = $"[{DateTime.Now}] {text}\r\n";
                Trace.Write(logMessage);
                if (Config.EnableLogging)
                {
                    File.AppendAllText(Config.LogFilePath, logMessage);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
