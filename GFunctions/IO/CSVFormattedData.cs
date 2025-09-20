namespace GFunctions.IO
{
    /// <summary>
    /// Formatted data which can be written to or read from a csv file
    /// </summary>
    public class CSVFormattedData
    {
        /// <summary>
        /// The delimiter for items in the file
        /// </summary>
        public const char Delimiter = ',';

        /// <summary>
        /// CSV Line marking the end of the metadata region header
        /// </summary>
        public const string HeaderEndString = "#### DATA STARTS HERE ####";

        // ----------------------- Public Properties ---------------------------

        /// <summary>
        /// Metadata fields at the top of the csv file
        /// </summary>
        public List<CSVMetaData> MetaData { get; set; } = [];

        /// <summary>
        /// Contains the data column headers
        /// </summary>
        public string[] DataHeaders { get; private set; } = [];

        /// <summary>
        /// Contains the list of data, with each row as a list item
        /// </summary>
        public List<string[]> Data { get; private set; } = [];

        // ----------------------- Public Methods ---------------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public CSVFormattedData() { }

        /// <summary>
        /// Construct from data classes
        /// </summary>
        /// <param name="data">List of data items to write</param>
        /// <param name="metaData">Metadata fields to go at the top of the file</param>
        /// <exception cref="DataMisalignedException"></exception>
        public CSVFormattedData(List<ICSVWriteable> data, List<CSVMetaData> metaData)
        {
            MetaData = metaData;
            DataHeaders = data[0].HeaderFields;

            foreach (ICSVWriteable item in data)
            {
                if (item.HeaderFields != DataHeaders)
                    throw new DataMisalignedException("All data items must have the same column headers.");

                // Get all lines contained in the item
                foreach (var row in item.DataRowFields)
                {
                    Data.Add(row);
                }
            }
        }

        /// <summary>
        /// Writes the class to a csv file
        /// </summary>
        /// <param name="writer">Writer to write the file</param>
        public void WriteToFile(CSVWriter writer)
        {
            writer.Delimiter = Delimiter;

            // Write all the metaData
            foreach (var item in MetaData)
            {
                writer.WriteLine([item.Name, item.Value]);
            }

            // Write the metadata end line
            writer.WriteLine([HeaderEndString]);

            // Write the data column headers
            writer.WriteLine(DataHeaders);
            writer.WriteLines(Data);

            writer.Close();
        }

        /// <summary>
        /// Reads in formatted data from a csv file
        /// </summary>
        /// <param name="reader">Reader with an open file</param>
        /// <returns>Data in the file</returns>
        /// <exception cref="IOException">No file is opened</exception>
        /// <exception cref="FormatException">The file is formatted incorrectly</exception>
        public static CSVFormattedData GetFromFile(CSVReader reader)
        {
            reader.Delimiter = Delimiter;

            CSVFormattedData output = new();

            // Metadata must be read first
            output.MetaData = ReadMetaData(reader);

            // The next line is the data column headers
            output.DataHeaders = reader.ReadLine();

            // Following lines are all data
            output.Data = reader.ReadAllLines();

            reader.Close();

            return output;
        }

        /// <summary>
        /// Reads the metadata fields for a formatted csv, including the header end line
        /// </summary>
        /// /// <param name="reader">CSV reader class</param>
        /// <returns>List of metadata rows [name, value]</returns>
        /// <exception cref="IOException">No file is opened</exception>
        /// <exception cref="FormatException">The file is formatted incorrectly</exception>
        private static List<CSVMetaData> ReadMetaData(CSVReader reader)
        {
            List<CSVMetaData> lines = [];

            string[] line = [""];

            // Read until we hit the end string
            while (line[0] != HeaderEndString)
            {
                if (!reader.MoreLinesToRead)
                    throw new FormatException($"Reached end of file and did not find header string <{HeaderEndString}>. The file is improperly formatted.");
                
                line = reader.ReadLine();
                var name = line[0]; // We're always guaranteed to get at lead one item from readline
                var value = line.Length > 1 ? line[1] : "";

                lines.Add(new CSVMetaData(name, value));
            }

            return lines;
        }
    }

    /// <summary>
    /// Metadata which goes at the top of a csv file
    /// </summary>
    /// <param name="name">Name of the field</param>
    /// <param name="value">The value</param>
    public class CSVMetaData(string name, string value)
    {
        /// <summary>
        /// The field name
        /// </summary>
        public string Name { get; set; } = name;

        /// <summary>
        /// The field value
        /// </summary>
        public string Value { get; set; } = value;
    }
}
