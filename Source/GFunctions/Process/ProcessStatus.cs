namespace GFunctions.Process
{
    /// <summary>
    /// Tags to describe the running state of a process
    /// </summary>
    public enum ProcessStatus 
    { 
        /// <summary>
        /// Process is running
        /// </summary>
        Running,

        /// <summary>
        /// Process is waiting to be started
        /// </summary>
        Idle,

        /// <summary>
        /// Process is in the process of cancelling
        /// </summary>
        Cancelling,

        /// <summary>
        /// Process has finished cancelling
        /// </summary>
        Cancelled,

        /// <summary>
        /// Process has completed succesfully
        /// </summary>
        Complete, 
        
        /// <summary>
        /// Process has exited due to an error
        /// </summary>
        Error 
    }
}
