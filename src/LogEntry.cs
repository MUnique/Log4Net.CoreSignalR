// <copyright file="LogEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    /// <summary>
    /// A log entry which is sent to the client.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="id">The unique identifier which is a sequence number.</param>
        /// <param name="formattedEvent">The formtted event as configured in the settings.</param>
        /// <param name="loggingEvent">The logging event data.</param>
        public LogEntry(long id, string formattedEvent, LogEventData loggingEvent)
        {
            this.Id = id;
            this.FormattedEvent = formattedEvent;
            this.LoggingEvent = loggingEvent;
        }

        /// <summary>
        /// Gets or sets the unique identifier (sequence number).
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the event as formatted text as configured in the settings.
        /// </summary>
        public string FormattedEvent { get; set; }

        /// <summary>
        /// Gets or sets the logging event data.
        /// </summary>
        public LogEventData LoggingEvent { get; set; }
    }
}