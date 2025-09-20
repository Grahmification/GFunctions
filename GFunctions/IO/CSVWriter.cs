namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify writing to csv files
    /// </summary>
    public class CSVWriter
    {
        private readonly StreamWriter _writer;

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
        /// Opens the csv file which will be written to
        /// </summary>
        /// <param name="folder">Folder containing the file</param>
        /// <param name="fileName">Name of the file</param>
        public CSVWriter(string folder, string fileName)
        {
            FileName = fileName;
            FilePath = Path.Combine(folder, fileName);

            _writer = new StreamWriter(FilePath);
        }

        /// <summary>
        /// Closes the csv file
        /// </summary>
        public void Close()
        {
            _writer.Close();
            _writer.Dispose();
        }

        /// <summary>
        /// Writes a line to the csv file
        /// </summary>
        /// <param name="lineItems">Fields in the line</param>
        public void WriteLine(string[] lineItems)
        {
            string line = string.Join(Delimiter, lineItems);

            _writer.WriteLine(line);
            _writer.Flush();
        }

        /// <summary>
        /// Writes multiple lines to the csv file
        /// </summary>
        /// <param name="lines">A list of lines containing fields</param>
        public void WriteLines(List<string[]> lines)
        {
            foreach (string[] line in lines)
            {
                WriteLine(line);
            }
        }
    }
}
