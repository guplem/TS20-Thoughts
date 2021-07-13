using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlansUI : MonoBehaviour
{

    [SerializeField] private GameObject executionPlanUIPrefab;
    
    public void Setup(List<ExecutionPlan> mapElementExecutionPlans)
    {
        if (mapElementExecutionPlans == null)
            return;
        
        foreach (ExecutionPlan executionPlan in mapElementExecutionPlans)
        {
            GameObject spawnedExecutionPlanUI = Instantiate(executionPlanUIPrefab, this.transform);
            ExecutionPlanUI executionPlanUI = spawnedExecutionPlanUI.GetComponentRequired<ExecutionPlanUI>();
            executionPlanUI.Setup(executionPlan);
        }
    }
}
