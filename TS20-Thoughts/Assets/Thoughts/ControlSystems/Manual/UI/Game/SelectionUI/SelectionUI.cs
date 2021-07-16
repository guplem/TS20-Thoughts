using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

public class SelectionUI : MonoBehaviour
{

    /// <summary>
    /// Reference to the visualization of the descriptive information of the selected MapElement
    /// </summary>
    [Tooltip("Reference to the visualization of the descriptive information of the selected MapElement")]
    [SerializeField] private DescriptionUI descriptionUI;
    
    /// <summary>
    /// Displays the information regarding the given selected MapElement
    /// </summary>
    /// <param name="selectedMapElement">The MapElement to show the information of</param>
    public void Setup(MapElement selectedMapElement)
    {
        bool showUI = selectedMapElement != null;
        
        gameObject.SetActive(showUI);

        if (showUI)
        {
            descriptionUI.DisplayInformationOf(selectedMapElement);
        }
    }
}
