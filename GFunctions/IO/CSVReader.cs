namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify reading from csv files
    /// </summary>
    public class CSVReader
    {
        private StreamReader? _reader = null;
        private string _fullPath = "";
        private bool _continueSearchFlag = true; //if file does not exist, program will keep searching until this is set to false
        private const int reCheckPeriod = 100; //refresh period in ms to check if a file exists

        /// <summary>
        /// Default constructor
        /// </summary>
        public CSVReader() { }

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="folder">Folder path containing the file</param>
        /// <param name="fileName">File name</param>
        public void open(string folder, string fileName)
        {
            _fullPath = Paths.BuildFullFilePath(fileName, folder);
            open(_fullPath);
        }

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="filePath">File path to the csv file</param>
        public void open(string filePath)
        {
            WaitForFile(filePath, reCheckPeriod);
            Thread.Sleep(reCheckPeriod * 2); //wait so no usage errors thrown

            if (_continueSearchFlag) //if false, user has cancelled search... don't initialize
            {
                _reader = new StreamReader(filePath);
            }
            else { return; }
        }

        /// <summary>
        /// Close the csv file
        /// </summary>
        /// <param name="deleteFile">Deletes the file if true</param>
        public void close(bool deleteFile = false)
        {
            if (_reader != null)
            {
                _reader.Close();

                if (deleteFile)
                {
                    File.Delete(_fullPath);
                }

            }
        }

        /// <summary>
        /// If waiting to open a csv file that doesn't exist, cancels the process
        /// </summary>
        public void cancelSearch()
        {
            _continueSearchFlag = false;
        }

        /// <summary>
        /// Reads the latest line from the csv file
        /// </summary>
        /// <returns>The complete line</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public string readline()
        {
            if (_reader == null)
                throw new IOException("Cannot read. No file is opened.");
            
            var line = _reader.ReadLine();
            return line ?? "";
        }

        /// <summary>
        /// Reads all lines from the csv file
        /// </summary>
        /// <returns>A list of lines in the file</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public string[] readAllLines()
        {
            if (_reader == null)
                throw new IOException("Cannot read. No file is opened.");

            List<string> lineList = [];

            while (!_reader.EndOfStream)
            {
                lineList.Add(_reader.ReadLine() ?? "");
            }

            return [.. lineList];
        }

        /// <summary>
        /// Splits a csv line into individual items
        /// </summary>
        /// <param name="line">The whole csv line</param>
        /// <returns>The individual fields</returns>
        public static string[] splitLine(string line)
        {
            return line.Split(',');
        }

        /// <summary>
        /// Hangs until a file exists
        /// </summary>
        /// <param name="fullPath">The file paths</param>
        /// <param name="reCheckPeriod">How often to re-check for the file [ms]</param>
        private void WaitForFile(string fullPath, int reCheckPeriod)
        {
            while (!File.Exists(fullPath) && _continueSearchFlag)
            {
                Thread.Sleep(reCheckPeriod);
            }
        }

    }
}
