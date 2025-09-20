namespace GFunctions.IO
{
    /// <summary>
    /// A general purpose class for logging data to a text file
    /// </summary>
    public class Logger
    {
        private readonly string _applicationPath = "";
        private readonly string _fileName = "Log File";
        private readonly string _folderName = "ErrorLogs";

        /// <summary>
        /// The extention of the log file
        /// </summary>
        public string FileExtention { get; set; } = ".txt";

        /// <summary>
        /// The folder path which the logger is creating files at
        /// </summary>
        public string FolderPath => Path.Combine(_applicationPath, _folderName);

        /// <summary>
        /// The file path which is being logged to
        /// </summary>
        public string FilePath => Path.Combine(FolderPath, $"{IOHelpers.DateStamp()} {_fileName}{FileExtention}");


        /// <summary>
        /// Initialize the logger
        /// </summary>
        /// <param name="applicationPath">The application root path</param>
        /// <param name="fileName">The name to use for log files</param>
        /// <param name="folderName">The folder to store log files</param>
        public Logger(string applicationPath, string fileName = "", string folderName = "")
        {
            try
            {
                _applicationPath = applicationPath;

                if (fileName != "")
                    _fileName = fileName;

                if (folderName != "")
                    _folderName = folderName;

                CreateFolder();
                WriteTextDatedBlock("Logger Initialized");
            }
            catch { }
        }

        /// <summary>
        /// Writes a line of text to the log file
        /// </summary>
        /// <param name="text">The text to write</param>
        protected void WriteTextLine(string text)
        {
            File.AppendAllText(FilePath, text + Environment.NewLine);
        }
        
        /// <summary>
        /// Writes a blank line to the log file
        /// </summary>
        protected void WriteBlankLine()
        {
            File.AppendAllText(FilePath, Environment.NewLine);
        }
        
        /// <summary>
        /// Writes a block to the log file, with a timestamp
        /// </summary>
        /// <param name="text">The text to include inside the block</param>
        protected void WriteTextDatedBlock(string text)
        {
            WriteTextLine("***************************************************************************************************************");
            WriteTextLine(IOHelpers.DateTimeStamp());
            WriteBlankLine();
            WriteTextLine(text);
            WriteTextLine("***************************************************************************************************************");
        }


        private void CreateFolder()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }
    }
}
