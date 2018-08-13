// <copyright file="LogLocation.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.Log4Net.CoreSignalR
{
    /// <summary>
    /// The log location data.
    /// </summary>
    public class LogLocation
    {
        /// <summary>
        /// Gets or sets the name of the class where the log event happened.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the name of the file where the log event happened.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the full information.
        /// </summary>
        public string FullInfo { get; set; }

        /// <summary>
        /// Gets or sets the line number where the log event happened.
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the method where the log event happened.
        /// </summary>
        public string MethodName { get; set; }
    }
}