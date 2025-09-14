namespace GFunctions.IO
{
    public class CSVWriter
    {
        private StreamWriter _writer = null;

        public CSVWriter(string folder, string fileName)
        {
            string fullPath = folder + @"\" + fileName;
            _writer = new StreamWriter(fullPath);
        }

        public void close()
        {
            _writer.Close();
        }

        public void writeLine(string[] lineItems)
        {
            string line = String.Join(",", lineItems);

            _writer.WriteLine(line);
            _writer.Flush();
        }

        public void writeLines(List<string[]> lines)
        {
            foreach (string[] line in lines)
            {
                string joinedLine = String.Join(",", line);

                _writer.WriteLine(joinedLine);
                _writer.Flush();
            }

        }

    }
}
