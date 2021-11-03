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
        Debug.Log($"Triggering the generation of the step after {this.name}: {(nextCreationStepGenerator != null? nextCreationStepGenerator.name : "null")}");
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
            //Debug.Log($"{gameObject.name} will generate the next step after finishing", this);
        }
        Debug.Log($"Generating step {this.name}. clearPrevious = {clearPrevious}. generateNextStepOnFinish = {generateNextStepOnFinish}", this);
        GenerateStep(clearPrevious);
    }

    protected abstract void GenerateStep(bool clearPrevious);

    private event System.Action OnFinishStepGeneration;
    protected virtual void InvokeOnFinishStepGeneration()
    {
        Debug.Log($"Step {this.name} finished generation.", this);
        OnFinishStepGeneration?.Invoke();
        OnFinishStepGeneration -= GenerateNextStep;
    }
}
