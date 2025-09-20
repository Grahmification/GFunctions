namespace GFunctions.Process
{
    /// <summary>
    /// Controller that manages locking/unlocking views when a long running process is running that must block other things
    /// </summary>
    public class ViewLockController
    {
        
        /// <summary>
        /// The list of items that support locking out their view, or have long running processes
        /// </summary>
        public List<IViewLock> Controllers { get; private set; } = [];
        
        /// <summary>
        /// Adds a controller to be controlled with viewlocking
        /// </summary>
        /// <param name="controller">The controller to add</param>
        public void AddController(IViewLock controller)
        {
            controller.ViewLocker.ViewUseChanged += OnViewUseChanged;
            controller.ViewLocker.ViewReleaseRequest += OnViewReleaseRequest;
            Controllers.Add(controller);
        }

        /// <summary>
        /// Removes a controller from ing controlled with viewlocking
        /// </summary>
        /// <param name="controller">The controller to remove</param>
        public void RemoveController(IViewLock controller)
        {
            if (Controllers.Contains(controller))
            {
                controller.ViewLocker.ViewUseChanged -= OnViewUseChanged;
                controller.ViewLocker.ViewReleaseRequest -= OnViewReleaseRequest;
                Controllers.Remove(controller);
            }                
        }
        
        /// <summary>
        /// Stops long running processes on all views, for example if a global cancel/error occurs
        /// </summary>
        public void ReleaseAllViews()
        {
            foreach (IViewLock controller in Controllers)
            {
                if (controller.ViewLocker.ViewInUse)
                    controller.ReleaseView();
            }
        }

        /// <summary>
        /// Fires when a view notifies that a long running process has started or stopped
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">True if a long running process has started</param>
        private void OnViewUseChanged(object? sender, bool e)
        {
            if (e)
            {
                SetGlobalViewLock();
            }
            else
            {
                ReleaseGlobalViewLock();
            }
        }

        /// <summary>
        /// Fires when a view requests that all others stop their long running processes
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnViewReleaseRequest(object? sender, EventArgs e)
        {
            ReleaseAllViews();
        }

        /// <summary>
        /// Locks out all views, except the one with the long running process
        /// </summary>
        private void SetGlobalViewLock()
        {
            foreach (IViewLock controller in Controllers)
            {
                if (!controller.ViewLocker.ViewInUse)
                    controller.AllowViewUse(false);
            }
        }
        
        /// <summary>
        /// Unlocks all views, allowing user input
        /// </summary>
        private void ReleaseGlobalViewLock()
        {
            foreach (IViewLock controller in Controllers)
            {
                controller.AllowViewUse(true);
            }
        }
    }
}
