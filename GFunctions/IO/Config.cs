using System.Xml.Serialization;

namespace GFunctions.IO
{
    public class Config
    {
        // ---------------------------------------------------------------------------------------
        public List<string> settingNames = new List<string>(); //do not directly set this
        public List<object> settingValues = new List<object>(); //do not directly set this

        // ---------------------------------------------------------------------------------------   
        [XmlIgnore]
        public const string DefaultFolderPath = "Program Settings"; //default relative folder for saving settings
        [XmlIgnore]
        public const string FileName = "config.xml"; // Name of configuration file.
        [XmlIgnore]
        public const string FileNameDefault = "config_default.xml"; // Name of default configuration file.
        [XmlIgnore]
        public string FolderPath { get; private set; } = ""; //optional folder path if desired to save the config files at a location other than with the application, can be absolute or relative

        [XmlIgnore]
        public static Config Instance { get; private set; } // Globally accessable instance of loaded configuration.

        // ---------------------------------------------------------------------------------------   

        // Empty constructor for XmlSerializer.
        public Config()
        {
        }

        // Checks if config data exists
        public static bool DataExists(string folderPath = Config.DefaultFolderPath)
        {
            return File.Exists(Config.buildfullFilePath(folderPath, Config.FileName)) && File.Exists(Config.buildfullFilePath(folderPath, Config.FileNameDefault));
        }
        // Loads the configuration from file.
        public static void Load(string folderPath = Config.DefaultFolderPath)
        {
            Config.LoadDoWork(Config.FileName, folderPath);
        }
        // Loads the default configuration from file.
        public static void LoadDefault(string folderPath = Config.DefaultFolderPath)
        {
            Config.LoadDoWork(Config.FileNameDefault, folderPath);
        }
        //called by both load functions
        private static void LoadDoWork(string fileName, string folderPath = "")
        {
            var serializer = new XmlSerializer(typeof(Config));

            if (!File.Exists(Config.buildfullFilePath(folderPath, fileName))) //file does not exist, no previous configuration has been saved
            {
                Config.Instance = new Config();
                Config.Instance.FolderPath = folderPath;
            }
            else //file does exist, load config from file
            {
                using (var fStream = new FileStream(Config.buildfullFilePath(folderPath, fileName), FileMode.Open))
                    Config.Instance = (Config)serializer.Deserialize(fStream);

                Config.Instance.FolderPath = folderPath;
            }
        }
        // Saves the configuration to file.
        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Config));

            if (!Directory.Exists(this.FolderPath))
                Directory.CreateDirectory(this.FolderPath); //create the settings folder if it doesn't exist


            if (!File.Exists(Config.buildfullFilePath(this.FolderPath, Config.FileName))) //file does not exist, create default settings file on first save
            {
                using (var fStreamDefault = new FileStream(Config.buildfullFilePath(this.FolderPath, Config.FileNameDefault), FileMode.Create))
                    serializer.Serialize(fStreamDefault, this);
            }

            using (var fStream = new FileStream(Config.buildfullFilePath(this.FolderPath, Config.FileName), FileMode.Create))
            {
                serializer.Serialize(fStream, this);
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

        // --------------------------------------------------------------------------------------- 

        public void AddSetting(string settingName, object value = null)
        {
            if (!settingNames.Contains(settingName)) //only add if the setting name doesn't exist
            {
                settingNames.Add(settingName);
                value = Config.CheckInput(value); //check for correct setting type
                settingValues.Add(value);

            }
        }
        public void DeleteSetting(string settingName)
        {
            if (settingNames.Contains(settingName)) //only remove if the setting name exists
            {
                int index = settingNames.IndexOf(settingName);
                settingNames.RemoveAt(index);
                settingValues.RemoveAt(index);
            }
        }
        public bool SettingExists(string settingName)
        {
            return settingNames.Contains(settingName);
        }

        private void SetSetting(string settingName, object value)
        {
            if (!settingNames.Contains(settingName)) //throw error if it doesn't exists
                throw new Exception("Tried to set setting that doesn't exist: " + settingName);

            value = Config.CheckInput(value); //check for correct setting type

            int index = settingNames.IndexOf(settingName);
            settingValues[index] = value;
        }
        public void SetSetting(string settingName, string value)
        {
            this.SetSetting(settingName, (object)value);
        }
        public void SetSetting(string settingName, int value)
        {
            this.SetSetting(settingName, (object)value);
        }
        public void SetSetting(string settingName, double value)
        {
            this.SetSetting(settingName, (object)value);
        }
        public void SetSetting(string settingName, List<string> value)
        {
            this.SetSetting(settingName, (object)value);
        }
        public void SetSetting(string settingName, List<int> value)
        {
            this.SetSetting(settingName, (object)value);
        }
        public void SetSetting(string settingName, List<double> value)
        {
            this.SetSetting(settingName, (object)value);
        }

        private object GetSetting(string settingName)
        {
            if (!settingNames.Contains(settingName)) //throw error if it doesn't exists
                throw new Exception("Tried to get setting that doesn't exist: " + settingName);

            int index = settingNames.IndexOf(settingName);
            return settingValues[index];
        }
        public int GetSettingInt(string settingName)
        {
            var value = this.GetSetting(settingName);

            if (value == null)
                return 0;
            else
                return (int)value;
        }
        public double GetSettingDouble(string settingName)
        {
            var value = this.GetSetting(settingName);

            if (value == null)
                return 0;
            else
                return (double)value;
        }
        public string GetSettingString(string settingName)
        {
            var value = this.GetSetting(settingName);

            if (value == null)
                return "";
            else
                return (string)value;
        }
        public List<int> GetSettingListInt(string settingName)
        {
            List<string> LS = (List<string>)this.GetSetting(settingName);

            if (LS == null)
            {
                return new List<int>();
            }
            else
            {
                return LS.Select(s => int.Parse(s)).ToList();
            }
        }
        public List<double> GetSettingListDouble(string settingName)
        {
            List<string> LS = (List<string>)this.GetSetting(settingName);
            if (LS == null)
            {
                return new List<double>();
            }
            else
            {
                return LS.Select(s => double.Parse(s)).ToList();
            }
        }
        public List<string> GetSettingListString(string settingName)
        {
            List<string> LS = (List<string>)this.GetSetting(settingName);
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
            if (value is List<double>)
            {
                var LD = (List<double>)value;
                List<string> LS = LD.ConvertAll<string>(x => x.ToString());
                return LS;
            }
            else if (value is List<int>)
            {
                var LI = (List<int>)value;
                List<string> LS = LI.ConvertAll<string>(x => x.ToString());
                return LS;
            }
            else if (!(value is string) && !(value is int) && !(value is double) && !(value is List<string>) && !(value is null))
            {
                throw new Exception("Invalid setting type. Only single values or lists of string, double, or int can be stored.");
            }
            else
            {
                return value;
            }
        }
    }
}
