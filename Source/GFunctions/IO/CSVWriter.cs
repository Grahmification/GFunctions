namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify writing to csv files
    /// </summary>
    public class CSVWriter
    {
        private StreamWriter? _writer = null;

        /// <summary>
        /// The delimiter for items in the file
        /// </summary>
        public char Delimiter { get; set; } = ',';

        /// <summary>
        /// File name of the file being written to
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Full path of the file being written to
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Returns true of the writer has a file open
        /// </summary>
        public bool IsFileOpen => _writer != null;

        /// <summary>
        /// Opens the csv file which will be written to
        /// </summary>
        /// <param name="folder">Folder containing the file</param>
        /// <param name="fileName">Name of the file</param>
        public CSVWriter(string folder, string fileName)
        {
            FileName = fileName;
            FilePath = IOHelpers.PrepareSaveFilePath(Path.Combine(folder, fileName), ".csv");

            _writer = new StreamWriter(FilePath);
        }

        /// <summary>
        /// Closes the csv file
        /// </summary>
        public void Close()
        {
            _writer?.Close();
            _writer?.Dispose();
            _writer = null;
        }

        /// <summary>
        /// Writes a line to the csv file
        /// </summary>
        /// <param name="lineItems">Fields in the line</param>
        /// <exception cref="IOException">No file is open.</exception>
        public void WriteLine(string[] lineItems)
        {
            if (!IsFileOpen)
                throw new IOException("Cannot write to csv file, no file is open.");

            string line = string.Join(Delimiter, lineItems);

            _writer?.WriteLine(line);
            _writer?.Flush();
        }

        /// <summary>
        /// Writes multiple lines to the csv file
        /// </summary>
        /// <param name="lines">A list of lines containing fields</param>
        /// <exception cref="IOException">No file is open.</exception>
        public void WriteLines(List<string[]> lines)
        {
            foreach (string[] line in lines)
            {
                WriteLine(line);
            }
        }
    }
}
