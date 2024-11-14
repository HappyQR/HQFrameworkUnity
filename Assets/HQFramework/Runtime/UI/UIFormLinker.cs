using HQFramework.UI;
using UnityEngine;

namespace HQFramework.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIFormLinker : MonoBehaviour, IUIFormLinker
    {
        [SerializeField]
        private bool destroyOnClose = false;

        [SerializeField]
        private bool pauseOnCovered = true;

        [SerializeField]
        internal RectTransform[] linkedElements;

        public bool DestroyOnClose => destroyOnClose;

        public bool PauseOnCovered => pauseOnCovered;

        public object FormObject => gameObject;
    }
}
