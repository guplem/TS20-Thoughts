using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using TMPro;
using UnityEngine;

public class ExecutionPlanUI : MonoBehaviour
{
    
    private ExecutionPlan executionPlan;
    [SerializeField] private TMP_Text executionPlanText;
    
    public void Setup(ExecutionPlan executionPlan)
    {
        this.executionPlan = executionPlan;
        executionPlanText.text = executionPlan.mapEvent.name;
    }
    
}
