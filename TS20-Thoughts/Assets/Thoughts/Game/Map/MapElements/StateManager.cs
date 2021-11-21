using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements
{
    /// <summary>
    /// Manages the current State of a MapElement
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// The current State of the owner of this StateManager
        /// </summary>
        public MapElementState currentState { get; private set; }
        public string currentStateName => currentState.stateTypeName;

        /// <summary>
        /// The MapElement hosting this StateManager. 
        /// </summary>
        private MapElement owner;

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="owner">The MapElement hosting this StateManager</param>
        /// <param name="currentState">The initial State of the owner of this StateManager</param>
        public StateManager(MapElement owner, MapElementState currentState)
        {
            this.owner = owner;
            this.currentState = currentState;
        }

        /// <summary>
        /// Updates the remaining time in the current State and, if the remaining time is less than 0, the State becomes StateType 'None'
        /// </summary>
        /// <param name="deltaTime">The elapsed time from the previous step</param>
        public void Step(float deltaTime)
        {
            if (currentState.stateType == StateType.None)
                return;

            UpdatedState(currentState.UpdateRemainingTime(-deltaTime));
            if (currentState.remainingTime <= 0f)
                UpdatedState(new MapElementState(StateType.None));

            // Debug.Log($"Executing 'Step' of StateManager. Current StateType = {this.ToString()}");
        }

        /// <summary>
        /// Sets a new StateType with a remaining time to finish it and plays the animation of the given StateType at this StateManager owner MapElement
        /// </summary>
        /// <param name="newState">The new state of the StateManager's MapElement</param>
        public void UpdatedState(MapElementState newState)
        {
            currentState = newState;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return currentState.ToString();
        }

    }

    [Serializable]
    public struct MapElementState
    {
        public StateType stateType;
        [HideIf("stateType", StateType.None)]
        [Range(0f,30f)]
        [SerializeField] public float duration; //In seconds

        public MapElementState(StateType stateType, float duration = 0)
        {
            this.stateType = stateType;
            this.duration = duration;
        }
        
        public float remainingTime => duration;
        public string stateTypeName => Enum.GetName(typeof(StateType), stateType);

        /// <summary>
        /// Updates (increasing or decreasing) the remaining duration of this state
        /// </summary>
        /// <param name="deltaTime">The time to increase or decrease from the remaining time of this state</param>
        /// <returns></returns>
        public MapElementState UpdateRemainingTime(float deltaTime)
        {
            float newRemainingTime = duration + deltaTime;
            return new MapElementState(this.stateType, newRemainingTime);
        }

        private bool IsDurationZero()
        {
            return duration == 0f;
        }
        
        public override string ToString()
        {
            return $"State of type '{Enum.GetName(typeof(StateType), stateType)}' with {duration}s remaining.";
        }
        
    }
    
    /// <summary>
    /// The current condition of a MapElement.
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// Can be interrupted. The MapElement is moving, looking for something to do, chilling, ...
        /// </summary>
        None = 0, 
        /// <summary>
        /// The MapElement is inactive/stopped/resting, in one place waiting comfortably, turned off, ...
        /// </summary>
        Resting = 1,
        /// <summary>
        /// The MapElement is doing an active/working task to (usually) obtain an object, being turn on (bonfire...).
        /// </summary>
        Active = 2,
    }

}