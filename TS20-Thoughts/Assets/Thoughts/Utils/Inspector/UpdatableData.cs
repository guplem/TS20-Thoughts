using UnityEditor;
using UnityEngine;

namespace Thoughts.Utils.Inspector
{
    public class UpdatableData : ScriptableObject
    {
        public event System.Action OnValuesUpdated;
        public bool autoUpdate;
        private bool recompilationDone = false;

        public void ClearOnValuesUpdated()
        {
            OnValuesUpdated = null;
        }
        
    #if UNITY_EDITOR
        
        void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        }

        public void OnBeforeAssemblyReload()
        {
            //Debug.Log("Before Assembly Reload");
            recompilationDone = false;
        }

        public void OnAfterAssemblyReload()
        {
            //Debug.Log("After Assembly Reload");
            recompilationDone = true;
        }
        
        protected virtual void OnValidate() {
            if (recompilationDone && autoUpdate && !Application.isPlaying)
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

