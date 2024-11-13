namespace HQFramework.UI
{
    public interface IUIForm
    {
        string AssetPath
        {
            get;
        }

        uint AssetCrc
        {
            get;
        }

        object FormHandle
        {
            get;
        }

        int GroupID
        {
            get;
        }

        int SortLayer
        {
            get;
        }

        bool DestroyOnClose
        {
            get;
        }

        bool PauseOnCovered
        {
            get;
        }

        bool Visible
        {
            get;
        }

        bool Covered
        {
            get;
        }

        protected internal void OnCreate(object formObject);

        protected internal void OnOpen();

        protected internal void OnUpdate();

        protected internal void OnCovered();

        protected internal void OnClose();

        protected internal void OnDestroy();
    }
}
