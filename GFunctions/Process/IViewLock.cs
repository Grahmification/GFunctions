namespace GFunctions.Process
{
    public interface IViewLock
    {
        ViewLock ViewLocker { get; }
        void ReleaseView();
        void AllowViewUse(bool enabled);
    }
}
