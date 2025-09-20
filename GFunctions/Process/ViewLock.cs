namespace GFunctions.Process
{
    /// <summary>
    /// A class that controls locking out an individual view, reporting back to the <see cref="ViewLockController"/> about long running processes
    /// </summary>
    public class ViewLock
    {
        /// <summary>
        /// Returns true if the view is in active use with a long running process
        /// </summary>
        public bool ViewInUse { get; private set; } = false;

        /// <summary>
        /// Notifies that the view has started being in use, potentially locking or releasing other views
        /// </summary>
        public event EventHandler<bool>? ViewUseChanged;

        /// <summary>
        /// Alerts any other views that have an active long running process to stop as soon as possible
        /// </summary>
        public event EventHandler? ViewReleaseRequest;

        /// <summary>
        /// Start using this view with a long running process, potentially locking out other views
        /// </summary>
        public void StartViewUse()
        {
            ViewInUse = true;
            ViewUseChanged?.Invoke(this, true);
        }

        /// <summary>
        /// End using this view with a long running process, potentially unlocking other views
        /// </summary>
        public void EndViewUse()
        {
            ViewInUse = false;
            ViewUseChanged?.Invoke(this, false);
        }

        /// <summary>
        /// Requests any other views that have an active long running process to stop as soon as possible
        /// </summary>
        public void RequestViewRelease()
        {
            ViewReleaseRequest?.Invoke(this, new EventArgs());
        }
    }
}
