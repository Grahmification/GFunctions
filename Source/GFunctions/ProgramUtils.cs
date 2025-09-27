using GFunctions.IO;
using System.Diagnostics;

namespace GFunctions
{
    /// <summary>
    /// Contains various utilities
    /// </summary>
    public static class ProgramUtils
    {
        /// <summary>
        /// Determine if the program executable has been run in debug mode.
        /// </summary>
        static public bool IsDebugExecutable
        {
            get
            {
                bool isDebug = false;
                CheckDebugExecutable(ref isDebug);
                return isDebug;
            }
        }

        /// <summary>
        /// Returns true if the program executable has been run in debug mode.
        /// </summary>
        /// <param name="isDebug"></param>
        [Conditional("DEBUG")]
        static private void CheckDebugExecutable(ref bool isDebug) => isDebug = true;

        /// <summary>
        /// Determines if the executable is a duplicated (there's already another one running)
        /// </summary>
        /// <param name="appName">The name of the Executable file (no extension)</param>
        /// <returns>True if the app is a duplicate</returns>
        static public bool IsDuplicateExecutable(string appName)
        {
            _isDuplicateMutex = new Mutex(true, appName, out bool createdNew);
            return !createdNew;
        }

        /// <summary>
        /// Used for determining if a duplicate application is running
        /// </summary>
        static private Mutex? _isDuplicateMutex = null;

        /// <summary>
        /// Gets the version of the assembly which calls this method
        /// </summary>
        /// <returns>The version string (ex. 1.0.1)</returns>
        static public string GetAssemblyVersion(IExceptionLogger? logger = null)
        {
            // Try because the assembly may not have a version
            try
            {
                var assembly = System.Reflection.Assembly.GetCallingAssembly();
                var versInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}";
            }
            catch (Exception ex)
            {
                logger?.Log(ex);
                return "X.X.X";
            }
        }
    }
}
