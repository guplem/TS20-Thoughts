using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using TMPro;
using UnityEngine;

public class ExecutionPlanUI : MonoBehaviour
{
    /// <summary>
    /// A reference to the text component that displays the information regarding the displayed ExecutionPlan
    /// </summary>
    [Tooltip("A reference to the text component that displays the information regarding the displayed ExecutionPlan")]
    [SerializeField] private TMP_Text executionPlanText;
    
    /// <summary>
    /// Displays the given execution plan in the UI 
    /// </summary>
    /// <param name="executionPlan">The ExecutionPlan to display in the UI.</param>
    public void Setup(ExecutionPlan executionPlan)
    {
        bool display = executionPlan != null;
        
        gameObject.SetActive(display);
        
        if (display)
            executionPlanText.text = executionPlan.mapEvent.name;
    }
    
}