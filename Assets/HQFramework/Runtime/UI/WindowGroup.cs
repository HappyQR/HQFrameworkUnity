using UnityEngine;

namespace HQFramework.Runtime
{
    public class WindowGroup
    {
        private Transform root;
        private int id;
        private string name;

        public int ID => id;
        public string Name => name;
        public Transform Root => root;

        public WindowGroup(GameObject groupRootObject, int id, string name)
        {
            this.root = groupRootObject.transform;
            this.id = id;
            this.name = name;
        }
    }
}
