namespace HQFramework.UI
{
    public interface IUIGroup
    {
        int GroupID
        {
            get;
        }

        object GroupRoot
        {
            get;
        }

        int Depth
        {
            get;
        }

        void OnFormOpened(IUIForm form);

        void OnFormClosed(IUIForm form);
    }
}
