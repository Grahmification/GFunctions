﻿using System;
using System.IO;

namespace GFunctions.IO
{
    /// <summary>
    /// Helper functions for creating, reading and writing files
    /// </summary>
    public static class IOHelpers
    {
        /// <summary>
        /// Gets a date stamp string yy-mm-dd
        /// </summary>
        /// <param name="nowDateOverride">Override for the current date</param>
        /// <returns>The formatted date stamp string</returns>
        public static string DateStamp(DateTime? nowDateOverride = null)
        {
            if (!nowDateOverride.HasValue)
                nowDateOverride = DateTime.Now;

            return string.Format("{0}-{1}-{2}", nowDateOverride?.Year, nowDateOverride?.Month, nowDateOverride?.Day);
        }

        /// <summary>
        /// Gets a time stamp string xxhyymzzs
        /// </summary>
        /// <param name="nowDateOverride">Override for the current time</param>
        /// <returns>The formatted time stamp string</returns>
        public static string TimeStamp(DateTime? nowDateOverride = null)
        {
            if (!nowDateOverride.HasValue)
                nowDateOverride = DateTime.Now;

            return string.Format("{0}h{1}m{2}s", nowDateOverride?.Hour, nowDateOverride?.Minute, nowDateOverride?.Second);
        }

        /// <summary>
        /// Gets a date and time stamp string yy-mm-dd xxhyymzzs
        /// </summary>
        /// <param name="nowDateOverride">Override for the current time</param>
        /// <returns>The formatted date & time stamp string</returns>
        public static string DateTimeStamp(DateTime? nowDateOverride = null)
        {
            return string.Format("{0} {1}", DateStamp(nowDateOverride), TimeStamp(nowDateOverride));
        }

        /// <summary>
        /// Checks if a folder exists and creates a new one if not
        /// </summary>
        /// <param name="basePath">The path where to create the folder</param>
        /// <param name="folderName">The name of the folder</param>
        /// <returns>The created or existing folder's information</returns>
        public static DirectoryInfo CreateExportFolder(string basePath, string folderName)
        {
            var fullPath = Path.Combine(basePath, folderName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return new DirectoryInfo(fullPath);
        }

    }
}
