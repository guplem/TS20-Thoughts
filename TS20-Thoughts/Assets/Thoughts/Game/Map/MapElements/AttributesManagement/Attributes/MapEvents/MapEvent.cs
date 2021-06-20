using System;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using UnityEngine;

namespace Thoughts.Game.GameMap
{

    /// <summary>
    /// An event that must be executed by a MapElement. It is part of an attribute.
    /// </summary>
    [Serializable]
    public class MapEvent
    {
        /// <summary>
        /// The name of the event
        /// </summary>
        [Tooltip("The name of the event")]
        [SerializeField] public string name = "";
        
        /// <summary>
        /// The maximum distance allowed to execute the event. The distance is checked between the executer and the target, and between the executer and the event owner. Ignored if it is less than 0 or if executeWithTimeElapse is true.
        /// </summary>
        [Tooltip("The maximum distance allowed to execute the event. The distance is checked between the executer and the target, and between the executer and the event owner. Ignored if it is <0 or if executeWithTimeElapse is true.")]
        [SerializeField] public float maxDistance = 5f;
        
        /// <summary>
        /// Determines if the event should be executed every time the time elapses in the map.
        /// </summary>
        [Tooltip("Determines if the event should be executed every time the time elapses in the map.")]
        [SerializeField] public bool executeWithTimeElapse = false;
        
        /// <summary>
        /// Determines if the executer of the event must own the attribute where this event lives in.
        /// </summary>
        [Tooltip("Determines if the executer of the event must own the attribute where this event lives in.")]
        [SerializeField] public bool executerMustOwnAttribute = false;
        
        /// <summary>
        /// List of update to attributes that will be triggered as a consequence of the execution of the MapEvent.
        /// </summary>
        [Tooltip("List of update to attributes that will be triggered as a consequence of the execution of the MapEvent.")]
        [SerializeField] public List<AttributeUpdate> consequences = new List<AttributeUpdate>();
        
        /// <summary>
        /// Determines whether a plan should be made to cover requirements that are not met at the time of attempting to execute the event.
        /// <para>If false, in case the requirements are not met and the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed).</para>
        /// </summary>
        [Tooltip("If false, in case the requirements are not met and the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed).")]
        [SerializeField] public bool tryToCoverRequirementsIfNotMet = true;
        
        /// <summary>
        /// List of attributes with specific values that must be met in order to execute the event..
        /// </summary>
        [Tooltip("List of attributes with specific values that must be met in order to execute the event.")]
        [SerializeField] public List<AttributeUpdate> requirements = new List<AttributeUpdate>();

