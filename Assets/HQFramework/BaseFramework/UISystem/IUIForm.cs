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

        void SetVisible(bool visible);

        void OnCreate(IUIFormLinker linker);

        void OnOpen(object userData);

        void OnUpdate();

        void OnCovered();

        void OnRevealed();

        void OnClose();

        void OnDestroy();
    }
}
