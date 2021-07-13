using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class ExecutionPlanWithObjectiveUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the visualization of the objective attribute to cover
    /// </summary>
    [Tooltip("Reference to the visualization of the objective attribute to cover")]
    [SerializeField] private AttributeUI objectiveAttribute;
    
    /// <summary>
    /// Reference to the visualization of all the execution plans
    /// </summary>
    [Tooltip("Reference to the visualization of all the execution plans")]
    [SerializeField] private ExecutionPlansUI executionPlans;

    /// <summary>
    /// Displays the objective attribute with the execution plan related to the given MapElement.
    /// </summary>
    /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
    public void Setup(MapElement mapElement)
    {
        bool showMapElementUI = mapElement != null;
        
        this.gameObject.SetActive(showMapElementUI);

        if (showMapElementUI)
        {
            objectiveAttribute.Setup(mapElement.attributeOwnershipToCover);
            executionPlans.Setup(mapElement.executionPlans);
        }
    }
}
