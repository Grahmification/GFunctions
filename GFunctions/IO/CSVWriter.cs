namespace GFunctions.IO
{
    /// <summary>
    /// Class to simplify writing to csv files
    /// </summary>
    public class CSVWriter
    {
        private readonly StreamWriter _writer;

        /// <summary>
        /// Opens the csv file which will be written to
        /// </summary>
        /// <param name="folder">Folder containing the file</param>
        /// <param name="fileName">Name of the file</param>
        public CSVWriter(string folder, string fileName)
        {
            _writer = new StreamWriter(Paths.BuildFullFilePath(fileName, folder));
        }

        /// <summary>
        /// Closes the csv file
        /// </summary>
        public void close()
        {
            _writer.Close();
        }

        /// <summary>
        /// Writes a line to the csv file
        /// </summary>
        /// <param name="lineItems">Fields in the line</param>
        public void writeLine(string[] lineItems)
        {
            string line = string.Join(",", lineItems);

            _writer.WriteLine(line);
            _writer.Flush();
        }

        /// <summary>
        /// Writes multiple lines to the csv file
        /// </summary>
        /// <param name="lines">A list of lines containing fields</param>
        public void writeLines(List<string[]> lines)
        {
            foreach (string[] line in lines)
            {
                string joinedLine = string.Join(",", line);

                _writer.WriteLine(joinedLine);
                _writer.Flush();
            }
        }
    }
}
