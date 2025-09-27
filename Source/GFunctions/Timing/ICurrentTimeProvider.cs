namespace GFunctions.Timing
{
    /// <summary>
    /// Generic definition for a class that keeps track of the current time
    /// </summary>
    public interface ICurrentTimeProvider
    {
        /// <summary>
        /// True if the time provider is actively recording time
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// The number of seconds since the time provider started counting time
        /// </summary>
        public double ElapsedSeconds { get; }
    }
}
