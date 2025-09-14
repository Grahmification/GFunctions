using System;


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
    }
}
