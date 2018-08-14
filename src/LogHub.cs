// <copyright file="LogHub.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// The SignalR hub which forwards logged messages to connected and subscribing clients.
    /// </summary>
    public abstract class LogHub : Hub<ILogHubClient>
    {
        /// <summary>
        /// The default group name.
        /// </summary>
        internal const string DefaultGroup = "DefaultGroup";

        // TODO: instead of a linkedlist we could implement something like a ring buffer. Currently we might add a lot of pressure to the GC.
        private static readonly IDictionary<string, LinkedList<LogEntry>> CachedEntriesPerGroup =
            new ConcurrentDictionary<string, LinkedList<LogEntry>>();

        /// <summary>
        /// Gets or sets the maximum cached entries. Setting this to <c>0</c> deactivates the cache.
        /// </summary>
        public static int MaximumCachedEntries { get; set; }

        /// <summary>
        /// Subscribes to the log entries of this hub, by adding the connected client to the default group.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task SubscribeToDefaultGroup()
        {
            await this.SubscribeToGroup(DefaultGroup).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to the log entries of this hub, by adding the connected client to the specified group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task SubscribeToGroup(string groupName)
        {
            await this.SubscribeToGroupWithMessageOffset(groupName, 0).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes to the log entries of this hub, by adding the connected client to the specified group.
        /// This causes <see cref="ILogHubClient.Initialize"/> to be called with the available logger names
        /// and the cached log entries which have a higher id than <paramref name="idOfLastReceivedEntry"/>.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="idOfLastReceivedEntry">The identifier of last received entry.</param>
        /// <returns>The task.</returns>
        public async Task SubscribeToGroupWithMessageOffset(string groupName, long idOfLastReceivedEntry)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            var client = this.Clients.Client(this.Context.ConnectionId);
            var loggers = LogManager.GetAllRepositories().SelectMany(repo => repo.GetCurrentLoggers()).Select(log => log.Name).OrderBy(name => name).ToList();

            IList<LogEntry> cachedEntries;
            if (MaximumCachedEntries > 0)
            {
                // connecting should not happen often, so a lock is sufficient here
                var cache = GetCache(groupName);

                lock (cache)
                {
                    cachedEntries = cache.Where(entry => entry.Id > idOfLastReceivedEntry).ToList();
                }
            }
            else
            {
                cachedEntries = new List<LogEntry>();
            }

            await client.Initialize(loggers, cachedEntries);
        }

        /// <summary>
        /// Unsubscribes from the log entries of this hub by removing the connected client from the specified group.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>The task.</returns>
        public async Task UnsubscribeFromGroup(string groupName)
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Unsubscribes from the log entries of this hub by removing the connected client from the default group.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task UnsubscribeFromDefaultGroup() => await this.UnsubscribeFromGroup(DefaultGroup).ConfigureAwait(false);

        /// <summary>
        /// Called when a log entry got logged by the <see cref="SignalrAppender"/>.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns>The task.</returns>
        public async Task OnMessageLogged(LogEntry logEntry)
        {
            await this.OnMessageLogged(logEntry, DefaultGroup).ConfigureAwait(false);
        }

        /// <summary>
        /// Called when a log entry got logged by the <see cref="SignalrAppender" /> for the specified group.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task OnMessageLogged(LogEntry logEntry, string groupName)
        {
            if (MaximumCachedEntries > 0)
            {
                AddEntryToCache(logEntry, groupName);
            }

            await this.Clients.Group(groupName).OnLoggedEvent(logEntry.FormattedEvent, logEntry.LoggingEvent, logEntry.Id);
        }

        private static void AddEntryToCache(LogEntry logEntry, string groupName)
        {
            var cache = GetCache(groupName);
            lock (cache)
            {
                cache.AddLast(logEntry);
                if (cache.Count > MaximumCachedEntries)
                {
                    cache.RemoveFirst();
                }
            }
        }

        private static LinkedList<LogEntry> GetCache(string groupName)
        {
            if (!CachedEntriesPerGroup.TryGetValue(groupName, out var cache))
            {
                cache = new LinkedList<LogEntry>();
                CachedEntriesPerGroup.Add(groupName, cache);
            }

            return cache;
        }
    }
}