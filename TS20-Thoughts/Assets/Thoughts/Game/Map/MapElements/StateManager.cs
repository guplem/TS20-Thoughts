using System;
using UnityEngine;

namespace Thoughts.Game.GameMap
{
    /// <summary>
    /// Manages the current state of a MapElement
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// The current state of the owner of this StateManager
        /// </summary>
        public State currentState { get; private set; }
        
        /// <summary>
        /// The remaining time (in seconds) of the current state. When the remining time is 0, the state switches to "None". 
        /// </summary>
        public float remainingStateTime { get; private set; }

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="currentState">The initial state of the owner of this StateManager</param>
        /// <param name="remainingStateTime">The initial remaining time of the initial state of this StateManager</param>
        public StateManager(State currentState = State.None, float remainingStateTime = 0)
        {
            this.currentState = currentState;
            this.remainingStateTime = remainingStateTime;
        }

        /// <summary>
        /// Updates the remaining time in the current State and, if the remaining time is less than 0, the state becomes State.None
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Step(float deltaTime)
        {
            if (currentState == State.None)
                return;
            
            float newRemainingTime = remainingStateTime - deltaTime;
            if (newRemainingTime <= 0)
            {
                SetState(State.None, 0f);
            }
            else
            {
                remainingStateTime = Mathf.Max(newRemainingTime, 0f);
            }
            
            Debug.Log($"Executing 'Step' of StateManager. Current State = {this.ToString()}");
        }

        /// <summary>
        /// Sets a new State with a remaining time to finish it
        /// </summary>
        /// <param name="newState">The new State of the StateManager</param>
        /// <param name="timeInState">The remaining time in the new State</param>
        public void SetState(State newState, float timeInState)
        {
            Debug.Log($"Changing state from {this.ToString()} to '{Enum.GetName(typeof(State), newState)}' with {timeInState} seconds remaining.");
            
            currentState = newState;
            remainingStateTime = timeInState;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"State '{Enum.GetName(typeof(State), currentState)}' with {remainingStateTime} seconds remaining";
        }

    }

    /// <summary>
    /// The current condition of a MapElement.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Can be interrupted. The MapElement is moving, looking for something to do, ...
        /// </summary>
        None, 
        /// <summary>
        /// The MapElement is stopped, in one place waiting comfortably.
        /// </summary>
        Resting,
        /// <summary>
        /// The MapElement is doing an active task to (usually) obtain an object.
        /// </summary>
        Working
    }
}