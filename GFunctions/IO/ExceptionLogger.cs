using System;
using System.Windows.Forms;


namespace GFunctions.IO
{
    public interface IExceptionLogger
    {
        void Log(Exception Ex);
    }
    public class ExceptionLogger : Logger, IExceptionLogger
    {
        public ExceptionLogger(string ApplicationPath) : base(ApplicationPath)
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
