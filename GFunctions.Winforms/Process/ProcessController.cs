using GFunctions.IO;
using GFunctions.Process;
using GFunctions.Winforms.Dialogs;

namespace GFunctions.Winforms.Process
{
    /// <summary>
    /// A controller for a <see cref="IProcessView"/>
    /// </summary>
    /// <typeparam name="T">Parameter type to pass into the process when starting</typeparam>
    public class ProcessController<T> : ProcessControllerBase<T>
    {
        //------------------------- Private Members ------------------------
        
        /// <summary>
        /// Definition for a function that can be used as a background process
        /// </summary>
        /// <param name="processArgs">Arguments to the background process</param>
        /// <param name="progress">For reporting progress to the foreground thread</param>
        /// <param name="cToken">For processes cancellation</param>
        /// <returns></returns>
        public delegate Task DoWorkMethod(T? processArgs, IProgress<ProcessProgressArgs>? progress, CancellationToken cToken);

        /// <summary>
        /// Definition for a function that can be called when a progress update occurs
        /// </summary>
        /// <param name="progArgs">Information about the progress state</param>
        public delegate void ProgressMethod(ProcessProgressArgs progArgs);

        //-------------------- Public Properties --------------------

        /// <summary>
        /// The views being controlled by this controller.
        /// </summary>
        public IProcessView View { get; private set; }

        /// <summary>
        /// Argument to pass to the background process when it starts
        /// </summary>
        protected T? ProcessArgument { get; set; }

        /// <summary>
        /// Methods to run in the background thread
        /// </summary>
        public List<DoWorkMethod> WorkMethods { get; private set; } = [];

        /// <summary>
        /// Methods to run after the workmethods complete - successfully or due to an error/cancelled
        /// </summary>
        public List<DoWorkMethod> CleanupMethods { get; private set; } = [];

        /// <summary>
        /// Methods to run when a background progres update occurs
        /// </summary>
        public List<ProgressMethod> ProgressMethods { get; private set; } = [];
        //-------------------- Public functions --------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="view">View to control the process</param>
        /// <param name="logger">Exception logger for the background process</param>
        public ProcessController(IProcessView view, ExceptionLogger? logger = null) : base(logger)
        {
            View = view;
            View.StartRequest += OnStartRequest;
            View.StopRequest += OnStopRequest;
            View.ToggleRequest += OnToggleRequest;

            //by default disable the view at startup
            View.AllowUse(false);
        }

        //-------------------- Event Methods --------------------
        private async void OnStartRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    //get the view that sent the command
                    IProcessView v = (IProcessView)sender;
                    await Start(ProcessArgument);
                }
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                FormattedMessageBox.DisplayError(ex.Message);
            }
        }
        private void OnStopRequest(object? sender, EventArgs e)
        {
            try
            {
                //stop the process
                Stop();
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                FormattedMessageBox.DisplayError(ex.Message);
            }
        }
        private async void OnToggleRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    //get the view that sent the command
                    IProcessView v = (IProcessView)sender;
                    await Toggle(ProcessArgument);
                }
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                FormattedMessageBox.DisplayError(ex.Message);
            }
        }

        //-------------------- Override functions --------------------

        /// <summary>
        /// Called in the background process after all workmethods finish - successfully or due to an error/cancelled
        /// </summary>
        /// <param name="processArgs">Arguments to the background process</param>
        /// <param name="progress">For reporting progress to the foreground thread</param>
        /// <param name="cToken">For processes cancellation</param>
        protected override void DoCleanup(T? processArgs, IProgress<ProcessProgressArgs> progress, CancellationToken cToken)
        {
            foreach (DoWorkMethod m in CleanupMethods)
            {
                m(processArgs, progress, cToken);
            }
        }

        /// <summary>
        /// Called in the background process
        /// </summary>
        /// <param name="processArgs">Arguments to the background process</param>
        /// <param name="progress">For reporting progress to the foreground thread</param>
        /// <param name="cToken">For processes cancellation</param>
        /// <returns></returns>
        protected override async Task DoWork(T? processArgs, IProgress<ProcessProgressArgs>? progress, CancellationToken cToken)
        {
            foreach (DoWorkMethod m in WorkMethods)
            {
                await m(processArgs, progress, cToken);
            }
        }

        /// <summary>
        /// Called when progress is updated from the background process
        /// </summary>
        /// <param name="progArgs">Information about the progress state</param>
        protected override void OnProgress(ProcessProgressArgs progArgs)
        {
            if (progArgs.Status == ProcessStatus.Running)
                View.ToggleButtonText = "Stop";
            else
                View.ToggleButtonText = "Start";

            //if we get an error, display the message, its stored in the status string arguments
            if (progArgs.Status == ProcessStatus.Error)
                FormattedMessageBox.DisplayError(progArgs.StatusStringArgs);

            View.SetStatus(progArgs.StatusString);
            View.SetProgress(progArgs.PercentProgress);

            foreach (ProgressMethod m in ProgressMethods)
            {
                m(progArgs);
            }
        }

        //-------------------- View Lock functions --------------------

        /// <summary>
        /// Cancels any long running processes associated with the connected view
        /// </summary>
        public void ReleaseView()
        {
            // Resources need to be used by something else, stop the process
            OnStopRequest(this, new EventArgs());
        }

        /// <summary>
        /// Enables or disables use of the connected view
        /// </summary>
        /// <param name="enabled">True to enable the view</param>
        public void AllowViewUse(bool enabled)
        {
            View.AllowUse(enabled);
        }
    }
}
