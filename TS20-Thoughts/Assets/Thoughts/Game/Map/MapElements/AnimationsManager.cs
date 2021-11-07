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


        public void PlayAnimation(string trigger)
        {
            PlayAnimation(Animator.StringToHash(trigger));
        
        }
        public void PlayAnimation(int triggerHash)
        {
            if (animator.runtimeAnimatorController != null)
                animator.SetTrigger(triggerHash);
            //else Debug.LogWarning($"The MapElement {this.transform.parent.name} doesn't have an AnimatorController set in the animator.", animator);
        }

        internal void UpdateAnimationsUpdates(MapElement owner)
        {
            foreach (AnimationTriggerCondition condition in animationTriggerConditions)
            {
                bool triggerAnimation = false;
                int valueInOwner = owner.propertyManager.GetValueOf(condition.property);
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