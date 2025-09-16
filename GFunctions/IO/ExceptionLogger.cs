namespace GFunctions.IO
{
    /// <summary>
    /// Generic definition of an exception logging class
    /// </summary>
    public interface IExceptionLogger
    {
        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="ex">Exception to log</param>
        void Log(Exception ex);
    }

    /// <summary>
    /// Class for easily logging exceptions
    /// </summary>
    /// <param name="ApplicationPath">The root path of the application</param>
    public class ExceptionLogger(string ApplicationPath) : Logger(ApplicationPath), IExceptionLogger
    {
        /// <summary>
        /// Log the exception
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public void Log(Exception ex)
        {
            try
            {
                WriteTextDatedBlock(ex.ToString());
            }
            catch { }
        }
    }
}
