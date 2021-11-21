using System;
using Sirenix.OdinInspector;
using Thoughts.Game.Map.MapElements.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.MapEvents
{
    /// <summary>
    /// Requirement for the execution of a MapEvent
    /// </summary>
    public class Requirement : PropertyEvaluator
    {
        /// <summary>
        /// The minimal value required of the required property
        /// </summary>
        [Tooltip("The minimal value required of the required property")]
        [HideIf("propertyType", PropertyType.None)]
        public float minValue = 1;
        

        
        /// <summary>
        /// The MapElement that must fulfill the requirement so the MapEvent can be executed
        /// </summary>
        
        // The InfoBox must be after the XML comment of the field even tough it is not related to it... \0.0/
        [InfoBox("It makes no sense to have a PropertyType of 'None' in a requirement. You should require something... Either the holding property (self) or another (other) property.", InfoMessageType.Warning, "IsPropertyTypeNone")]
        [Tooltip("The MapElement that must fulfill the requirement so the MapEvent can be executed")]
        public AffectedMapElement affectedMapElement = AffectedMapElement.eventOwner;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Property: '{property}', minValue = {minValue}, affectedMapElement = {(AffectedMapElement)affectedMapElement}";
        }

    }
}
