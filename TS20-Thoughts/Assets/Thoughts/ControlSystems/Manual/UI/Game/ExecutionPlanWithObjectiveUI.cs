using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlanWithObjectiveUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the visualization of the objective attribute to cover
    /// </summary>
    [Tooltip("Reference to the visualization of the objective attribute to cover")]
    [SerializeField] private AttributeUI objectiveAttributeUI;
    
    /// <summary>
    /// Reference to the visualization of all the execution plans
    /// </summary>
    [Tooltip("Reference to the visualization of all the execution plans")]
    [SerializeField] private ExecutionPlansUI executionPlansUI;

    /// <summary>
    /// Displays the execution plan 
    /// </summary>
    /// <param name="executionPlans">The ExecutionPlans to display in the UI.</param>
    public void DisplayExecutionPlans(List<ExecutionPlan> executionPlans)
    {
        bool showExecutionPlansUI = executionPlans != null;
        executionPlansUI.gameObject.SetActive(showExecutionPlansUI);
        if (showExecutionPlansUI)
            executionPlansUI.Setup(executionPlans);
    }

    /// <summary>
    /// Displays the objective attribute
    /// </summary>
    /// <param name="newObjectiveAttribute">The AttributeOwnership that is the objective Attribute to display in the UI.</param>
    public void DisplayObjectiveAttribute(AttributeOwnership newObjectiveAttribute)
    {
        bool showObjectiveAttributeUI = newObjectiveAttribute != null;
        objectiveAttributeUI.gameObject.SetActive(showObjectiveAttributeUI);
        if (showObjectiveAttributeUI)
            objectiveAttributeUI.Setup(newObjectiveAttribute);

    }
}
