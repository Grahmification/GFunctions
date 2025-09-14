using GFunctions.IO;

namespace GFunctions.Process
{
    public class ProcessControllerBase<T>
    {
        /// <summary>
        /// Returns true if the process is running
        /// </summary>
        public bool Running { get; private set; } = false;

        /// <summary>
        /// The current progress of the process from 0 to 1
        /// </summary>
        public ProcessProgressArgs CurrentProgress { get; private set; } = new ProcessProgressArgs(0, ProcessStatus.Idle);

        /// <summary>
        /// Fires whenever the progress gets updated (thread safe)
        /// </summary>
        public event EventHandler<ProcessProgressArgs> ProgressUpdated;

        /// <summary>
        /// Token source for stopping the running process
        /// </summary>
        protected CancellationTokenSource tSource = new CancellationTokenSource();

        /// <summary>
        /// Exception logger for the controller
        /// </summary>
        protected ExceptionLogger exLogger;

        //-------------------- Public functions --------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Exception logger for the class</param>
        public ProcessControllerBase(ExceptionLogger logger = null)
        {
            exLogger = logger;
        }

        /// <summary>
        /// Starts the process
        /// </summary>
        /// <param name="ProcessArgs">Arguments to be passed into the process</param>
        /// <returns></returns>
        public async Task Start(T ProcessArgs)
        {
            try
            {
                if (Running == false)
                {
                    tSource = new CancellationTokenSource();

                    var Progress = new Progress<ProcessProgressArgs>(s => _onProgress(s));
                    await _DoWork(ProcessArgs, Progress, tSource.Token);
                }
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                _onProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
        }

        /// <summary>
        /// Cancels the process
        /// </summary>
        public void Stop()
        {
            try
            {
                tSource.Cancel();

                if (Running)
                    _onProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Cancelling));
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                _onProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
        }

        /// <summary>
        /// Toggles the process run state
        /// </summary>
        /// <param name="ProcessArgs">Arguments to be passed into the process</param>
        /// <returns></returns>
        public async Task Toggle(T ProcessArgs)
        {
            try
            {
                if (Running)
                    Stop();
                else
                    await Start(ProcessArgs);
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                _onProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
        }

        //-------------------- Overrideable functions --------------------

        /// <summary>
        /// Gets called by the primary background work process
        /// </summary>
        protected virtual async Task DoWork(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken)
        {
            //prevent the await error from showing up
            await Task.Delay(1);
        }

        /// <summary>
        /// Gets called by the primary background work process in the finally block
        /// </summary>
        protected virtual void DoCleanup(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken) { }

        /// <summary>
        /// Gets called whenever progress is updated by the background process
        /// </summary>
        /// <param name="progArgs">Information about the progress</param>
        protected virtual void onProgress(ProcessProgressArgs progArgs) { }

        //-------------------- Private helpers --------------------

        /// <summary>
        /// Primary method for doing work in the background
        /// </summary>
        /// <param name="ProcessArgs">Misc arguments to be passed into the process</param>
        /// <param name="Progress">Progress reporter for the process</param>
        /// <param name="cToken">Cancellation token for the process</param>
        /// <returns></returns>
        private async Task _DoWork(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken)
        {
            Running = true;

            try
            {
                //Report start progress here. Report 1% so progress bar shows a tiny change
                Progress?.Report(new ProcessProgressArgs(0.01, ProcessStatus.Running));

                //get the parent to do the actual process work
                await DoWork(ProcessArgs, Progress, cToken);

                //cleanup - we don't want to report this if an error occurs
                if (tSource.IsCancellationRequested)
                    Progress?.Report(new ProcessProgressArgs(0, ProcessStatus.Cancelled));
                else
                    Progress?.Report(new ProcessProgressArgs(1.0, ProcessStatus.Complete));
            }
            //We don't care about cancelled exceptions, this is ok
            catch (TaskCanceledException)
            {
                Progress?.Report(new ProcessProgressArgs(0, ProcessStatus.Cancelled));
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                Progress?.Report(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
            finally
            {
                Running = false;
                DoCleanup(ProcessArgs, Progress, cToken);
            }
        }

        /// <summary>
        /// Private helper which gets called by process progressReporter
        /// </summary>
        /// <param name="progArgs"></param>
        private void _onProgress(ProcessProgressArgs progArgs)
        {
            CurrentProgress = progArgs;
            onProgress(progArgs);
            ProgressUpdated?.Invoke(this, progArgs);
        }

    }
}
