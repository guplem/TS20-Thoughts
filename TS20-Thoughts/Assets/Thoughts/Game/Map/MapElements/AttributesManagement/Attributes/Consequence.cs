using System;
using Thoughts.Game.GameMap;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Attributes
{
    /// <summary>
    /// Consequence of the execution of a MapEvent
    /// </summary>
    [Serializable]
    public class Consequence
    {
        /// <summary>
        /// Attribute that will be updated as a consequence of the execution of the MapEvent
        /// </summary>
        [Tooltip("Attribute that will be updated as a consequence of the execution of the MapEvent")]
        public Thoughts.Game.Attributes.Attribute attribute;
        
        /// <summary>
        /// The difference to apply to the attribute's value of this consequence. Can be positive and negative
        /// </summary>
        [Tooltip("The difference to apply to the attribute's value of this consequence. Can be positive and negative")]
        [FormerlySerializedAs("value")]
        public int deltaValue = 1;
        
        /// <summary>
        /// The MapElement that will get the effects of this consequence
        /// </summary>
        [Tooltip("The MapElement that will get the effects of this consequence")]
        public AffectedMapElement affectedMapElement = AffectedMapElement.eventOwner;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{attribute} (val={deltaValue})";
        }

    }
}
