namespace GFunctions.Process
{
    public class ViewLock
    {
        public bool ViewInUse { get; private set; } = false;

        public event EventHandler<bool>? ViewUseChanged;
        public event EventHandler? ViewReleaseRequest;

        public void StartViewUse()
        {
            ViewInUse = true;
            ViewUseChanged?.Invoke(this, true);
        }
        public void EndViewUse()
        {
            ViewInUse = false;
            ViewUseChanged?.Invoke(this, false);
        }
        public void RequestViewRelease()
        {
            ViewReleaseRequest?.Invoke(this, new EventArgs());
        }
    }
}
