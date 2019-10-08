// <copyright file="LoggingEventExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using log4net.Core;

    /// <summary>
    /// Extensions for <see cref="LoggingEvent"/>.
    /// </summary>
    public static class LoggingEventExtensions
    {
        /// <summary>
        /// Copies the data of the logging event into a new object.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        /// <returns>The copied data.</returns>
        /// <remarks>
        /// We must "fix" volatile data before we can access it. Some properties are more expensive to fix than others.
        /// For performance reasons, we just set the fix flags LocationInfo and UserName, if really required. See also:
        /// http://www.codewrecks.com/blog/index.php/2015/03/27/bufferingappenderskeleton-performance-problem-in-log4net/ .
        /// </remarks>
        public static LogEventData CopyData(this LoggingEvent loggingEvent)
        {
            var copy = new LogEventData();

            var fixFlags = loggingEvent.Level >= Level.Error ? FixFlags.All : FixFlags.Partial;
            LoggingEventData loggingEventData = loggingEvent.GetLoggingEventData(fixFlags);
            copy.Level = LogLevel.Of(loggingEventData.Level);
            copy.LoggerName = loggingEventData.LoggerName;
            copy.Domain = loggingEventData.Domain;
            copy.Message = loggingEventData.Message;
            copy.ThreadName = loggingEventData.ThreadName;
            copy.TimeStamp = loggingEventData.TimeStampUtc;
            copy.Properties = loggingEventData.Properties;

            if (loggingEvent.Level >= Level.Error)
            {
                copy.ExceptionString = loggingEventData.ExceptionString;
                copy.Identity = loggingEventData.Identity;
                copy.LogLocation = loggingEventData.LocationInfo.CopyData();
                copy.UserName = loggingEventData.UserName;
            }

            return copy;
        }

        private static LogLocation CopyData(this LocationInfo locationInfo)
        {
            return new LogLocation
            {
                ClassName = locationInfo.ClassName,
                FileName = locationInfo.FileName,
                FullInfo = locationInfo.FullInfo,
                LineNumber = locationInfo.LineNumber,
                MethodName = locationInfo.MethodName,
            };
        }
    }
}