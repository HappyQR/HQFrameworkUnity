using UnityEngine;

namespace HQFramework.Runtime
{
    public abstract class BaseComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameEntry.RegisterModule(this);
        }
    }
}
