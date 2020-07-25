using System;
using System.IO;


namespace GFunctions.IO
{
    public class Logger
    {
        private string _applicationPath = "";
        private string _fileName = "Log File";
        private string _folderName = "ErrorLogs";

        public Logger(string ApplicationPath, string fileName = "", string folderName = "")
        {
            try
            {
                this._applicationPath = ApplicationPath;

                if (fileName != "")
                    this._fileName = fileName;

                if (folderName != "")
                    this._folderName = folderName;

                createFolder();
                writeTextDatedBlock("Logger Initialized");
            }
            catch
            {

            }
        }


        protected void writeTextLine(string text)
        {
            File.AppendAllText(filePath(), text + Environment.NewLine);
        }
        protected void writeBlankLine()
        {
            File.AppendAllText(filePath(), Environment.NewLine);
        }
        protected void writeTextDatedBlock(string text)
        {
            writeTextLine("***************************************************************************************************************");
            writeTextLine(currentTimeAndDate());
            writeBlankLine();
            writeTextLine(text);
            writeTextLine("***************************************************************************************************************");
        }


        private void createFolder()
        {
            if (Directory.Exists(folderPath()) == false)
            {
                Directory.CreateDirectory(folderPath());
            }
        }
        private string folderPath()
        {
            return _applicationPath + "/" + _folderName;
        }
        private string filePath()
        {
            string fileName = currentDate() + " " + _fileName;

            return folderPath() + "/" + fileName + ".txt";
        }

        protected string currentTime()
        {
            return DateTime.Now.Hour.ToString() + "h " + DateTime.Now.Minute.ToString() + "m " + DateTime.Now.Second.ToString() + "s ";
        }
        protected string currentDate()
        {
            return DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
        }
        protected string currentTimeAndDate()
        {
            return currentDate() + " " + currentTime();
        }

    }

}
