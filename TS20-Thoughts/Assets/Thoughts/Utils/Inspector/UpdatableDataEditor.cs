#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Thoughts.Utils.Inspector
{
    [CustomEditor(typeof(UpdatableData), true)]
    public class UpdatableDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UpdatableData data = (UpdatableData) target;

            GUILayout.Space(30f);
            if (GUILayout.Button("Update"))
            {
                data.NotifyOfUpdatedValues();
                EditorUtility.SetDirty(target);
            }
        }
    }
}

#endif