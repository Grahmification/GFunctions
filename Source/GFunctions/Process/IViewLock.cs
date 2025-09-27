namespace GFunctions.Process
{
    
    /// <summary>
    /// Generic definition for a controller connected to a view that supports locking out
    /// </summary>
    public interface IViewLock
    {
        /// <summary>
        /// The local object that controls reporting back to the <see cref="ViewLockController"/>
        /// </summary>
        ViewLock ViewLocker { get; }

        /// <summary>
        /// Cancels any long running processes associated with the connected view
        /// </summary>
        void ReleaseView();
        
        /// <summary>
        /// Enables or disables use of the connected view
        /// </summary>
        /// <param name="enabled">True to enable the view</param>
        void AllowViewUse(bool enabled);
    }
}
