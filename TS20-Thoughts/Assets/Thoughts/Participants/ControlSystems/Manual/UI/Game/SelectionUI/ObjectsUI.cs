using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.SelectionUI
{
    public class ObjectsUI : SimpleAnimationUIElement
    {
        /// <summary>
        /// The MapElement to show the information of 
        /// </summary>
        private MapElement selectedMapElement;
        
        /// <summary>
        /// The last map element from which the in
        /// </summary>
        private MapElement lastShownMapElement;

        /// <summary>
        /// The prefab used to display an property
        /// </summary>
        [Tooltip("The prefab used to display an property")]
        [SerializeField] private GameObject propertyUIPrefab;

        /// <summary>
        /// A list holding a reference to all PropertyUI elements currently existent in this group
        /// </summary>
        private List<PropertyUI> propertyUIs = new List<PropertyUI>();
        
        /// <summary>
        /// A reference to the GameObject that will be the parent of the contained PropertyUIs 
        /// </summary>
        [Tooltip("A reference to the GameObject that will be the parent of the contained PropertyUIs")]
        [SerializeField] private GameObject propertiesArea;
        
        /// <summary>
        /// Prepares the NeedUI to be shown
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to show the information of</param>
        public void Setup(MapElement selectedMapElement)
        {
            this.selectedMapElement = selectedMapElement;
        }

        /// <summary>
        /// Populates the NeedGroupUIs with all the needed elements
        /// </summary>
        public void DisplayObjects()
        {
            if (lastShownMapElement != selectedMapElement)
            {
                lastShownMapElement = selectedMapElement;

                List<PropertyOwnership> needs = selectedMapElement.propertyManager.GetPropertiesWithPriority(Property.NeedPriority.None);
            
                if (needs == null)
                    return;

                Debug.Log($"Setting up UI for group of needs (Properties) with length {needs.Count}:\n    ● {needs.ToStringAllElements("\n    ● ")}\n", gameObject);

                // Instantiate missing UI elements
                int missingUIElements = needs.Count - propertyUIs.Count;
                for (int e = 0; e < missingUIElements; e++)
                {
                    GameObject spawnedPropertyUI = Instantiate(propertyUIPrefab, propertiesArea.transform);
                    PropertyUI propertyUI = spawnedPropertyUI.GetComponentRequired<PropertyUI>();
                    propertyUIs.Add(propertyUI);
                }

                // Configure the UI elements
                for (int e = 0; e < propertyUIs.Count; e++)
                {
                    PropertyOwnership propertyOwnershipToDisplay = needs.Count > e ? needs[e] : null;
                    propertyUIs[e].Setup(propertyOwnershipToDisplay);
                }
            }
        }

        /// <summary>
        /// Plays the Show or the Hide animation depending on the current state of the UI Element. If state is Show, it plays the Hide animation and vice-versa.
        /// </summary>
        /// <param name="initialStateIsShowing">Should the fist call set the UI Element to the state "hide"?</param>
        public new void SwitchState(bool initialStateIsShowing = false)
        {
            base.SwitchState(initialStateIsShowing);
            
            if (state == VisualizationState.Showing)
                DisplayObjects();
        }
        
    }
}
