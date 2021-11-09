using System;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements
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
        /// The MapElement hosting this StateManager. 
        /// </summary>
        private MapElement owner;

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="owner">The MapElement hosting this StateManager</param>
        /// <param name="currentState">The initial state of the owner of this StateManager</param>
        /// <param name="remainingStateTime">The initial remaining time of the initial state of this StateManager</param>
        public StateManager(MapElement owner, State currentState = State.None, float remainingStateTime = 0)
        {
            this.owner = owner;
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
            
            // Debug.Log($"Executing 'Step' of StateManager. Current State = {this.ToString()}");
        }

        /// <summary>
        /// Sets a new State with a remaining time to finish it and plays the animation of the given state at this StateManager owner MapElement
        /// </summary>
        /// <param name="newState">The new State of the StateManager</param>
        /// <param name="timeInState">The remaining time in the new State</param>
        public void SetState(State newState, float timeInState)
        {
            if (newState == currentState)
                Debug.LogWarning($"Resetting state from {currentState} to {newState} while it had {remainingStateTime}s remaining (new time to be set is {timeInState})", owner);
            Debug.Log($"Changing state in {owner.ToString()} from {this.ToString()} to '{Enum.GetName(typeof(State), newState)}' with {timeInState} seconds programmed to be elapsed in this step.", owner);
            
            currentState = newState;
            remainingStateTime = timeInState;
            
            owner.animationsManager.PlayAnimation(GetAnimationTriggerId(newState));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"State '{Enum.GetName(typeof(State), currentState)}' with {remainingStateTime} seconds remaining";
        }

        #region Animations


        /// <summary>
        /// Returns the id of the trigger for the animation of the given state
        /// </summary>
        /// <param name="state">The state for which it is wanted to know the trigger id of its animation</param>
        /// <returns>The id of the trigger for the animation of the given state</returns>
        private int GetAnimationTriggerId(State state)
        {
            switch (state)
            {
                case State.None: return Animator.StringToHash("Idle"); // Id of the trigger for the animation 'Idle' used in the Animator  // Waiting, 
                case State.Inactive: return Animator.StringToHash("Inactive"); // Id of the trigger for the animation 'Inactive' used in the Animator // Resting
                case State.Active: return Animator.StringToHash("Active"); // Id of the trigger for the animation 'Active' used in the Animator // Doing something
            }
            return Animator.StringToHash("Idle"); ;
        }

        #endregion

    }

    /// <summary>
    /// The current condition of a MapElement.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Can be interrupted. The MapElement is moving, looking for something to do, chilling, ...
        /// </summary>
        None, 
        /// <summary>
        /// The MapElement is inactive/stopped/resting, in one place waiting comfortably, turned off, ...
        /// </summary>
        Inactive,
        /// <summary>
        /// The MapElement is doing an active/working task to (usually) obtain an object, being turn on (bonfire...).
        /// </summary>
        Active,
        
    }
}