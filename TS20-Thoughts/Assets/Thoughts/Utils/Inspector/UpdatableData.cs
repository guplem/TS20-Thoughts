using UnityEngine;

namespace Thoughts.Utils.Inspector
{
    public class UpdatableData : ScriptableObject
    {
        public event System.Action OnValuesUpdated;
        public bool autoUpdate;
        
        public void ClearOnValuesUpdated()
        {
            OnValuesUpdated = null;
        }
        
    #if UNITY_EDITOR
        protected virtual void OnValidate() {
            if (autoUpdate && !Application.isPlaying)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
        }

        public void NotifyOfUpdatedValues()
        {
            
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            
            if (OnValuesUpdated != null && !Application.isPlaying) {
                OnValuesUpdated ();
            }
        }
    #endif
    }
}

