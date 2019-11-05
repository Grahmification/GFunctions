using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace GFunctions
{
    public class csvReader
    {
        private StreamReader _reader = null;
        private string _fullPath = "";
        private bool _continueSearchFlag = true; //if file does not exist, program will keep searching until this is set to false
        private const int reCheckPeriod = 100; //refresh period in ms to check if a file exists


        public csvReader()
        {

        }

        public void open(string folder, string fileName)
        {
            _fullPath = folder + @"\" + fileName;

            this.open(_fullPath);
        }

        public void open(string filePath)
        {
            waitForFile(filePath, reCheckPeriod);
            Thread.Sleep(reCheckPeriod * 2); //wait so no usage errors thrown

            if (_continueSearchFlag == true) //if false, user has cancelled search... don't initialize
            {
                _reader = new StreamReader(filePath);
            }
            else { return; }
        }


        public void close(bool deleteFile = false)
        {
            if (_reader != null)
            {
                _reader.Close();

                if (deleteFile == true)
                {
                    File.Delete(_fullPath);
                }

            }
        }

        public void cancelSearch()
        {
            _continueSearchFlag = false;
        }

        public string readline()
        {
            var line = _reader.ReadLine();
            return line;
        }

        public string[] readAllLines()
        {
            List<string> lineList = new List<string>();

            while (!_reader.EndOfStream)
            {
                lineList.Add(_reader.ReadLine());
            }

            return lineList.ToArray();
        }

        public static string[] splitLine(string line)
        {
            return line.Split(',');
        }

        private void waitForFile(string fullPath, int reCheckPeriod)
        {
            while (!File.Exists(fullPath) && _continueSearchFlag == true)
            {
                Thread.Sleep(reCheckPeriod);
            }
        }

    }

    public class csvWriter
    {
        private StreamWriter _writer = null;

        public csvWriter(string folder, string fileName)
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
