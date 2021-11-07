using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared;
using TMPro;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.SelectionUI
{
    
    public class NeedGroupUI : MonoBehaviour
    {
        
        /// <summary>
        /// A reference to the text component that displays the name of this group/type/level of needs
        /// </summary>
        [Tooltip("A reference to the text component that displays the name of this group/type/level of needs")]
        [SerializeField] private TMP_Text mapElementNameText;
        
        /// <summary>
        /// A reference to the GameObject that will be the parent of the contained PropertyUIs (needs of the group)
        /// </summary>
        [Tooltip("A reference to the GameObject that will be the parent of the contained PropertyUIs (needs of the group)")]
        [SerializeField] private GameObject propertiesArea;
        
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
        /// Displays the properties of the given MapElement, with the given priority, using as header the given name
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to show the needs of </param>
        /// <param name="groupNeedPriority">The priority of the properties to display in this group</param>
        /// <param name="groupName">The displayed text in the header of the need group</param>
        public void Setup(MapElement selectedMapElement, Property.NeedPriority groupNeedPriority, string groupName)
        {
            mapElementNameText.text = groupName;

            List<PropertyOwnership> needs = selectedMapElement.propertyManager.GetPropertiesWithPriority(groupNeedPriority);
            
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
}