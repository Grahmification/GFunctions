using System.Diagnostics;

namespace GFunctions.Timing
{
    /// <summary>
    /// A stopwatch that operations much more accurately than the default <see cref="Stopwatch"/> becuase it uses ticks.
    /// </summary>
    public class StopWatchPrecision
    {
        private Stopwatch _sw = new();
        private readonly double _freq = Stopwatch.Frequency / 1000.0; //number of milliseconds in one timer tick

        /// <summary>
        /// True if the stopwatch is running
        /// </summary>
        public bool IsRunning => _sw.IsRunning;

        /// <summary>
        /// The number of milliseconds since starting
        /// </summary>
        public double ElapsedMilliseconds => _sw.ElapsedTicks / _freq;

        /// <summary>
        /// The number of seconds since starting
        /// </summary>
        public double ElapsedSeconds => _sw.ElapsedTicks / (_freq * 1000.0);

        /// <summary>
        /// Default constructor
        /// </summary>
        public StopWatchPrecision() { }

        /// <summary>
        /// Start the stopwatch at T=0
        /// </summary>
        public void StartNew()
        {
            _sw = Stopwatch.StartNew();
        }
        
        /// <summary>
        /// Stop of the stopwatch
        /// </summary>
        public void Stop()
        {
            _sw.Stop();
        }
    }
}
