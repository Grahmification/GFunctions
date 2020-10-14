using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GFunctions.IO;
using GFunctions.Process;

namespace GFunctions.Winforms.Process
{
    /// <summary>
    /// A controller for a <see cref="IProcessView"/>
    /// </summary>
    /// <typeparam name="T">Parameter type to pass into the process when starting</typeparam>
    public class ProcessController<T> : ProcessControllerBase<T>
    {
        //------------------------- Private Members ------------------------
        public delegate Task DoWorkMethod(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken);

        public delegate void ProgressMethod(ProcessProgressArgs progArgs);

        //-------------------- Public Properties --------------------

        /// <summary>
        /// The views being controlled by this controller.
        /// </summary>
        public IProcessView View { get; private set; } = null;

        protected T ProcessArgument { get; set; }

        public List<DoWorkMethod> WorkMethods { get; private set; } = new List<DoWorkMethod>();
        public List<DoWorkMethod> CleanupMethods { get; private set; } = new List<DoWorkMethod>();
        public List<ProgressMethod> ProgressMethods { get; private set; } = new List<ProgressMethod>();
        //-------------------- Public functions --------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Exception logger for the class</param>
        public ProcessController(IProcessView view, ExceptionLogger logger = null) : base(logger)
        {
            View = view;
            View.StartRequest += onStartRequest;
            View.StopRequest += onStopRequest;
            View.ToggleRequest += onToggleRequest;

            //by default disable the view at startup
            View.AllowUse(false);
        }

        //-------------------- Event Methods --------------------
        private async void onStartRequest(object sender, EventArgs e)
        {
            try
            {
                //get the view that sent the command
                IProcessView v = (IProcessView)sender;
                await Start(ProcessArgument);
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                FormattedMessageBox.DisplayError(ex.Message);
            }
        }
        private void onStopRequest(object sender, EventArgs e)
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
        private async void onToggleRequest(object sender, EventArgs e)
        {
            try
            {
                //get the view that sent the command
                IProcessView v = (IProcessView)sender;
                await Toggle(ProcessArgument);
            }
            catch (Exception ex)
            {
                exLogger?.Log(ex);
                FormattedMessageBox.DisplayError(ex.Message);
            }
        }

        //-------------------- Override functions --------------------
        protected override void DoCleanup(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken)
        {
            foreach (DoWorkMethod m in CleanupMethods)
            {
                m(ProcessArgs, Progress, cToken);
            }
        }
        protected override async Task DoWork(T ProcessArgs, IProgress<ProcessProgressArgs> Progress, CancellationToken cToken)
        {
            foreach (DoWorkMethod m in WorkMethods)
            {
                await m(ProcessArgs, Progress, cToken);
            }
        }
        protected override void onProgress(ProcessProgressArgs progArgs)
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

        //-------------------- Device Lock functions --------------------
        public void ReleaseDevice()
        {
            //device needs to be used by something else, stop the process
            onStopRequest(this, new EventArgs());
        }
        public void AllowDeviceUse(bool enabled)
        {
            View.AllowUse(enabled);
        }
    }
}
