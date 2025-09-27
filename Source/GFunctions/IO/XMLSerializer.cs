using System.Xml.Serialization;

namespace GFunctions.IO
{
    /// <summary>
    /// Allows easily reading and writing data to an xml file
    /// </summary>
    /// <typeparam name="T">The class to read or write from the file</typeparam>
    public class XMLSerializer<T>
    {
        /// <summary>
        /// Load a class from a file
        /// </summary>
        /// <param name="folderPath">The folder path</param>
        /// <param name="fileName">The file name to load</param>
        /// <returns>The instance of the class in the file, or an empty version of the class</returns>
        public static T? Load(string folderPath, string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));

            T? output = default; //initialize to default
            var filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath)) //file does exist, load config from file
            {
                using (var fStream = new FileStream(filePath, FileMode.Open))
                    output = (T?)serializer.Deserialize(fStream);
            }

            return output;
        }

        /// <summary>
        /// Saves a class to an xml file
        /// </summary>
        /// <param name="folderPath">An optional folder path</param>
        /// <param name="fileName">The file name to save</param>
        /// <param name="saveClass">The class data to save</param>
        public static void Save(string folderPath, string fileName, T saveClass)
        {
            var serializer = new XmlSerializer(typeof(T));

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); //create the settings folder if it doesn't exist

            using (var fStream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create))
            {
                serializer.Serialize(fStream, saveClass);
            }
        }
    }
}
