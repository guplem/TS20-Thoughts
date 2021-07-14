using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlansUI : MonoBehaviour
{

    [SerializeField] private GameObject executionPlanUIPrefab;
    private List<ExecutionPlanUI> executionPlanUIs = new List<ExecutionPlanUI>();
    
    public void Setup(List<ExecutionPlan> mapElementExecutionPlans)
    {
        if (mapElementExecutionPlans == null)
            return;

        // Spawn missing ones
        for (int e = 0; e < mapElementExecutionPlans.Count - executionPlanUIs.Count; e++)
        {
            GameObject spawnedExecutionPlanUI = Instantiate(executionPlanUIPrefab, this.transform);
            ExecutionPlanUI executionPlanUI = spawnedExecutionPlanUI.GetComponentRequired<ExecutionPlanUI>();
            executionPlanUIs.Add(executionPlanUI);
        }
        
        // Configure the existing
        for (int e = 0; e < executionPlanUIs.Count; e++)
        {
            if (mapElementExecutionPlans.Count > e)
            {
                executionPlanUIs[e].Setup(mapElementExecutionPlans[e]);
            }
            else
            {
                executionPlanUIs[e].gameObject.SetActive(false);
            }
        }
        
    }
}
