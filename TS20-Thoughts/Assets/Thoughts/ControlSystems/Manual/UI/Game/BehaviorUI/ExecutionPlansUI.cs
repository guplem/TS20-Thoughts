using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlansUI : MonoBehaviour
{

    /// <summary>
    /// The prefab used to display an execution plan
    /// </summary>
    [Tooltip("The prefab used to display an execution plan")]
    [SerializeField] private GameObject executionPlanUIPrefab;
    
    /// <summary>
    /// A list holding a reference to all ExecutionPlanUI elements currently existent
    /// </summary>
    private List<ExecutionPlanUI> executionPlanUIs = new List<ExecutionPlanUI>();
    
    /// <summary>
    /// Displays the given ExecutionPlans in the UI 
    /// </summary>
    /// <param name="executionPlans">The ExecutionPlans to display in the UI.</param>
    public void Setup(List<ExecutionPlan> executionPlans)
    {
        if (executionPlans == null)
            return;

        Debug.Log($"Setting up UI for list of execution plans with length {executionPlans.Count}:\n    ● {executionPlans.ToStringAllElements("\n    ● ")}\n", gameObject);
        
        // Instantiate missing UI elements
        int missingUIElements = executionPlans.Count - executionPlanUIs.Count;
        for (int e = 0; e < missingUIElements; e++)
        {
            GameObject spawnedExecutionPlanUI = Instantiate(executionPlanUIPrefab, this.transform);
            ExecutionPlanUI executionPlanUI = spawnedExecutionPlanUI.GetComponentRequired<ExecutionPlanUI>();
            executionPlanUIs.Add(executionPlanUI);
        }
        
        // Configure the UI elements
        for (int e = 0; e < executionPlanUIs.Count; e++)
        {
            ExecutionPlan executionPlanToDisplay = executionPlans.Count > e ? executionPlans[e] : null;
            executionPlanUIs[e].Setup(executionPlanToDisplay);
        }
        
    }
}