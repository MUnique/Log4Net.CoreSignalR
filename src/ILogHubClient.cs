// <copyright file="ILogHubClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Client interface for the <see cref="LogHub"/>.
    /// </summary>
    public interface ILogHubClient
    {
        /// <summary>
        /// Called when a log entry got logged by the <see cref="SignalrAppender"/>.
        /// </summary>
        /// <param name="formattedEvent">The formatted event.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>The task.</returns>
        Task OnLoggedEvent(string formattedEvent, LogEventData entry, long id);

        /// <summary>
        /// Initializes the client with the available loggers and cached log entries.
        /// </summary>
        /// <param name="loggers">The loggers.</param>
        /// <param name="cachedEntries">The cached entries.</param>
        /// <returns>The task.</returns>
        Task Initialize(IList<string> loggers, IList<LogEntry> cachedEntries);
    }
}