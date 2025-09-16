namespace GFunctions.IO
{
    /// <summary>
    /// Methods for working with file paths
    /// </summary>
    public class Paths
    {
        /// <summary>
        /// Returns the filename, or optionally with the folder as a full path if specifed
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <param name="folderPath">The optional folder path</param>
        /// <returns>The filename, or full path with folder</returns>
        public static string BuildFullFilePath(string fileName, string folderPath = "")
        {
            if (folderPath == "")
            {
                return fileName;
            }
            else
            {
                return @$"{folderPath}\{fileName}";
            }
        }
    }
}
