using GFunctions.IO;

namespace GFunctions.Process
{
    /// <summary>
    /// Base definition for a class which manages a process running in the background
    /// </summary>
    /// <typeparam name="T">Argument to pass into the background process when it starts</typeparam>
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
        public event EventHandler<ProcessProgressArgs>? ProgressUpdated;

        /// <summary>
        /// Token source for stopping the running process
        /// </summary>
        protected CancellationTokenSource tSource = new CancellationTokenSource();

        /// <summary>
        /// Exception logger for the controller
        /// </summary>
        protected ExceptionLogger? exLogger;

        //-------------------- Public functions --------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Exception logger for the class</param>
        public ProcessControllerBase(ExceptionLogger? logger = null)
        {
            exLogger = logger;
        }

        /// <summary>
        /// Starts the process
        /// </summary>
        /// <param name="processArgs">Arguments to be passed into the process</param>
        /// <returns></returns>
        public async Task Start(T? processArgs)
        {
            try
            {
                if (!Running)
                {
                    tSource = new CancellationTokenSource();

                    var Progress = new Progress<ProcessProgressArgs>(s => _OnProgress(s));
                    await _DoWork(processArgs, Progress, tSource.Token);
                }
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                _OnProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
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
                    _OnProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Cancelling));
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                _OnProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
        }

        /// <summary>
        /// Toggles the process run state
        /// </summary>
        /// <param name="ProcessArgs">Arguments to be passed into the process</param>
        /// <returns></returns>
        public async Task Toggle(T? ProcessArgs)
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
                _OnProgress(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
        }

        //-------------------- Overrideable functions --------------------

        /// <summary>
        /// Gets called by the primary background work process
        /// </summary>
        /// <param name="processArgs">Misc arguments to be passed into the process</param>
        /// <param name="progress">Progress reporter for the process</param>
        /// <param name="cToken">Cancellation token for the process</param>
        protected virtual async Task DoWork(T? processArgs, IProgress<ProcessProgressArgs>? progress, CancellationToken cToken)
        {
            //prevent the await error from showing up
            await Task.Delay(1);
        }

        /// <summary>
        /// Gets called by the primary background work process in the finally block
        /// <param name="processArgs">Misc arguments to be passed into the process</param>
        /// <param name="progress">Progress reporter for the process</param>
        /// <param name="cToken">Cancellation token for the process</param>
        /// </summary>
        protected virtual void DoCleanup(T? processArgs, IProgress<ProcessProgressArgs> progress, CancellationToken cToken) { }

        /// <summary>
        /// Gets called in the foreground thread whenever progress is updated by the background process
        /// </summary>
        /// <param name="progArgs">Information about the progress</param>
        protected virtual void OnProgress(ProcessProgressArgs progArgs) { }

        //-------------------- Private helpers --------------------

        /// <summary>
        /// Primary method for doing work in the background
        /// </summary>
        /// <param name="processArgs">Misc arguments to be passed into the process</param>
        /// <param name="progress">Progress reporter for the process</param>
        /// <param name="cToken">Cancellation token for the process</param>
        /// <returns></returns>
        private async Task _DoWork(T? processArgs, IProgress<ProcessProgressArgs> progress, CancellationToken cToken)
        {
            Running = true;

            try
            {
                //Report start progress here. Report 1% so progress bar shows a tiny change
                progress?.Report(new ProcessProgressArgs(0.01, ProcessStatus.Running));

                //get the parent to do the actual process work
                await DoWork(processArgs, progress, cToken);

                //cleanup - we don't want to report this if an error occurs
                if (tSource.IsCancellationRequested)
                    progress?.Report(new ProcessProgressArgs(0, ProcessStatus.Cancelled));
                else
                    progress?.Report(new ProcessProgressArgs(1.0, ProcessStatus.Complete));
            }
            //We don't care about cancelled exceptions, this is ok
            catch (TaskCanceledException)
            {
                progress?.Report(new ProcessProgressArgs(0, ProcessStatus.Cancelled));
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                progress?.Report(new ProcessProgressArgs(CurrentProgress.Progress, ProcessStatus.Error, ex.Message));
            }
            finally
            {
                Running = false;
                DoCleanup(processArgs, progress, cToken);
            }
        }

        /// <summary>
        /// Private helper which gets called by process progressReporter
        /// </summary>
        /// <param name="progArgs"></param>
        private void _OnProgress(ProcessProgressArgs progArgs)
        {
            CurrentProgress = progArgs;
            OnProgress(progArgs);
            ProgressUpdated?.Invoke(this, progArgs);
        }
    }
}
