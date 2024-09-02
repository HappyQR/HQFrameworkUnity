using System.Collections;


namespace HQFramework.Coroutine
{
    internal partial class CoroutineManager : HQModuleBase, ICoroutineManager
    {
        public override byte Priority => byte.MaxValue;
    }
}
