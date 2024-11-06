namespace HQFramework.UI
{
    public class UIFormShowCompleteEventArgs : IReference
    {
        public IUIForm UIForm
        {
            get;
            private set;
        }

        public static UIFormShowCompleteEventArgs Create(IUIForm uiForm)
        {
            UIFormShowCompleteEventArgs args = ReferencePool.Spawn<UIFormShowCompleteEventArgs>();
            args.UIForm = uiForm;
            return args;
        }

        void IReference.OnRecyle()
        {
            UIForm = null;
        }
    }
}
