// <copyright file="LogLevel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    using System.Collections.Generic;
    using System.Threading;
    using log4net.Core;

    /// <summary>
    /// The log level.
    /// </summary>
    public class LogLevel
    {
        /// <summary>
        /// The cached <see cref="LogLevel"/> instances for each <see cref="Level"/>.
        /// We cache them, so we don't create tons of these instances.
        /// </summary>
        private static readonly IDictionary<Level, LogLevel> CachedLevels = new Dictionary<Level, LogLevel>();

        private static readonly ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLevel"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        protected LogLevel(Level level)
        {
            this.DisplayName = level.DisplayName;
            this.Name = level.Name;
            this.Value = level.Value;
        }

        /// <summary>
        /// Gets the display name of the log level.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the name of the log level.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the numerical value of the log level.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Gets the instance of <see cref="LogLevel"/> of the specified <see cref="Level"/>.
        /// </summary>
        /// <param name="level">The <see cref="Level"/>.</param>
        /// <returns>The <see cref="LogLevel"/>.</returns>
        public static LogLevel Of(Level level)
        {
            LockSlim.EnterUpgradeableReadLock();
            try
            {
                if (!CachedLevels.TryGetValue(level, out var result))
                {
                    LockSlim.EnterWriteLock();
                    try
                    {
                        result = new LogLevel(level);
                        CachedLevels.Add(level, result);
                    }
                    finally
                    {
                        LockSlim.ExitWriteLock();
                    }
                }

                return result;
            }
            finally
            {
                LockSlim.ExitUpgradeableReadLock();
            }
        }
    }
}