namespace GFunctions.Winforms.Process
{
    /// <summary>
    /// A view for starting and stopping a background process, while displaying progress
    /// </summary>
    public partial class ProcessToggleView : UserControl, IProcessView
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ProcessToggleView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The toggle button display text
        /// </summary>
        public string ToggleButtonText { get { return button_toggle.Text; } set { button_toggle.Text = value; } }

        /// <summary>
        /// Fires when the view requests the process start
        /// </summary>
        public event EventHandler? StartRequest;

        /// <summary>
        /// Fires when the view requests the process stop
        /// </summary>
        public event EventHandler? StopRequest;

        /// <summary>
        /// Fires when the view requests the process toggle start/stop
        /// </summary>
        public event EventHandler? ToggleRequest;

        /// <summary>
        /// Enables or disabled view input
        /// </summary>
        /// <param name="enabled">True to enable</param>
        public void AllowUse(bool enabled)
        {
            button_toggle.Enabled = enabled;
        }

        /// <summary>
        /// Sets the view progress status
        /// </summary>
        /// <param name="percentProgress">Progres from 0 to 100</param>
        public void SetProgress(int percentProgress)
        {
            progressBar_progress.Value = percentProgress;
        }
        
        /// <summary>
        /// Sets the view status
        /// </summary>
        /// <param name="status">String describing process status</param>
        public void SetStatus(string status)
        {
            label_status.Text = status;
        }


        private void button_toggle_Click(object? sender, EventArgs e)
        {
            ToggleRequest?.Invoke(this, e);
        }

        /// <summary>
        /// Prevents the compiler from showing warnings for unused events
        /// </summary>
        private void RemoveWarnings()
        {
            StartRequest?.Invoke(this, new EventArgs());
            StopRequest?.Invoke(this, new EventArgs());
        }
    }
}
