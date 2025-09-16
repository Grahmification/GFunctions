using System.Xml.Serialization;

namespace GFunctions.IO
{
    /// <summary>
    /// A class for saving and loading program settings to a file
    /// </summary>
    public class Config
    {
        // ---------------------------------------------------------------------------------------

        /// <summary>
        /// Do not directly set this - for serializer
        /// </summary>
        public List<string> settingNames = [];

        /// <summary>
        /// Do not directly set this - for serializer
        /// </summary>
        public List<object> settingValues = [];

        // ---------------------------------------------------------------------------------------   

        /// <summary>
        /// The default relative folder for saving settings
        /// </summary>
        [XmlIgnore]
        public const string DefaultFolderPath = "Program Settings";

        /// <summary>
        /// Name of the configuration file
        /// </summary>
        [XmlIgnore]
        public const string FileName = "config.xml";

        /// <summary>
        /// Name of the default configuration file.
        /// </summary>
        [XmlIgnore]
        public const string FileNameDefault = "config_default.xml";

        /// <summary>
        /// An optional folder path if desired to save config files at a location other than with the application, can be absolute or relative
        /// </summary>
        [XmlIgnore]
        public string FolderPath { get; private set; } = "";

        /// <summary>
        /// Globally accessable instance of the loaded configuration
        /// </summary>
        [XmlIgnore]
        public static Config? Instance { get; private set; }

        // ---------------------------------------------------------------------------------------   

        /// <summary>
        /// Empty constructor for XmlSerializer
        /// </summary>
        public Config() { }

        /// <summary>
        /// Checks if config data exists
        /// </summary>
        /// <param name="folderPath">Optional folder path to check, defaults to <see cref="DefaultFolderPath"/></param>
        /// <returns></returns>
        public static bool DataExists(string folderPath = DefaultFolderPath)
        {
            return File.Exists(Paths.BuildFullFilePath(FileName, folderPath)) && File.Exists(Paths.BuildFullFilePath(FileNameDefault,folderPath));
        }

        /// <summary>
        /// Loads the configuration from file.
        /// </summary>
        /// <param name="folderPath">Optional folder path to check, defaults to <see cref="DefaultFolderPath"/></param>
        public static void Load(string folderPath = DefaultFolderPath)
        {
            LoadDoWork(FileName, folderPath);
        }

        /// <summary>
        /// Loads the default configuration from file.
        /// </summary>
        /// <param name="folderPath">Optional folder path to check, defaults to <see cref="DefaultFolderPath"/></param>
        public static void LoadDefault(string folderPath = DefaultFolderPath)
        {
            LoadDoWork(FileNameDefault, folderPath);
        }

        /// <summary>
        /// Loads a configuration file - Called by both functions
        /// </summary>
        /// <param name="fileName">The filename to load</param>
        /// <param name="folderPath">Optional folder path to check</param>
        private static void LoadDoWork(string fileName, string folderPath = "")
        {
            var serializer = new XmlSerializer(typeof(Config));

            if (!File.Exists(Paths.BuildFullFilePath(fileName, folderPath))) //file does not exist, no previous configuration has been saved
            {
                Instance = new Config
                {
                    FolderPath = folderPath
                };
            }
            else //file does exist, load config from file
            {
                using (var fStream = new FileStream(Paths.BuildFullFilePath(fileName, folderPath), FileMode.Open))
                    Instance = (Config?)serializer.Deserialize(fStream);

                if (Instance != null )
                    Instance.FolderPath = folderPath;
            }
        }

