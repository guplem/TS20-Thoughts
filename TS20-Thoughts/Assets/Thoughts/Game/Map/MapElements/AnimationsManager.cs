using System;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements.Properties;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements
{
    [RequireComponent(typeof(Animator))]
    public class AnimationsManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Animator handling the animations of this MapElement.
        /// </summary>
        private Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = this.GetComponentRequired<Animator>();
                return _animator;
            }
        }
        private Animator _animator;
        [SerializeField] private List<AnimationTriggerCondition> animationTriggerConditions = new List<AnimationTriggerCondition>();


        private void PlayAnimation(string trigger)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(trigger);
        }
        
        /*private void PlayAnimation(int triggerHash)
        {
            if (animator.runtimeAnimatorController != null)
                if (animator.runtimeAnimatorController != null)
                {
                    animator.SetTrigger(triggerHash);
                }
                //else Debug.LogWarning($"The MapElement {this.transform.parent.name} doesn't have an AnimatorController set in the animator.", animator);
        }*/

        internal void UpdateAnimationsUpdates(MapElement owner)
        {
            foreach (AnimationTriggerCondition condition in animationTriggerConditions)
            {
                bool triggerAnimation = false;
                float valueInOwner = owner.propertyManager.GetValueOf(condition.property);
                switch (condition.triggerOptions)
                {

                    case AnimationTriggerCondition.TriggerOptions.lessThan:
                        triggerAnimation = valueInOwner < condition.quantity;
                        break;
                    case AnimationTriggerCondition.TriggerOptions.lessThanOrEqual:
                        triggerAnimation = valueInOwner <= condition.quantity;
                        break;
                    case AnimationTriggerCondition.TriggerOptions.equal:
                        triggerAnimation = valueInOwner == condition.quantity;
                        break;
                    case AnimationTriggerCondition.TriggerOptions.moreThanOrEqual:
                        triggerAnimation = valueInOwner >= condition.quantity;
                        break;
                    case AnimationTriggerCondition.TriggerOptions.moreThan:
                        triggerAnimation = valueInOwner > condition.quantity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (triggerAnimation)
                    PlayAnimation(condition.animationTrigger);
            }
        }
        
        /// <summary>
        /// Returns the id of the trigger for the animation of the given state
        /// </summary>
        /// <param name="state">The state for which it is wanted to know the trigger id of its animation</param>
        /// <returns>The id of the trigger for the animation of the given state</returns>
        private string GetAnimationTriggerId(State state, MapElement owner)
        {
            switch (state)
            {
                case State.None: 
                    if (owner.navMeshAgent == null || owner.navMeshAgent.velocity.magnitude < 0.15f)
                        return "Idle"; // Id of the trigger for the animation 'Idle' used in the Animator  // Waiting
                    return "Move"; // Id of the trigger for the animation 'Move' used in the Animator  // Walking //Todo: chang animation for 'walk'
                case State.Inactive: return "Inactive"; // Id of the trigger for the animation 'Inactive' used in the Animator // Resting
                case State.Active: return "Active"; // Id of the trigger for the animation 'Active' used in the Animator // Doing something
            }

            Debug.LogWarning($"Unknown state '{(State)state}'");
            return null;
        }
        public void PlayStateAnimation(State state, MapElement owner)
        {
            //Debug.Log($"Playing animation for the state {state} in MapElement {owner}");
            PlayAnimation(GetAnimationTriggerId(state, owner));
        }
    }

    
    /// <summary>
    /// Which animation should be triggered depending on the condition of the given properties
    /// </summary>
    [Serializable]
    public class AnimationTriggerCondition
    {
        [SerializeField] internal Property property;
        [SerializeField] internal  TriggerOptions triggerOptions;
        [SerializeField] internal  int quantity;
        [SerializeField] internal  string animationTrigger;
        
        internal enum TriggerOptions
        {
            lessThan,
            lessThanOrEqual,
            equal,
            moreThanOrEqual,
            moreThan
            
        }
    }
}