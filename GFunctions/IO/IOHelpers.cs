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

            return $"{nowDateOverride?.Year}-{nowDateOverride?.Month}-{nowDateOverride?.Day}";
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

            return $"{nowDateOverride?.Hour}h{nowDateOverride?.Minute}m{nowDateOverride?.Second}s";
        }

        /// <summary>
        /// Gets a date and time stamp string yy-mm-dd xxhyymzzs
        /// </summary>
        /// <param name="nowDateOverride">Override for the current time</param>
        /// <returns>The formatted date and time stamp string</returns>
        public static string DateTimeStamp(DateTime? nowDateOverride = null)
        {
            return $"{DateStamp(nowDateOverride)} {TimeStamp(nowDateOverride)}";
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

        /// <summary>
        /// Prepare a path for saving a file. Create folders if needed and change the name to prevent duplicates
        /// </summary>
        /// <param name="filePath">The full filepath</param>
        /// <param name="validateExtension">Check if the file has a valid extension (".csv")</param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string PrepareSaveFilePath(string filePath, string validateExtension = "")
        {
            var file = new FileInfo(filePath);
            var folder = file.Directory;
            var fileName = file.Name;

            if (folder == null)
            {
                throw new NullReferenceException($"The file path {filePath} is invalid");
            }

            if (!folder.Exists)
                folder.Create();

            if (validateExtension != "" && file.Extension != validateExtension)
                throw new FormatException($"File extension is not {validateExtension}.");

            return Path.Combine(folder.FullName, $"{fileName}");
        }

    }
}
