using System;
using System.Collections.Generic;
using Animancer;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.MapEvents;
using Thoughts.Game.Map.Properties;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements
{
    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimationsManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Animator handling the animations of this MapElement.
        /// </summary>
        private AnimancerComponent animancer
        {
            get
            {
                if (_animancer == null)
                    _animancer = this.GetComponentRequired<AnimancerComponent>();
                return _animancer;
            }
        }
        private AnimancerComponent _animancer;
        [SerializeField] private MapElement owner;
        [Header("Standard animations")]
        [SerializeField] private AnimationClip animationClipIdle;
        [SerializeField] private AnimationClip animationClipWalk;
        [SerializeField] private AnimationClip animationClipResting;
        [SerializeField] private AnimationClip animationClipActive;
        [Header("Dedicated animations")]
        [SerializeField] private List<AnimationPerEvent> animationsAsEventExecuter;
        [SerializeField] private List<AnimationTriggerCondition> animationsTriggers = new List<AnimationTriggerCondition>();
        
        private void PlayAnimation(AnimationClip animationClip)
        {
            if (animationClip != null)
            {
                //Todo: loop and play with ease, not cut
                animancer.Play(animationClip);
            }
            else
            {
                animancer.Stop();
            }
        }

        public bool UpdateAnimationsByTrigger()
        {
            bool animationUpdated = false;
            foreach (AnimationTriggerCondition condition in animationsTriggers)
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
                        triggerAnimation = Math.Abs(valueInOwner - condition.quantity) < 0.001f;
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
                {
                    if (owner.stateManager.currentState.stateType != StateType.None)
                        Debug.LogWarning($"Starting to play animation clip triggered by '{condition}' while the state of the MapElement is '{owner.stateManager.currentStateName}'");
                    PlayAnimation(condition.animationClip);
                }
                animationUpdated = triggerAnimation;
            }
            return animationUpdated;
        }

        public void PlayAnimationOfEventExecuter(MapEvent mapEvent, Consequence consequence = null)
        {
            foreach (AnimationPerEvent animationPerEvent in animationsAsEventExecuter)
            {
                if (animationPerEvent.mapEvent != mapEvent)
                    continue;
                    
                PlayAnimation(animationPerEvent.animationClip);
                return;
            }

            if (consequence == null)
                foreach (Consequence mapEventConsequence in mapEvent.consequences)
                {
                    if (mapEventConsequence.affectedMapElement == AffectedMapElement.eventExecuter)
                    {
                        consequence = mapEventConsequence;
                        break;
                    }
                }
                
            PlayAnimationOfStateType(consequence.stateUpdate.stateType);
        }
        
        private void PlayAnimationOfStateType(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.None:
                    if (!owner.IsMoving())
                        PlayAnimation(animationClipIdle);
                    else
                        PlayAnimation(animationClipWalk);
                    break;
                case StateType.Resting:
                    PlayAnimation(animationClipResting);
                    break;
                case StateType.Active:
                    PlayAnimation(animationClipActive);
                    break;
                default:
                    Debug.LogWarning($"Unknown default animation for state type '{Enum.GetName(typeof(StateType), stateType)}'");
                    break;
            }
        }

        public void UpdateAnimation()
        {
            if (owner.stateManager.currentState.stateType == StateType.None)
                PlayAnimationOfStateType(owner.stateManager.currentState.stateType);
        }

        /*
        /// <summary>
        /// Returns the id of the animationClip for the animation of the given StateType
        /// </summary>
        /// <param name="StateType">The StateType for which it is wanted to know the animationClip id of its animation</param>
        /// <returns>The id of the animationClip for the animation of the given StateType</returns>
        private string GetAnimationTriggerId(StateType StateType, MapElement owner)
        {
            switch (StateType)
            {
                case StateType.None: 
                    if (owner.navMeshAgent == null || !owner.IsMoving())
                        return "Idle"; // Id of the animationClip for the animation 'Idle' used in the Animator  // Waiting
                    return "Move"; // Id of the animationClip for the animation 'Move' used in the Animator  // Walking //Todo: chang animation for 'walk'
                case StateType.Resting: return "Inactive"; // Id of the animationClip for the animation 'Inactive' used in the Animator // Resting
                case StateType.Active: return "Active"; // Id of the animationClip for the animation 'Active' used in the Animator // Doing something
            }

            Debug.LogWarning($"Unknown StateType '{(StateType)StateType}'");
            return null;
            
        }
        public void PlayStateAnimation(StateType StateType, MapElement owner)
        {
            //Debug.Log($"Playing animation for the StateType {StateType} in MapElement {owner}");
            PlayAnimation(GetAnimationTriggerId(StateType, owner));
        }
        */
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
        [SerializeField] internal  AnimationClip animationClip;
        
        internal enum TriggerOptions
        {
            lessThan,
            lessThanOrEqual,
            equal,
            moreThanOrEqual,
            moreThan
            
        }

        public override string ToString()
        {
            return $"AnimationTriggerCondition [property={property.name}, triggerOptions={Enum.GetName(typeof(TriggerOptions), triggerOptions)}, quantity={quantity}, animationClip={animationClip.name}]";
        }
        
    }

    [Serializable]
    public class AnimationPerEvent
    {
        public MapEvent mapEvent;
        public AnimationClip animationClip;
    }
}