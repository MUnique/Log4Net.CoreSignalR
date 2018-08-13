// <copyright file="SignalrAppender.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using Microsoft.AspNetCore.SignalR.Client;

    /// <summary>
    /// A log4net appender which logs to the <see cref="LogHub"/>.
    /// </summary>
    /// <seealso cref="log4net.Appender.AppenderSkeleton" />
    public class SignalrAppender : AppenderSkeleton
    {
        private HubConnection hubConnection;
        private ConnectionState connectionState = ConnectionState.Closed;
        private long currentId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalrAppender"/> class.
        /// </summary>
        public SignalrAppender()
        {
            this.GroupName = LogHub.DefaultGroup;
        }

        /// <summary>
        /// Gets or sets the hub URL.
        /// </summary>
        public string HubUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the group in the hub to which this appender is supposed to log.
        /// </summary>
        public string GroupName { get; set; }

        /// <inheritdoc />
        protected override void Append(LoggingEvent loggingEvent)
        {
            var formattedEvent = this.RenderLoggingEvent(loggingEvent);
            var id = Interlocked.Increment(ref this.currentId);
            var logEntry = new LogEntry(id, formattedEvent, loggingEvent.CopyData());

            this.SendLogEntry(logEntry);
        }

        /// <inheritdoc />
        protected override void OnClose()
        {
            base.OnClose();
            if (this.hubConnection != null)
            {
                this.hubConnection.StopAsync().Wait();
                this.hubConnection = null;
            }
        }

        private void EnsureConnection()
        {
            if (this.hubConnection == null)
            {
                if (string.IsNullOrEmpty(this.HubUrl))
                {
                    LogManager.GetLogger(this.GetType()).Error($"{nameof(this.HubUrl)} needs to be configured.");
                }

                this.hubConnection = new HubConnectionBuilder().WithUrl(this.HubUrl).Build();
                this.hubConnection.Closed += this.HubConnectionClosed;
                this.hubConnection
                    .StartAsync()
                    .ContinueWith(task => this.connectionState = ConnectionState.Open)
                    .Wait();
            }
        }

        private Task HubConnectionClosed(Exception arg)
        {
            this.hubConnection = null;
            this.connectionState = ConnectionState.Closed;
            return Task.CompletedTask;
        }

        private void SendLogEntry(LogEntry entry)
        {
            try
            {
                this.EnsureConnection();
                if (this.hubConnection != null && this.connectionState == ConnectionState.Open)
                {
                    this.hubConnection.SendAsync(nameof(LogHub.OnMessageLogged), entry, this.GroupName);
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger(this.GetType()).Warn("OnMessageLogged Failed:", e);
            }
        }
    }
}