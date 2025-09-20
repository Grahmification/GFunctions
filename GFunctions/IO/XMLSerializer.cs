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
        /// <param name="fileName">The file name to load</param>
        /// <param name="folderPath">An optional folder path</param>
        /// <returns>The instance of the class in the file, or an empty version of the class</returns>
        public static T? Load(string fileName, string folderPath = "")
        {
            var serializer = new XmlSerializer(typeof(T));

            T? output = default; //initialize to default

            if (File.Exists(Paths.BuildFullFilePath(fileName, folderPath))) //file does exist, load config from file
            {
                using (var fStream = new FileStream(Paths.BuildFullFilePath(fileName, folderPath), FileMode.Open))
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

            using (var fStream = new FileStream(Paths.BuildFullFilePath(fileName, folderPath), FileMode.Create))
            {
                serializer.Serialize(fStream, saveClass);
            }
        }
    }
}
