using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.Properties.MapEvents
{

    /// <summary>
    /// An event that must be executed by a MapElement. It is part of an property.
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
        /// Determines if the executer of the event must own the property where this event lives in.
        /// </summary>
        [Tooltip("Determines if the executer of the event must own the property where this event lives in.")]
        [SerializeField] public bool executerMustOwnProperty = false;
        
        /// <summary>
        /// List of update to properties that will be triggered as a consequence of the execution of the MapEvent.
        /// </summary>
        [Tooltip("List of update to properties that will be triggered as a consequence of the execution of the MapEvent.")]
        [SerializeField] public List<Consequence> consequences = new List<Consequence>();
        
        /// <summary>
        /// Determines whether a plan should be made to cover requirements that are not met at the time of attempting to execute the event.
        /// <para>If false, in case the requirements are not met and the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed).</para>
        /// </summary>
        [Tooltip("If false, in case the requirements are not met and the event can not be executed, this event is going to be ignored (so no map element is going to 'try' to fix the requirements so it can be executed).")]
        [SerializeField] public bool tryToCoverRequirementsIfNotMet = true;
        
        /// <summary>
        /// List of properties with specific values that must be met in order to execute the event..
        /// </summary>
        [Tooltip("List of properties with specific values that must be met in order to execute the event.")]
        [SerializeField] public List<Requirement> requirements = new List<Requirement>();

        /// <summary>
        /// Executes the event applying the consequences of it.
        /// </summary>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        public void Execute(MapElement executer, MapElement target, MapElement owner)
        {
            if (!executeWithTimeElapse)
                Debug.Log($"        · MapElement '{executer}' is executing '{name}' using a property in '{owner}' with target '{target}'.\n         \\_Their properties are: {owner.propertyManager.ToString()}");

            foreach (Consequence consequence in consequences)
            {
                switch (consequence.affectedMapElement)
                {
                    case AffectedMapElement.eventOwner:
                        owner.propertyManager.UpdateProperty(consequence.property, consequence.deltaValue);
                        if (consequence.setNewState)
                            owner.stateManager.SetState(consequence.stateUpdate.newState, consequence.stateUpdate.newStateDuration);
                        break;
                    
                    case AffectedMapElement.eventExecuter:
                        executer.propertyManager.UpdateProperty(consequence.property, consequence.deltaValue);
                        if (consequence.setNewState)
                            executer.stateManager.SetState(consequence.stateUpdate.newState, consequence.stateUpdate.newStateDuration);
                        break;
                    
                    case AffectedMapElement.eventTarget:
                        target.propertyManager.UpdateProperty(consequence.property, consequence.deltaValue);
                        if (consequence.setNewState)
                            target.stateManager.SetState(consequence.stateUpdate.newState, consequence.stateUpdate.newStateDuration);
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
        
        /// <summary>
        /// Returns a list of the requirements that are not met at the moment to execute the event, it and outputs a list of the value missing for each one of the requirements that are not met (in the same order).
        /// </summary>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        /// <param name="executionTimes">The amount of times that is desired to execute the event.</param>
        /// <returns>A list of the requirements that are not met at the moment to execute the event (the keys), each one of them related to the value missing (value to cover)</returns>
        public Dictionary<PropertyOwnership, float> GetRequirementsNotMet(MapElement executer, MapElement target, MapElement owner, int executionTimes)
        {
            Dictionary<PropertyOwnership, float> requirementsNotMet = new Dictionary<PropertyOwnership, float>();
            
            foreach (Requirement requirement in requirements)
            {
                if (requirement.property == null)
                    Debug.LogWarning($"Found a requirement without a property linked to it. Requirement: {requirement.ToString()}");
                //OwnedProperty propertyThatMostCloselyMeetsTheRequirement;
                float remainingValueToCoverRequirementNotMet;
                bool meets = true;

                switch (requirement.affectedMapElement)
                {
                    case AffectedMapElement.eventOwner:
                        meets = owner.propertyManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            PropertyOwnership req = (owner.propertyManager.GetOwnedPropertyAndAddItIfNotFound(requirement.property));
                            requirementsNotMet.Add(req, remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AffectedMapElement.eventExecuter:
                        meets = executer.propertyManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            PropertyOwnership req = (executer.propertyManager.GetOwnedPropertyAndAddItIfNotFound(requirement.property));
                            requirementsNotMet.Add(req, remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    case AffectedMapElement.eventTarget:
                        meets = target.propertyManager.CanCover(requirement, executionTimes, out remainingValueToCoverRequirementNotMet);
                        if (!meets) {
                            PropertyOwnership req = (target.propertyManager.GetOwnedPropertyAndAddItIfNotFound(requirement.property));
                            requirementsNotMet.Add(req, remainingValueToCoverRequirementNotMet);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            
            return requirementsNotMet;
        }

        /// <summary>
        /// Indicates if the consequences of the execution of this event will increase the value of an property owned by a map element.
        /// </summary>
        /// <param name="propertyOwnershipToCover">PropertyOwnership to cover.</param>
        /// <param name="executer">The MapElement that is going to execute/trigger the event.</param>
        /// <param name="target">The MapElement target of the execution of the event.</param>
        /// <param name="owner">The MapElement that owns the event.</param>
        /// <returns>True, if the execution of this MapEvent with this target, executer and owner would increase the value of the given property. False, otherwise.</returns>
        public bool ConsequencesCover(PropertyOwnership propertyOwnershipToCover, MapElement target, MapElement executer, MapElement owner)
        {
            bool consequenceCoversOwnerOfProperty = false;
            // Debug.Log($"$$$$$ Checking if consequences of '{name}' cover '{propertyOwnershipToCover.property}'. target = {target}, executer = {executer}, owner = {owner}\n");
            foreach (Consequence consequence in consequences)
            {
                // Debug.Log($"    $$$$$ Current consequence's property = '{consequence.property}'. Delta value = {consequence.deltaValue}. Covers desired property? {(consequence.property == propertyOwnershipToCover.property && consequence.deltaValue > 0)}\n");
                if (consequence.property == propertyOwnershipToCover.property && consequence.deltaValue > 0)
                {
                    switch (consequence.affectedMapElement)
                    {
                        case AffectedMapElement.eventOwner:
                            consequenceCoversOwnerOfProperty = propertyOwnershipToCover.owner == owner;
                            break;
                        case AffectedMapElement.eventExecuter:
                            consequenceCoversOwnerOfProperty = propertyOwnershipToCover.owner == executer;
                            break;
                        case AffectedMapElement.eventTarget:
                            consequenceCoversOwnerOfProperty = propertyOwnershipToCover.owner == target;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (consequenceCoversOwnerOfProperty)
                    {
                        if (consequence.affectedMapElement == AffectedMapElement.eventTarget)
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
    
    /// <summary>
    /// Lists the possible MapElements related to the MapEvent that can be the affected ones to the event execution, requirements, consequences, ...
    /// </summary>
    public enum AffectedMapElement
    {
        /// <summary>
        /// The owner of the event
        /// </summary>
        eventOwner,
        /// <summary>
        /// The executer of the event
        /// </summary>
        eventExecuter,
        /// <summary>
        /// The target of the event
        /// </summary>
        eventTarget
    }
    
}