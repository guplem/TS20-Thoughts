using System;
using Thoughts.Game.GameMap;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Attributes
{
    /// <summary>
    /// Requirement for the execution of a MapEvent
    /// </summary>
    [Serializable]
    public class Requirement
    {
        /// <summary>
        /// The required attribute
        /// </summary>
        [Tooltip("The required attribute")]
        public Thoughts.Game.Attributes.Attribute attribute;
        
        /// <summary>
        /// The minimal value required of the required attribute
        /// </summary>
        [Tooltip("The minimal value required of the required attribute")]
        [FormerlySerializedAs("value")]
        public int minValue = 1;
        
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
            return $"{attribute} (val={minValue})";
        }

    }
}
