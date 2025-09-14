namespace GFunctions.Winforms.Process
{
    public partial class ProcessToggleView : UserControl, IProcessView
    {
        public ProcessToggleView()
        {
            InitializeComponent();
        }

        public string ToggleButtonText { get { return button_toggle.Text; } set { button_toggle.Text = value; } }

        public event EventHandler? StartRequest;
        public event EventHandler? StopRequest;
        public event EventHandler? ToggleRequest;

        public void AllowUse(bool enabled)
        {
            button_toggle.Enabled = enabled;
        }
        public void SetProgress(int percentProgress)
        {
            progressBar_progress.Value = percentProgress;
        }
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
        private void removeWarnings()
        {
            StartRequest?.Invoke(this, new EventArgs());
            StopRequest?.Invoke(this, new EventArgs());
        }
    }
}
