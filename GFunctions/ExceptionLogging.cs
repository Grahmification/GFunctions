using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace ZaberFunctions
{
    public class Logging
    {
        private string _applicationPath = "";
        private string _fileName = "Log File";
        private string _folderName = "ErrorLogs";

        public Logging(string ApplicationPath, string fileName = "", string folderName = "")
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

    public interface IExceptionLogger
    {
        void Log(Exception Ex);
    }
    public class ExceptionLogging : Logging, IExceptionLogger
    {
        public ExceptionLogging(string ApplicationPath) : base(ApplicationPath)
        {

        }

        public void Log(Exception ex)
        {
            try
            {
                this.writeTextDatedBlock(ex.ToString());
            }
            catch
            {

            }
        }

        public static void DisplayError(string message)
        {
            try
            {

                MessageBox.Show("An error occurred: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {

            }
        }
    }

}
