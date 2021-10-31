using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreationStepGenerator : MonoBehaviour
{
    [SerializeField] private CreationStepGenerator nextCreationStepGenerator;
    protected bool clearPreviousAtGeneration;
    protected bool generateNextStepOnFinish = false;

    private void GenerateNextStep()
    {
        GenerateNextStep(this.clearPreviousAtGeneration, this.generateNextStepOnFinish);
    }
    
    private void GenerateNextStep(bool clearPrevious, bool generateNextOnFinish)
    {
        if (nextCreationStepGenerator != null)
            nextCreationStepGenerator.Generate(clearPrevious, generateNextOnFinish);
        else
            Debug.LogWarning($"Trying to generate the next step of {this.name}, with no 'nextCreationStepGenerator' defined");
    }

    public void Generate(bool clearPrevious, bool generateNextStepOnFinish)
    {
        this.generateNextStepOnFinish = generateNextStepOnFinish;
        this.clearPreviousAtGeneration = clearPrevious;
        if (generateNextStepOnFinish)
        {
            OnFinishStepGeneration += GenerateNextStep;
            Debug.Log($"{gameObject.name} will generate the next step after finishing", this);
        }
        GenerateStep(clearPrevious, generateNextStepOnFinish);
    }

    protected abstract void GenerateStep(bool clearPrevious, bool generateNextStepOnFinish);

    private event System.Action OnFinishStepGeneration;
    protected virtual void InvokeOnFinishStepGeneration()
    {
        Debug.Log("InvokeOnFinishStepGeneration");
        OnFinishStepGeneration?.Invoke();
        OnFinishStepGeneration -= GenerateNextStep;
    }
}
