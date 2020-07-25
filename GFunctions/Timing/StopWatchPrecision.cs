using System.Diagnostics;

namespace GFunctions.Timing
{
    public class StopWatchPrecision
    {
        //Created because the timer operates much more accurately working in ticks than ms

        private Stopwatch _sw = new Stopwatch();
        private double _freq = Stopwatch.Frequency / 1000.0; //number of milliseconds in one timer tick

        public bool IsRunning { get { return _sw.IsRunning; } }
        public double ElapsedMilliseconds { get { return this._sw.ElapsedTicks / _freq; } }
        public double ElapsedSeconds { get { return this._sw.ElapsedTicks / (_freq * 1000.0); } }

        public StopWatchPrecision()
        {
            _sw = new Stopwatch();
            _freq = Stopwatch.Frequency / 1000.0;
        }

        public void StartNew()
        {
            _sw = Stopwatch.StartNew();
        }
        public void Stop()
        {
            _sw.Stop();
        }

    }
}
