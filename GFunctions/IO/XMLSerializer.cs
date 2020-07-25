using System.Xml.Serialization;
using System.IO;


namespace GFunctions.IO
{
    public class XMLSerializer<T>
    {
        public static T Load(string fileName, string folderPath = "")
        {
            var serializer = new XmlSerializer(typeof(T));

            T output = default; //initialize to default

            if (File.Exists(buildfullFilePath(folderPath, fileName))) //file does exist, load config from file
            {
                using (var fStream = new FileStream(buildfullFilePath(folderPath, fileName), FileMode.Open))
                    output = (T)serializer.Deserialize(fStream);
            }

            return output;
        }

        public static void Save(string folderPath, string fileName, T saveClass)
        {
            var serializer = new XmlSerializer(typeof(T));

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); //create the settings folder if it doesn't exist

            using (var fStream = new FileStream(buildfullFilePath(folderPath, fileName), FileMode.Create))
            {
                serializer.Serialize(fStream, saveClass);
            }
        }

        private static string buildfullFilePath(string folderPath, string fileName)
        {
            if (folderPath == "")
            {
                return fileName;
            }
            else
            {
                return folderPath + @"\" + fileName;
            }
        }
    }

}
