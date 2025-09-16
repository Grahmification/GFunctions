namespace GFunctions.Timing
{
    /// <summary>
    /// Class for performing a simulation that involves calling a method at a specific time interval
    /// </summary>
    public class TimeSimulation
    {
        private readonly ManualResetEvent _finishedStop = new(false); //flag to notify that all running loops have ended
        private readonly ManualResetEvent _workDoneHandle = new(false); //flag to notify foreground work has been finished for cycle, must be set after doing work from the eventArgs
        private double _timeIncrement = 30; //simulation time increment in ms
        private readonly StopWatchPrecision _sw = new(); //keeps track of simulation time

        /// <summary>
        /// Fires to indicate the simulation work method should run based on the event
        /// </summary>
        public event EventHandler<TimeSimulationStepEventArgs>? SimulationDoWorkRequest;

        /// <summary>
        /// Fires to indicate the simulation FPS has been updates
        /// </summary>
        public event EventHandler<int>? RunFreqUpdated;

        /// <summary>
        /// True if the simulation is running
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// The number of simulation cycles that have passed since starting
        /// </summary>
        public int CycleCount { get; private set; } = 0;


        /// <summary>
        /// Start the simulation running
        /// </summary>
        /// <param name="timeIncrement">The time increment in ms between simulation cycles</param>
        public void Start(double timeIncrement)
        {
            var RequestCallBack = new Progress<int>(s => RunDoWorkCallback());
            var FPSCallBack = new Progress<int>(s => FrequencyCallback(s));

            _timeIncrement = timeIncrement;
            Running = true;
            CycleCount = 0;
            _finishedStop.Reset();

            _sw.StartNew(); //log starting time

            Task.Factory.StartNew(() => RunDoWork(FPSCallBack, RequestCallBack), TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Stop the simulation from running
        /// </summary>
        public void Stop()
        {
            Running = false; //set flag to stop

            _workDoneHandle.Set(); //don't need to wait anymore
            _finishedStop.WaitOne(); //loops still running, need to wait            

            //----------- now safely run clean up code -------------

            _sw.Stop();
        }

        private void RunDoWork(IProgress<int> freqCallBack, IProgress<int> requestCallBack)
        {
            while (Running)
            {
                double loopStartTime = _sw.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + _timeIncrement; //time loop should be ending in ms

                //-------------- Do work here ---------------------------------
                _workDoneHandle.Reset();
                requestCallBack.Report(-1); //allows work to be done in foreground thread
                _workDoneHandle.WaitOne(); //wait for work to be done

                // ------------- sleep if needed ------------------------------

                if (_sw.ElapsedMilliseconds < loopEndTime)
                { Thread.Sleep((int)(loopEndTime - _sw.ElapsedMilliseconds)); }

                // ------------- report operating frequency ------------------------------

                // Only every 10 cycles or else will make simulation slow
                if (CycleCount % 10 == 0)
                {
                    double freq = 1000.0 / (_sw.ElapsedMilliseconds - loopStartTime);
                    freqCallBack.Report((int)freq);
                }

                CycleCount++;
            }

            _finishedStop.Set(); //all running loops have ended
        }
        private void RunDoWorkCallback()
        {
            SimulationDoWorkRequest?.Invoke(this, new TimeSimulationStepEventArgs(_timeIncrement / 1000.0, _sw.ElapsedMilliseconds, _workDoneHandle));
        } // Makes event get raised in foreground thread
        private void FrequencyCallback(int runFrequency)
        {
            RunFreqUpdated?.Invoke(this, runFrequency);
        } // Makes event get raised in foreground thread
    }

    /// <summary>
    /// Arguments for when the <see cref="TimeSimulation"/> work method is called
    /// </summary>
    public class TimeSimulationStepEventArgs
    {
        /// <summary>
        /// The simulation cycle time [s]
        /// </summary>
        public double TimeIncrement { get; private set; }

        /// <summary>
        /// Elapsed time since the simulation was started [s]
        /// </summary>
        public double Time { get; private set; }

        /// <summary>
        /// The work method should set this callback to indicate that the simulation can proceed with another cycle
        /// </summary>
        public ManualResetEvent WorkDoneCallback { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="timeIncrement">The simulation cycle time [s]</param>
        /// <param name="time">Elapsed time since the simulation was started [s]</param>
        /// <param name="workDoneCallBack">Callback to indicate the simulation should proceed</param>
        public TimeSimulationStepEventArgs(double timeIncrement, double time, ManualResetEvent workDoneCallBack)
        {
            TimeIncrement = timeIncrement;
            Time = time;
            WorkDoneCallback = workDoneCallBack;
        }
    }
}
