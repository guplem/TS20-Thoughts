using UnityEditor;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps
{
    public abstract class CreationStepGenerator : MonoBehaviour
    {
        /// <summary>
        /// Reference to the MapManager managing the world
        /// </summary>
        [Tooltip("Reference to the MapManager managing the world")]
        [SerializeField] protected MapManager mapManager;
    
        [SerializeField] private CreationStepGenerator nextCreationStep;
    
        protected bool clearPreviousBeforeGeneration;
        protected bool generateNextStepOnFinishGeneration = false;
        protected bool deleteNextStepOnFinishDeletion = false;

        private event System.Action OnFinishStepDeletion;
        private event System.Action OnFinishStepGeneration;
    
    
    
        [ContextMenu("Delete Step")]
        private void DeleteStep() // parameter-less method needed for context menu
        {
            Delete(false);
        }
    
        [ContextMenu("Regenerate Step")]
        private void Regenerate() // parameter-less method needed for context menu
        {
            Generate(true, false);
        }
    
    
    
        public void Delete(bool deleteNextStepOnFinish = false)
        {
            this.deleteNextStepOnFinishDeletion = deleteNextStepOnFinish;
            if (deleteNextStepOnFinishDeletion)
            {
                OnFinishStepDeletion += DeleteNextStep;
                //Debug.Log($"{gameObject.name} will generate the next step after finishing", this);
            }
        
            Debug.Log($"Deleting step {this.name}. deleteNextStepOnFinish = {deleteNextStepOnFinish}", this);
            _DeleteStep();
        
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                EditorUtility.SetDirty(mapManager.gameObject);
            }
        #endif
        }
    
        public void Generate(bool clearPrevious = true, bool generateNextStepOnFinish = false)
        {
            this.generateNextStepOnFinishGeneration = generateNextStepOnFinish;
            this.clearPreviousBeforeGeneration = clearPrevious;
            if (generateNextStepOnFinishGeneration)
            {
                OnFinishStepGeneration += GenerateNextStep;
                //Debug.Log($"{gameObject.name} will generate the next step after finishing", this);
            }
            Debug.Log($"Generating step {this.name}. clearPrevious = {clearPrevious}. generateNextStepOnFinish = {generateNextStepOnFinish}", this);
        
            _GenerateStep(clearPrevious);
        
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                EditorUtility.SetDirty(mapManager.gameObject);
            }
        #endif
        }
    
    
    
        protected abstract void _DeleteStep(); // Do not call directly, instead call "Delete()"
        protected abstract void _GenerateStep(bool clearPrevious); // Do not call directly, instead call "Generate(clearPrevious, generateNextStepOnFinish)"

    
    
        protected void InvokeOnFinishStepDeletion()
        {
            Debug.Log($"Step {this.name} finished deletion.", this);
            OnFinishStepDeletion?.Invoke();
            OnFinishStepDeletion -= DeleteNextStep;
        }
    
        protected void InvokeOnFinishStepGeneration()
        {
            Debug.Log($"Step {this.name} finished generation.", this);
            OnFinishStepGeneration?.Invoke();
            OnFinishStepGeneration -= GenerateNextStep;
        }
    
    
    
        private void DeleteNextStep()
        {
            DeleteNextStep(this.deleteNextStepOnFinishDeletion);
        }
    
        private void GenerateNextStep()
        {
            GenerateNextStep(this.clearPreviousBeforeGeneration, this.generateNextStepOnFinishGeneration);
        }
    
    
    
        private void GenerateNextStep(bool clearPrevious, bool generateNextOnFinish)
        {
            Debug.Log($"Triggering the generation of the step after {this.name}: {(nextCreationStep != null? nextCreationStep.name : "null")}", nextCreationStep);
            if (nextCreationStep != null)
                nextCreationStep.Generate(clearPrevious, generateNextOnFinish);
            else
                Debug.LogWarning($"Trying to generate the next step of {this.name}, with no 'nextCreationStepGenerator' defined", this);
        }
    
        private void DeleteNextStep(bool deleteOnFinish)
        {
            Debug.Log($"Triggering the deletion of the step after {this.name}: {(nextCreationStep != null? nextCreationStep.name : "null")}", nextCreationStep);
            if (nextCreationStep != null)
                nextCreationStep.Delete(deleteOnFinish);
            else
                Debug.LogWarning($"Trying to delete the next step of {this.name}, with no 'nextCreationStepGenerator' defined", this);
        }
    }
}
