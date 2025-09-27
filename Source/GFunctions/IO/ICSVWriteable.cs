namespace GFunctions.IO
{
    /// <summary>
    /// Generic definition for a class that can be saved to as csv file
    /// </summary>
    public interface ICSVWriteable
    {
        /// <summary>
        /// A list of data rows that can be saved to the csv file
        /// </summary>
        public List<string[]> DataRowFields { get; }

        /// <summary>
        /// The data column headers to be written to the csv file
        /// </summary>
        public string[] HeaderFields { get; }
    }
}
