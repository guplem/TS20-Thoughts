using System.Collections.Generic;
using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using TMPro;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    
    public class NeedGroupUI : MonoBehaviour
    {
        
        /// <summary>
        /// A reference to the text component that displays the name of this group/type/level of needs
        /// </summary>
        [Tooltip("A reference to the text component that displays the name of this group/type/level of needs")]
        [SerializeField] private TMP_Text mapElementNameText;
        
        /// <summary>
        /// A reference to the GameObject that will be the parent of the contained AttributeUIs (needs of the group)
        /// </summary>
        [Tooltip("A reference to the GameObject that will be the parent of the contained AttributeUIs (needs of the group)")]
        [SerializeField] private GameObject attributesArea;
        
        /// <summary>
        /// The prefab used to display an attribute
        /// </summary>
        [Tooltip("The prefab used to display an attribute")]
        [SerializeField] private GameObject attributeUIPrefab;

        /// <summary>
        /// A list holding a reference to all AttributeUI elements currently existent in this group
        /// </summary>
        private List<AttributeUI> attributeUIs = new List<AttributeUI>();

        /// <summary>
        /// Displays the attributes of the given MapElement, with the given priority, using as header the given name
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to show the needs of </param>
        /// <param name="groupNeedPriority">The priority of the attributes to display in this group</param>
        /// <param name="groupName">The displayed text in the header of the need group</param>
        public void Setup(MapElement selectedMapElement, Attribute.NeedPriority groupNeedPriority, string groupName)
        {
            mapElementNameText.text = groupName;

            List<AttributeOwnership> needs = selectedMapElement.attributeManager.GetAttributesWithPriority(groupNeedPriority);
            
            if (needs == null)
                return;

            Debug.Log($"Setting up UI for group of needs (Attributes) with length {needs.Count}:\n    ● {needs.ToStringAllElements("\n    ● ")}\n", gameObject);

            // Instantiate missing UI elements
            int missingUIElements = needs.Count - attributeUIs.Count;
            for (int e = 0; e < missingUIElements; e++)
            {
                GameObject spawnedAttributeUI = Instantiate(attributeUIPrefab, attributesArea.transform);
                AttributeUI attributeUI = spawnedAttributeUI.GetComponentRequired<AttributeUI>();
                attributeUIs.Add(attributeUI);
            }

            // Configure the UI elements
            for (int e = 0; e < attributeUIs.Count; e++)
            {
                AttributeOwnership attributeOwnershipToDisplay = needs.Count > e ? needs[e] : null;
                attributeUIs[e].Setup(attributeOwnershipToDisplay);
            }

        }
        
        
    }
}