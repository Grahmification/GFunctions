namespace GFunctions.Process
{
    /// <summary>
    /// Allows more detailed information to be passed through a progress reporter
    /// </summary>
    public class ProcessProgressArgs
    {
        /// <summary>
        /// Status of the process
        /// </summary>
        public ProcessStatus Status { get; set; } = ProcessStatus.Idle;

        /// <summary>
        /// Gets a string associated with the current process status
        /// </summary>
        public string StatusString
        {
            get
            {
                switch (Status)
                {
                    case ProcessStatus.Complete:
                        return "Complete";

                    case ProcessStatus.Cancelled:
                        return "Cancelled";

                    case ProcessStatus.Cancelling:
                        return "Cancelling";

                    case ProcessStatus.Idle:
                        return "Idle";

                    case ProcessStatus.Running:
                        return "Running";

                    case ProcessStatus.Error:
                        return "Error";

                    default:
                        return "";
                }

            }
        }

        /// <summary>
        /// Optional extra progress argument
        /// </summary>
        public string StatusStringArgs = "";

        /// <summary>
        /// Progress from 0 to 1
        /// </summary>
        public double Progress { get; set; } = 0;

        /// <summary>
        /// Progress in percentage for a ProgressBar
        /// </summary>
        public int PercentProgress { get { return (int)(Progress * 100.0); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="progress">The progress from 0 to 1</param>
        /// <param name="status">Current status of the process</param>
        /// <param name="statusStringArgs">Optional extra arguments</param>
        public ProcessProgressArgs(double progress, ProcessStatus status, string statusStringArgs = "")
        {
            Progress = progress;
            Status = status;
            StatusStringArgs = statusStringArgs;
        }

        /// <summary>
        /// Calculates progress from 0 to 1 for a 0 starting for loop
        /// </summary>
        /// <param name="currentIteration">Current loop iteration, starting from 0</param>
        /// <param name="maxIterations">The max loop iteration</param>
        /// <returns>The progress from 0 to 1</returns>
        public static double ForLoopProgress(int currentIteration, int maxIterations)
        {
            //use 1.0 so everything gets converted to doubles
            return (currentIteration + 1.0) / (maxIterations * 1.0);
        }

    }
}
