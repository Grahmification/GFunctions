namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify reading from csv files
    /// </summary>
    public class CSVReader
    {
        private StreamReader? _reader = null;
        private bool _continueWaitFlag = true; // If the file does not exist, program will keep searching until this is set to false
        private const int _reCheckPeriod = 100; // Refresh period in ms to check if a file exists

        /// <summary>
        /// The delimiter for items in the file
        /// </summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>
        /// Full path of the file being read from
        /// </summary>
        public string FilePath { get; private set; } = string.Empty;

        /// <summary>
        /// True if the class is waiting to open a file
        /// </summary>
        public bool WaitingForFile { get; private set; } = false;

        /// <summary>
        /// Returns true of the reader has a file open
        /// </summary>
        public bool IsFileOpen => _reader != null;

        /// <summary>
        /// Returns true if there are more lines that can be read
        /// </summary>
        public bool MoreLinesToRead => !(_reader?.EndOfStream ?? false);

        // ------------------------- Public Methods ---------------------------------

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="folder">Folder path containing the file</param>
        /// <param name="fileName">File name</param>
        /// <param name="waitForFile">If true, the method will hang until the file exists or the search is cancelled</param>
        public async Task Open(string folder, string fileName, bool waitForFile = false)
        {
            FilePath = Path.Combine(folder, fileName);
            await Open(FilePath, waitForFile);
        }

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="filePath">File path to the csv file</param>
        /// <param name="waitForFile">If true, the method will hang until the file exists or the search is cancelled</param>
        public async Task Open(string filePath, bool waitForFile = false)
        {
            _reader = null;
            
            if (waitForFile)
                await WaitForFile(filePath, _reCheckPeriod);
            
            if (_continueWaitFlag) // If false, user has cancelled search... don't initialize
            {
                _reader = new StreamReader(filePath);
            }
        }

        /// <summary>
        /// Close the csv file
        /// </summary>
        /// <param name="deleteFile">Deletes the file if true</param>
        public void Close(bool deleteFile = false)
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;

                if (deleteFile)
                {
                    File.Delete(FilePath);
                }
            }
        }

        /// <summary>
        /// If waiting to open a csv file that doesn't exist, cancels the process
        /// </summary>
        public void CancelWaitForFile()
        {
            _continueWaitFlag = false;
        }

        /// <summary>
        /// Reads the latest line from the csv file
        /// </summary>
        /// <returns>The complete line</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public string[] ReadLine()
        {
            if (!IsFileOpen)
                throw new IOException("Cannot read. No file is opened.");
            
            var line = _reader?.ReadLine();
            return SplitLine(line ?? "");
        }

        /// <summary>
        /// Reads all unread lines from the csv file
        /// </summary>
        /// <returns>A list of lines in the file</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public List<string[]> ReadAllLines()
        {
            if (!IsFileOpen)
                throw new IOException("Cannot read. No file is opened.");

            List<string[]> lines = [];

            while (MoreLinesToRead)
            {
                lines.Add(SplitLine(_reader?.ReadLine() ?? ""));
            }

            return lines;
        }

        // ------------------------- Private Helpers ---------------------------------

        /// <summary>
        /// Splits a csv line into individual items
        /// </summary>
        /// <param name="line">The whole csv line</param>
        /// <returns>The individual fields</returns>
        private string[] SplitLine(string line)
        {
            return line.Split(Delimiter);
        }

        /// <summary>
        /// Hangs until a file exists
        /// </summary>
        /// <param name="fullPath">The file paths</param>
        /// <param name="reCheckPeriod">How often to re-check for the file [ms]</param>
        private async Task WaitForFile(string fullPath, int reCheckPeriod)
        {
            WaitingForFile = true;
            _continueWaitFlag = true;

            while (!File.Exists(fullPath) && _continueWaitFlag)
            {
                await Task.Delay(reCheckPeriod);
            }

            WaitingForFile = false;
            await Task.Delay(reCheckPeriod * 2); // Wait so no usage errors thrown
        }
    }
}
