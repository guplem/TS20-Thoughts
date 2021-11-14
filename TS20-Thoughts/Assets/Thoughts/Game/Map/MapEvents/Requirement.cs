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
    [Serializable]
    public class Requirement
    {
        /// <summary>
        /// The required property
        /// </summary>
        [FormerlySerializedAs("property")]
        [Tooltip("The required property")]
        [AssetsOnly]
        public Property property;
        
        /// <summary>
        /// The minimal value required of the required property
        /// </summary>
        [Tooltip("The minimal value required of the required property")]
        [FormerlySerializedAs("value")]
        public float minValue = 1;
        
        /// <summary>
        /// The MapElement that must fulfill the requirement so the MapEvent can be executed
        /// </summary>
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