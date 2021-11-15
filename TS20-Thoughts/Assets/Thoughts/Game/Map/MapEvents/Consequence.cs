using System;
using Sirenix.OdinInspector;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.MapEvents
{
    /// <summary>
    /// Consequence of the execution of a MapEvent
    /// </summary>
    public class Consequence : PropertyEvaluator
    {
        /// <summary>
        /// The difference to apply to the property's value of this consequence. Can be positive and negative
        /// </summary>
        [Tooltip("The difference to apply to the property's value of this consequence. Can be positive and negative")]
        [FormerlySerializedAs("value")]
        public float deltaValue = 1;
        
        /// <summary>
        /// The MapElement that will get the effects of this consequence. Be aware that if chosen 'target', it can not be the 'executer' neither the 'owner' of the event.
        /// </summary>
        [Tooltip("The MapElement that will get the effects of this consequence. Be aware that if chosen 'target', it can not be the 'executer' neither the 'owner' of the event")]
        public AffectedMapElement affectedMapElement = AffectedMapElement.eventOwner;

        /// <summary>
        /// Should a new state be set in the affected MapElement?
        /// </summary>
        [Tooltip("Should a new state be set in the affected MapElement?")]
        public bool setNewState = false;
        
        /// <summary>
        /// The new State and duration of it for the affected MapElement
        /// </summary>
        [ShowIf("setNewState")]
        [Tooltip("The new State and duration of it for the affected MapElement")]
        public StateUpdate stateUpdate;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{property} (val={deltaValue})";
        }
        
    }

    /// <summary>
    /// An update of the State of a MapElement
    /// </summary>
    [Serializable]
    public class StateUpdate
    {
        /// <summary>
        /// The new state of the affected MapElement
        /// </summary>
        public State newState = State.Active;
        
        /// <summary>
        /// The duration of the new state of the affected MapElement
        /// </summary>
        public float newStateDuration = 2;
    }
}
