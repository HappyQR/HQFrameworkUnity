using System.Collections;

namespace HQFramework.Coroutine
{
    public interface ICoroutineManager
    {
        int StartCoroutine(IEnumerator func, int groupID, int priority);

        bool StopCoroutine(int id);
    }
}
