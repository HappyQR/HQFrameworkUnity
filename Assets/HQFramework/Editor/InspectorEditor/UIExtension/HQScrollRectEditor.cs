using HQFramework.Runtime;
using UnityEditor;
using UnityEditor.UI;

namespace HQFramework.Editor
{
    [CustomEditor(typeof(HQScrollRect), true)]
    [CanEditMultipleObjects]
    public class HQScrollRectEditor : ScrollRectEditor
    {
        private SerializedProperty onScroll;

        protected override void OnEnable()
        {
            base.OnEnable();
            onScroll = serializedObject.FindProperty("onScroll");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(onScroll);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
