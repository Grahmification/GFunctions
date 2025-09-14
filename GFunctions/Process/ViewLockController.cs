namespace GFunctions.Process
{
    public class ViewLockController
    {
        public List<IViewLock> Controllers { get; private set; } = new List<IViewLock>();
        public void AddController(IViewLock controller)
        {
            controller.ViewLocker.ViewUseChanged += onViewUseChanged;
            controller.ViewLocker.ViewReleaseRequest += onViewReleaseRequest;
            Controllers.Add(controller);
        }
        public void RemoveController(IViewLock controller)
        {
            if (Controllers.Contains(controller))
            {
                controller.ViewLocker.ViewUseChanged -= onViewUseChanged;
                controller.ViewLocker.ViewReleaseRequest -= onViewReleaseRequest;
                Controllers.Remove(controller);
            }                
        }
        public void ReleaseAllViews()
        {
            foreach (IViewLock controller in Controllers)
            {
                if (controller.ViewLocker.ViewInUse == true)
                    controller.ReleaseView();
            }
        }


        private void onViewUseChanged(object? sender, bool e)
        {
            if (e == true)
            {
                SetGlobalViewLock();
            }
            else
            {
                ReleaseGlobalViewLock();
            }
        }
        private void onViewReleaseRequest(object? sender, EventArgs e)
        {
            ReleaseAllViews();
        }

        private void SetGlobalViewLock()
        {
            foreach (IViewLock controller in Controllers)
            {
                if (controller.ViewLocker.ViewInUse == false)
                    controller.AllowViewUse(false);
            }
        }
        private void ReleaseGlobalViewLock()
        {
            foreach (IViewLock controller in Controllers)
            {
                controller.AllowViewUse(true);
            }
        }
    }
}