        /// <summary>
        /// Executes the event applying the consequences of it.
        /// </summary>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        public void Execute(MapElement executer, MapElement target, MapElement owner)
        {
            // Debug.Log($"        · MapElement '{executer}' is executing '{name}' of '{ownerAttribute}' with target '{target}'.");

            foreach (AttributeUpdate attributeUpdate in consequences)
            {
                switch (attributeUpdate.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        owner.attributeManager.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        executer.attributeManager.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        target.attributeManager.UpdateAttribute(attributeUpdate.attribute, attributeUpdate.value);
                    break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public override string ToString()
        {
            return  (name.IsNullEmptyOrWhiteSpace() ? (this.GetType().Name + " (no name)") : name) ;
        }
        
        /// <summary>
        /// Indicates if the maximum distance allowed to execute the event is met or not.
        /// <para>The distance is checked between the executer and the target, and between the executer and the event owner.</para>
        /// <para>The distance will be always considered as met if the field maxDistance of the event is less than 0 or if the field executeWithTimeElapse of the event is true.</para>
        /// </summary>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        /// <returns></returns>
        public bool IsDistanceMet(MapElement executer, MapElement target, MapElement owner)
        {
            if (maxDistance < 0)
                return true;
        
            Vector3 eventOwnerPosition = owner.transform.position;
            
            Vector3 executerPosition = executer.transform.position;
            float distanceOwnerExecuter = Vector3.Distance(eventOwnerPosition, executerPosition);
            
            Vector3 targetPosition = target.transform.position;
            float distanceTargetExecuter = Vector3.Distance(eventOwnerPosition, targetPosition);

            float currentMaxDistance = Mathf.Max(distanceTargetExecuter, distanceOwnerExecuter);
            //Debug.Log($"CURRENT MAX DISTANCE = {currentMaxDistance}");
            
            return currentMaxDistance <= maxDistance;
        }
        
        //ToDo: refactor so instead of returning 2 lists, returns a dictionary
        /// <summary>
        /// Returns a list of the requirements that are not met at the moment to execute the event, it and outputs a list of the value missing for each one of the requirements that are not met (in the same order).
        /// </summary>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        /// <param name="executionTimes">The amount of times that is desired to execute the event.</param>
        /// <param name="remainingValueToCoverInRequirementsNotMet">A list of the value missing for each one of the requirements that are not met (in the same order than the returned list of requirements not met)</param>
        /// <returns>A list of the requirements that are not met at the moment to execute the event.</returns>
        public List<OwnedAttribute> GetRequirementsNotMet(MapElement executer, MapElement target, MapElement owner, int executionTimes, out List<int> remainingValueToCoverInRequirementsNotMet)
        {
            List<OwnedAttribute> requirementsNotMet = new List<OwnedAttribute>();
            remainingValueToCoverInRequirementsNotMet = new List<int>();
            
            foreach (AttributeUpdate requirement in requirements)
            {
                //OwnedAttribute attributeThatMostCloselyMeetsTheRequirement;
                int remainingValueToCoverRequirementNotMet;
                bool meets = true;

                switch (requirement.affected)
                {
                    case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                        meets = owner.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(owner.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverInRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                        meets = executer.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(executer.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverInRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                        meets = target.attributeManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            requirementsNotMet.Add(target.attributeManager.GetOwnedAttributeOf(requirement.attribute));
                            remainingValueToCoverInRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //if (!meets)
                //{
                //    //remainingValueToCoverRequirementsNotMet.Add(remainingValueToCoverRequirementNotMet);
                //    //requirementsNotMet.Add(requirement);
                //    //if (attributeThatMostCloselyMeetsTheRequirement == null)
                //    //    Debug.LogWarning("Adding a attributeThatMostCloselyMeetsTheRequirement that is null"); // Only ok if there is no 
                //}

                //if (requirementsNotMet.Count != remainingValueToCoverRequirementsNotMet.Count)
                //{
                //    Debug.LogWarning("The two lists (requirementsNotMet and remainingValueToCoverInRequirementsNotMet) should be kept syncronized and they have different sizes.");
                //    requirementsNotMet.DebugLogWarning();
                //    remainingValueToCoverRequirementsNotMet.DebugLogWarning();
                //}
                
            }
            
            return requirementsNotMet;
        }

        /// <summary>
        /// Indicates if the consequences of the execution of this event will increase the value of an attribute owned by a map element.
        /// </summary>
        /// <param name="attributeToCover">The desired attribute owned by a MapElement to cover.</param>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        /// <returns>True, if the execution of this MapEvent with this target, executer and owner would increase the value of the given attribute. False, otherwise.</returns>
        public bool ConsequencesCover(OwnedAttribute attributeToCover, MapElement target, MapElement executer, MapElement owner)
        {
            bool consequenceCoversOwnerOfAttribute = false;
            // Debug.Log($"$$$$$ Checking if consequences of '{name}' cover '{ownedAttribute.attribute}'.\n");
            foreach (AttributeUpdate consequence in consequences)
            {
                //Debug.Log($"    $$$$$ Current consequence's attribute = '{consequence.attribute}'.\n");
                if (consequence.attribute == attributeToCover.attribute && consequence.value > 0)
                {
                    switch (consequence.affected)
                    {
                        case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                            consequenceCoversOwnerOfAttribute = attributeToCover.ownerMapElement == owner;
                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                            consequenceCoversOwnerOfAttribute = attributeToCover.ownerMapElement == executer;
                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                            consequenceCoversOwnerOfAttribute = attributeToCover.ownerMapElement == target;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (consequenceCoversOwnerOfAttribute)
                    {
                        if (consequence.affected == AttributeUpdate.AttributeUpdateAffected.eventTarget)
                        {
                            // The 'target' must not be the 'executer' neither the 'owner'
                            if (target == executer || target == owner)
                                return false;
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }
            
            return false;
        }
    }
}