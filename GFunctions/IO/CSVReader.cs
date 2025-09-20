namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify reading from csv files
    /// </summary>
    public class CSVReader
    {
        private StreamReader? _reader = null;
        private bool _continueSearchFlag = true; // If the file does not exist, program will keep searching until this is set to false
        private const int reCheckPeriod = 100; //refresh period in ms to check if a file exists

        /// <summary>
        /// The delimiter for items in the file
        /// </summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>
        /// Full path of the file being read from
        /// </summary>
        public string FilePath { get; private set; } = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CSVReader() { }

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="folder">Folder path containing the file</param>
        /// <param name="fileName">File name</param>
        public async Task Open(string folder, string fileName)
        {
            FilePath = Path.Combine(folder, fileName);
            await Open(FilePath);
        }

        /// <summary>
        /// Open a csv file, waiting if it does not exist
        /// </summary>
        /// <param name="filePath">File path to the csv file</param>
        public async Task Open(string filePath)
        {
            _continueSearchFlag = true;

            // Wait until the file exists
            while (!File.Exists(filePath) && _continueSearchFlag)
            {
                await Task.Delay(reCheckPeriod);
            }

            await Task.Delay(reCheckPeriod * 2); // Wait so no usage errors thrown

            if (_continueSearchFlag) // If false, user has cancelled search... don't initialize
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

                if (deleteFile)
                {
                    File.Delete(FilePath);
                }

            }
        }

        /// <summary>
        /// If waiting to open a csv file that doesn't exist, cancels the process
        /// </summary>
        public void CancelSearch()
        {
            _continueSearchFlag = false;
        }

        /// <summary>
        /// Reads the latest line from the csv file
        /// </summary>
        /// <returns>The complete line</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public string[] ReadLine()
        {
            if (_reader == null)
                throw new IOException("Cannot read. No file is opened.");
            
            var line = _reader.ReadLine();
            return SplitLine(line ?? "");
        }

        /// <summary>
        /// Reads all lines from the csv file
        /// </summary>
        /// <returns>A list of lines in the file</returns>
        /// <exception cref="IOException">No file is opened</exception>
        public List<string[]> ReadAllLines()
        {
            if (_reader == null)
                throw new IOException("Cannot read. No file is opened.");

            List<string[]> lines = [];

            while (!_reader.EndOfStream)
            {
                lines.Add(SplitLine(_reader.ReadLine() ?? ""));
            }

            return lines;
        }

        /// <summary>
        /// Splits a csv line into individual items
        /// </summary>
        /// <param name="line">The whole csv line</param>
        /// <returns>The individual fields</returns>
        private string[] SplitLine(string line)
        {
            return line.Split(Delimiter);
        }
    }
}