        /// <summary>
        /// Saves the configuration to a file.
        /// </summary>
        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));

            if (!Directory.Exists(this.FolderPath))
                Directory.CreateDirectory(this.FolderPath); //create the settings folder if it doesn't exist


            if (!File.Exists(Paths.BuildFullFilePath(FileName, FolderPath))) //file does not exist, create default settings file on first save
            {
                using (var fStreamDefault = new FileStream(Paths.BuildFullFilePath(FileNameDefault, FolderPath), FileMode.Create))
                    serializer.Serialize(fStreamDefault, this);
            }

            using (var fStream = new FileStream(Paths.BuildFullFilePath(FileName, FolderPath), FileMode.Create))
            {
                serializer.Serialize(fStream, this);
            }
        }

        // --------------------------------------------------------------------------------------- 

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">Setting value. Can be types: string, double, int, or Lists of each</param>
        public void AddSetting(string settingName, object value)
        {
            if (!settingNames.Contains(settingName)) //only add if the setting name doesn't exist
            {
                settingNames.Add(settingName);
                value = CheckInput(value); //check for correct setting type
                settingValues.Add(value);
            }
        }

        /// <summary>
        /// Attempts to delete a setting
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        public void DeleteSetting(string settingName)
        {
            if (settingNames.Contains(settingName)) //only remove if the setting name exists
            {
                int index = settingNames.IndexOf(settingName);
                settingNames.RemoveAt(index);
                settingValues.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks if a setting exists
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns></returns>
        public bool SettingExists(string settingName)
        {
            return settingNames.Contains(settingName);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">Setting value. Can be types: string, double, int, or Lists of each</param>
        /// <exception cref="Exception">The setting didn't exist</exception>
        private void SetSetting(string settingName, object value)
        {
            if (!settingNames.Contains(settingName)) //throw error if it doesn't exists
                throw new Exception("Tried to set setting that doesn't exist: " + settingName);

            value = CheckInput(value); //check for correct setting type

            int index = settingNames.IndexOf(settingName);
            settingValues[index] = value;
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, string value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, int value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, double value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, List<string> value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, List<int> value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Sets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <param name="value">The setting value</param>
        public void SetSetting(string settingName, List<double> value)
        {
            SetSetting(settingName, (object)value);
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value as an object</returns>
        /// <exception cref="Exception">The setting didn't exist</exception>
        private object GetSetting(string settingName)
        {
            if (!settingNames.Contains(settingName)) //throw error if it doesn't exists
                throw new Exception("Tried to get setting that doesn't exist: " + settingName);

            int index = settingNames.IndexOf(settingName);
            return settingValues[index];
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public int GetSettingInt(string settingName)
        {
            var value = GetSetting(settingName);

            if (value == null)
                return 0;
            else
                return (int)value;
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public double GetSettingDouble(string settingName)
        {
            var value = GetSetting(settingName);

            if (value == null)
                return 0;
            else
                return (double)value;
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public string GetSettingString(string settingName)
        {
            var value = GetSetting(settingName);

            if (value == null)
                return "";
            else
                return (string)value;
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public List<int> GetSettingListInt(string settingName)
        {
            List<string> LS = (List<string>)GetSetting(settingName);

            if (LS == null)
            {
                return new List<int>();
            }
            else
            {
                return LS.Select(s => int.Parse(s)).ToList();
            }
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public List<double> GetSettingListDouble(string settingName)
        {
            List<string> LS = (List<string>)GetSetting(settingName);
            if (LS == null)
            {
                return new List<double>();
            }
            else
            {
                return LS.Select(s => double.Parse(s)).ToList();
            }
        }

        /// <summary>
        /// Gets a setting value
        /// </summary>
        /// <param name="settingName">Unique name of the setting</param>
        /// <returns>The setting value</returns>
        public List<string> GetSettingListString(string settingName)
        {
            List<string> LS = (List<string>)GetSetting(settingName);
            if (LS == null)
            {
                return new List<string>();
            }
            else
            {
                return LS;
            }
        }

        private static object CheckInput(object value)
        {
            if (value is List<double> LD)
            {
                List<string> LS = LD.ConvertAll<string>(x => x.ToString());
                return LS;
            }
            else if (value is List<int> LI)
            {
                List<string> LS = LI.ConvertAll<string>(x => x.ToString());
                return LS;
            }
            else if ((value is string) || (value is int) || (value is double) || (value is List<string>))
            {
                return value;
            }
            else
            {
                throw new Exception("Invalid setting type. Only single values or lists of string, double, or int can be stored.");
            }
        }
    }
}
