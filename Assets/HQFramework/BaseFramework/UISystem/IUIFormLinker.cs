namespace HQFramework.UI
{
    public interface IUIFormLinker
    {
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
    }
}
