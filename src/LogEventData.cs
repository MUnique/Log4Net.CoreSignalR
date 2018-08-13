// <copyright file="LogEventData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using System;
    using log4net.Util;

    /// <summary>
    /// Class which holds data about a <see cref="log4net.Core.LoggingEvent"/>.
    /// </summary>
    public class LogEventData
    {
        /// <summary>
        /// Gets or sets the AppDomain friendly name.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the exception string.
        /// </summary>
        public string ExceptionString { get; set; }

        /// <summary>
        /// Gets or sets the identity of the current thread principal.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Gets or sets the level of the logging event.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the location information of the logging event.
        /// </summary>
        public LogLocation LogLocation { get; set; }

        /// <summary>
        /// Gets or sets the name of the logger that logged the event.
        /// </summary>
        public string LoggerName { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets additional event specific properties.
        /// </summary>
        public PropertiesDictionary Properties { get; set; }

        /// <summary>
        /// Gets or sets the name of the thread what logged the event.
        /// </summary>
        public string ThreadName { get; set; }

        /// <summary>
        /// Gets or sets the time stamp of the event.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the (windows) user.
        /// </value>
        public string UserName { get; set; }
    }
}