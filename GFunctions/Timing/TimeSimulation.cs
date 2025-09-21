namespace GFunctions.Timing
{
    /// <summary>
    /// Class for performing a simulation that involves calling a foreground method at a specific time interval
    /// </summary>
    public class TimeSimulation
    {
        /// <summary>
        /// Flag to notify that background thread has finished
        /// </summary>
        private readonly ManualResetEvent _finishedStop = new(false);

        /// <summary>
        /// Flag to indicate foreground work has been finished for cycle, must be set after doing work from <see cref="SimulationDoWorkRequest"/> eventArgs
        /// </summary>
        private readonly ManualResetEvent _workDoneHandle = new(false);

        /// <summary>
        /// Keeps track of simulation time
        /// </summary>
        private readonly StopWatchPrecision _stopWatch = new();

        // --------------------------- Events -------------------------

        /// <summary>
        /// Fires to indicate the simulation work method should run based on the event
        /// </summary>
        public event EventHandler<TimeSimulationStepEventArgs>? SimulationDoWorkRequest;

        /// <summary>
        /// Fires to indicate the simulation run frequency has been updated. Int value is the current frequency [Hz].
        /// </summary>
        public event EventHandler<int>? RunFrequencyUpdated;

        // --------------------------- Public Properties -------------------------

        /// <summary>
        /// True if the simulation is running
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// The number of simulation cycles that have passed since starting
        /// </summary>
        public int CycleCount { get; private set; } = 0;

        /// <summary>
        /// The target simulation cycle time increment in ms
        /// </summary>
        public double TargetCycleTime { get; private set; } = 30;

        // --------------------------- Public Methods -------------------------

        /// <summary>
        /// Start the simulation running
        /// </summary>
        /// <param name="timeIncrement">The time increment in ms between simulation cycles</param>
        public void Start(double timeIncrement)
        {
            // Only allow starting if we're stopped
            if(!Running)
            {
                var workCallBack = new Progress<bool>(s => OnWorkCallbackProgress());
                var frequencyCallBack = new Progress<int>(s => RunFrequencyUpdated?.Invoke(this, s));

                TargetCycleTime = timeIncrement;
                Running = true;
                CycleCount = 0;
                _finishedStop.Reset();

                _stopWatch.StartNew(); // Log starting time

                Task.Factory.StartNew(() => RunDoWork(frequencyCallBack, workCallBack), TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Stop the simulation from running
        /// </summary>
        public void Stop()
        {
            if(Running)
            {
                Running = false; // Set flag to stop

                _workDoneHandle.Set(); // Don't need to wait anymore on work method
                _finishedStop.WaitOne(); //loops still running, need to wait

                //----------- now safely run clean up code -------------

                _stopWatch.Stop();
            }
        }

        // --------------------------- Private Helpers -------------------------

        /// <summary>
        /// Runs the simulation in the background thread
        /// </summary>
        /// <param name="freqCallBack">Callback to indicate when simulation run frequency is updated</param>
        /// <param name="workCallBack">Callback to indicate when foreground work should be performed</param>
        private void RunDoWork(IProgress<int> freqCallBack, IProgress<bool> workCallBack)
        {
            while (Running)
            {
                double loopStartTime = _stopWatch.ElapsedMilliseconds;
                double loopEndTime = loopStartTime + TargetCycleTime; // Time the loop should be ending in ms

                //-------------- Do work here ---------------------------------
                _workDoneHandle.Reset();
                workCallBack.Report(true); // Notify work should be done in foreground thread
                _workDoneHandle.WaitOne(); // Wait for work to be done

                // ------------- sleep if needed ------------------------------

                if (_stopWatch.ElapsedMilliseconds < loopEndTime)
                { Thread.Sleep((int)(loopEndTime - _stopWatch.ElapsedMilliseconds)); }

                // ------------- report operating frequency ------------------------------

                // Only every 10 cycles or else will make simulation slow
                if (CycleCount % 10 == 0)
                {
                    double freq = 1000.0 / (_stopWatch.ElapsedMilliseconds - loopStartTime);
                    freqCallBack.Report((int)freq);
                }

                CycleCount++;
            }

            _finishedStop.Set(); //all running loops have ended
        }
        
        /// <summary>
        /// Foreground thread - notifies work method to run
        /// </summary>
        private void OnWorkCallbackProgress()
        {
            SimulationDoWorkRequest?.Invoke(this, new TimeSimulationStepEventArgs(TargetCycleTime / 1000.0, _stopWatch.ElapsedMilliseconds, _workDoneHandle));
        }
    }

    /// <summary>
    /// Arguments for when the <see cref="TimeSimulation"/> work method is called
    /// </summary>
    /// <param name="timeIncrement">The simulation cycle time [s]</param>
    /// <param name="time">Elapsed time since the simulation was started [s]</param>
    /// <param name="workDoneCallBack">Callback to indicate the simulation should proceed</param>
    public class TimeSimulationStepEventArgs(double timeIncrement, double time, ManualResetEvent workDoneCallBack)
    {
        /// <summary>
        /// The work method should set this callback to indicate that the simulation can proceed with another cycle
        /// </summary>
        private readonly ManualResetEvent _workDoneCallback = workDoneCallBack;

        /// <summary>
        /// The simulation cycle time [s]
        /// </summary>
        public double TimeIncrement { get; private set; } = timeIncrement;

        /// <summary>
        /// Elapsed time since the simulation was started [s]
        /// </summary>
        public double Time { get; private set; } = time;

        /// <summary>
        /// The work method should call this method when all work is done to indicate that the simulation can proceed with another cycle
        /// </summary>
        public void FlagWorkDone()
        {
            _workDoneCallback.Set();
        }
    }
}
