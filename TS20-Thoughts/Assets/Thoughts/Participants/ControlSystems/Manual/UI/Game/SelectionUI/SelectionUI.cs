using Thoughts.Game.Map.MapElements;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.SelectionUI
{
    /// <summary>
    /// Controls the display of the information related to a MapElement (needs -Attributes-, name, description, objects -Attributes-, ...)
    /// </summary>
    public class SelectionUI : MonoBehaviour
    {

        /// <summary>
        /// Reference to the visualization of the descriptive information of the selected MapElement
        /// </summary>
        [Tooltip("Reference to the visualization of the descriptive information of the selected MapElement")]
        [SerializeField] private DescriptionUI descriptionUI;
        
        /// <summary>
        /// Reference to the visualization of the need of the selected MapElement
        /// </summary>
        [Tooltip("Reference to the visualization of the need of the selected MapElement")]
        [SerializeField] private NeedsUI needsUI;
                
        /// <summary>
        /// Reference to the visualization of the objects of the selected MapElement
        /// </summary>
        [Tooltip("Reference to the visualization of the objects of the selected MapElement")]
        [SerializeField] private ObjectsUI objectsUI;

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
                needsUI.Setup(selectedMapElement);
                objectsUI.Setup(selectedMapElement);
            }
        }
    }
}