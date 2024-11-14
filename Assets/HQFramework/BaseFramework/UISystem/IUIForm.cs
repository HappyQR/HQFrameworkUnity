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

        int GroupID
        {
            get;
        }

        bool Visible
        {
            get;
        }

        bool Alive
        {
            get;
        }

        bool Covered
        {
            get;
        }

        object FormObject
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

        protected internal void SetVisible(bool visible);

        protected internal void OnCreate(IUIFormLinker linker);

        protected internal void OnOpen(object userData);

        protected internal void OnUpdate();

        protected internal void OnCovered();

        protected internal void OnRevealed();

        protected internal void OnClose();

        protected internal void OnDestroy();
    }
}
