using System;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Properties.MapEvents;
using Thoughts.Game.Map.MapEvents;
using UnityEngine;

namespace Thoughts.Game.Map.Properties
{
    /// <summary>
    /// Manages a set of PropertyOwnerships all owned by the same MapElement
    /// </summary>
    [Serializable]
    public class PropertyManager
    {
        /// <summary>
        /// MapElement owner of this PropertyManager
        /// </summary>
        public MapElement owner { get; private set; }

        /// <summary>
        /// List of PropertyOwnerships 
        /// </summary>
        public List<PropertyOwnership> propertyOwnerships
        {
            get { return _propertyOwnerships; }
            private set { _propertyOwnerships = value; }
        }
        [SerializeField] private List<PropertyOwnership> _propertyOwnerships = new List<PropertyOwnership>();

        /// <summary>
        /// Initialized the PropertyManager by setting up the owner of all PropertyOwnerships to the given MapElement.
        /// </summary>
        /// <param name="owner">The MapElement owner of this PropertyManager</param>
        public void Initialize(MapElement owner)
        {
            this.owner = owner;
            foreach (PropertyOwnership property in propertyOwnerships)
                property.UpdateOwner(this.owner);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"PropertyManager of {owner} with properties '{propertyOwnerships}'";
        }

        /// <summary>
        /// Executes the MapEvents of all the Properties that are part of the PropertyOwnership of this PropertyManager that have the field executeWithTimeElapse enabled
        /// <para>This method is periodically called by the MapElement owner of this PropertyManager.</para>
        /// </summary>
        public void ExecuteMapEventsWithTimeElapseEnabled()
        {
            if (propertyOwnerships.IsNullOrEmpty())
                return;
            
            foreach (PropertyOwnership propertyOwnership in propertyOwnerships)
            {
                foreach (MapEvent propertyMapEvent in propertyOwnership.property.mapEvents)
                {
                    if (propertyMapEvent.executeWithTimeElapse &&
                        propertyMapEvent.GetRequirementsNotMet(propertyOwnership.property, owner, owner, owner, 1).IsNullOrEmpty())
                    {
                        //Debug.Log($"        · Executing mapEvent '{propertyMapEvent}' of '{property}' in '{mapElement}'.");
                        propertyMapEvent.Execute(owner, owner, owner, propertyOwnership.property);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the value of the PropertyOwnerships with the given Property. If no PropertyOwnerships related to the given Property is found, a new PropertyOwnerships will be created in this PropertyManager with the given value.
        /// </summary>
        /// <param name="propertyToUpdate">The Property that the updated PropertyOwnership must have.</param>
        /// <param name="deltaValue">The difference that is wanted to apply to the current value of the PropertyOwnership. Can be positive and negative.</param>
        public void UpdateProperty(Property propertyToUpdate, float deltaValue)
        {
            PropertyOwnership foundPropertyOwnership = null;
            foreach (PropertyOwnership propertyOwnership in propertyOwnerships)
            {
                if (propertyOwnership.property != propertyToUpdate)
                    continue;
                
                propertyOwnership.UpdateValue(deltaValue);
                //Debug.Log($"         > The new value for the property '{managerProperty}' in '{ownerMapElement}' is = {managerProperty.value}");
                foundPropertyOwnership = propertyOwnership;

            }
            
            if (foundPropertyOwnership == null)
            {
                AddProperty(propertyToUpdate, deltaValue);
            }
            else
            {
                if (foundPropertyOwnership.value == 0)
                {
                    switch (foundPropertyOwnership.property.behaviourWhenEmpty)
                    {
                        case BehaviourWhenEmpty.Remove:
                            RemoveProperty(foundPropertyOwnership);
                            break;
                        case BehaviourWhenEmpty.TakeCare: // The care should be taken somewhere else
                            break;
                        case BehaviourWhenEmpty.DoNothing:
                            break;
                        case BehaviourWhenEmpty.Transform:
                            foreach (PropertyOwnership inheritanceProperty in foundPropertyOwnership.property.inheritanceProperties)
                                AddProperty(inheritanceProperty.property, inheritanceProperty.value);
                            RemoveProperty(foundPropertyOwnership);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns a list of all the PropertyOwnership in this PropertyManager that need care. All the returned PropertyOwnership have a value of 0 or less and, for all of them, it has been indicated that it is intended to have the value always higher than 0.
        /// </summary>
        /// <returns>A list of all the PropertyOwnership in this PropertyManager that need care.</returns>
        public List<PropertyOwnership> GetPropertiesThatNeedCare()
        {
            List<PropertyOwnership> propertiesThatNeedCare = new List<PropertyOwnership>();
            foreach (PropertyOwnership property in propertyOwnerships)
            {
                if (property.NeedsCare())
                    propertiesThatNeedCare.Add(property);
            }
            return propertiesThatNeedCare;
        }

        /// <summary>
        /// Indicates if this PropertyManager can cover the Requirement a given amount of times. 
        /// </summary>
        /// <param name="containerProperty">The Property containing the given requirement to check</param>
        /// <param name="requirement">The Requirement to check if can be covered.</param>
        /// <param name="times">The amount of times the requirement will have to be met.</param>
        /// <param name="remainingValueToCover">The remaining value of the Requirement that can not be currently covered by this PropertyManager.</param>
        /// <returns>True if it contains an property with a value higher or equal than the one in the requirement/PropertyUpdate n times</returns>
        public bool CanCover(Property containerProperty, Requirement requirement, int times, out float remainingValueToCover)
        {
            remainingValueToCover = requirement.minValue * times;

            if (times <= 0)
                Debug.LogWarning($"   - Attention: Checking if the PropertyManager of '{owner}' can cover the requirement '{requirement.ToString()}' {times} times!.\n");
            //else Debug.Log($"   - Checking if the PropertyManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Amount of value to gain: {missingValueToCoverInThisPropertyManager}\n");


            foreach (PropertyOwnership ownedProperty in propertyOwnerships)
            {
                if (requirement.GetProperty(containerProperty) != ownedProperty.property)
                    continue;

                remainingValueToCover -= ownedProperty.value;

                if (remainingValueToCover <= 0) // No value is missing, so the requirement can be covered
                {
                    // Debug.Log($"   - The PropertyManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisPropertyManager}\n");
                    return true;
                }
            }

            // Debug.Log($"   - The PropertyManager of '{ownerMapElement}' can NOT cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisPropertyManager}\n");
            return false;
        }

        /// <summary>
        /// Returns an PropertyOwnership of the given Property. If no PropertyOwnerships related to the given Property is found, a new PropertyOwnerships will be created in this PropertyManager with a value of 0.
        /// </summary>
        /// <param name="property">The Property of the PropertyOwnership.</param>
        /// <returns>Returns an PropertyOwnership of the given Property.</returns>
        public PropertyOwnership GetOwnedPropertyAndAddItIfNotFound(Property property)
        {
            if (property == null)
                Debug.LogError($"Trying to find a null or empty property in {owner}.");
            
            foreach (PropertyOwnership ownedProperty in propertyOwnerships)
            {
                if (ownedProperty.property == property)
                    return ownedProperty;
            }
            Debug.Log($"   Property '{property.ToString()}' not found in '{owner}' owned properties. Creating a temporal property ownership (that is not be part of the actual owned properties by the MapElement {owner}). It will be used to calculate the execution path.\n", owner);
            return new PropertyOwnership(property, 0, owner);
        }
        
        private PropertyOwnership AddProperty(Property property, float value = 0)
        {
            PropertyOwnership newPropertyOwnership = new PropertyOwnership(property, value, owner);
            propertyOwnerships.Add(newPropertyOwnership);
            return newPropertyOwnership;
        }
        
        private bool RemoveProperty(Property property)
        {
            foreach (PropertyOwnership propertyOwnership in propertyOwnerships)
            {
                if (propertyOwnership.property != property)
                    continue;
                
                return RemoveProperty(propertyOwnership);
            }
            Debug.LogWarning($"Trying to remove a property ({property.ToString()}) from '{owner}' that doesn't exist.");
            return false;
        }
        
        private bool RemoveProperty(PropertyOwnership propertyOwnership)
        {
            bool found = propertyOwnerships.Remove(propertyOwnership);
            if (found)
            {
                Debug.Log($"Removing property ownership ({propertyOwnership.ToString()}) from {owner}");
                return true;
            }
            Debug.LogWarning($"Trying to remove a property ({propertyOwnership.ToString()}) from '{owner}' that doesn't exist.");
            return false;
        }

        /// <summary>
        /// Looks for an ExecutionPlan to cover the Property in the given PropertyOwnership relation using the Properties this PropertyManager.
        /// </summary>
        /// <param name="propertyOwnershipToCover">The PropertyOwnership to which it is wanted to increase the value.</param>
        /// <param name="remainingValueToCover">The amount of value that is desired to increase with the obtained ExecutionPlan</param>
        /// <param name="executer">The MapElement that will execute the ExecutionPlan</param>
        /// <returns></returns>
        public ExecutionPlan GetExecutionPlanToCover(PropertyOwnership propertyOwnershipToCover, float remainingValueToCover, MapElement executer)
        {
            MapElement target = propertyOwnershipToCover.owner;

            // Debug.Log($" >>> Searching for an execution plan to cover '{remainingValueToCover}' of '{propertyOwnershipToCover.property}' owned by '{propertyOwnershipToCover.owner}' executed by '{executer}'.\n");

            foreach (PropertyOwnership ownedProperty in propertyOwnerships)
            {
                foreach (MapEvent mapEvent in ownedProperty.property.mapEvents)
                {
                    // Debug.Log($" >>> In '{ownedProperty.owner}', checking if mapEvent '{mapEvent}' in property '{ownedProperty.property}' can cover {remainingValueToCover} missing of the property '{propertyOwnershipToCover.property}'.\nTarget: {target}, Executer: {executer}, EventOwner still unknown.\n");
                    if (!mapEvent.executerMustOwnProperty || (mapEvent.executerMustOwnProperty && ownedProperty.owner == executer))
                    {
                        MapElement eventOwner = ownedProperty.owner;
                        ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, eventOwner, ownedProperty.property);
                        executionPlan.SetExecutionTimesToCover(propertyOwnershipToCover, remainingValueToCover);
                        int executionsToCover = executionPlan.executionTimes;
                        if (executionsToCover < 0) // The executionPlan can not cover the property
                            continue;

                        Dictionary<PropertyOwnership, float> mapEventRequirementsNotMet = mapEvent.GetRequirementsNotMet(ownedProperty.property, executer, target, owner, executionPlan.executionTimes);

                        if (mapEvent.tryToCoverRequirementsIfNotMet || (!mapEvent.tryToCoverRequirementsIfNotMet && mapEventRequirementsNotMet.IsNullOrEmpty()))
                        {
                            // If reached here, the mapEvent can be executed - Now choose if it is the appropriate one
                            // Debug.Log($"   > The mapEvent '{mapEvent}' can be executed ({mapEventRequirementsNotMet.Count} requirements must be covered before):\n    - {mapEventRequirementsNotMet.ToStringAllElements("\n    - ")}\n");

                            if (mapEvent.ConsequencesCover(propertyOwnershipToCover, target, executer, eventOwner, ownedProperty.property))
                            {
                                Debug.Log($" ● Found Execution Plan: {executionPlan}\n");
                                return executionPlan;
                            }

                        }
                    }
                    else
                    {
                        // Debug.Log($"    The executer ({executer}) must own the property '{currentOwnedProperty.property}' to execute '{mapEvent}' but it does not. MapEvent owned by '{currentOwnedProperty.ownerMapElement}'.\n");
                        if (mapEvent.ConsequencesCover(propertyOwnershipToCover, target, executer, executer, ownedProperty.property))
                        {
                            ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, executer, ownedProperty.property);
                            executionPlan.SetExecutionTimesToCover(propertyOwnershipToCover, remainingValueToCover);
                            Debug.Log($" ● Found 'forced' Execution Plan: {executionPlan}\n");
                            return executionPlan;
                        }
                    }

                }
            }
            
            return null;
        }
        
        
        /// <summary>
        /// Looks for all the Properties that have the same level of priority as the given one
        /// </summary>
        /// <param name="priority">The priority that all the returned Properties must have</param>
        /// <returns>A list of PropertyOwnership where all Properties have the given level of priority</returns>
        public List<PropertyOwnership> GetPropertiesWithPriority(Property.NeedPriority priority)
        {
            List<PropertyOwnership> matchingPropertyOwnerships = new List<PropertyOwnership>();
            foreach (PropertyOwnership propertyOwnership in this.propertyOwnerships)
            {
                if (propertyOwnership.property.needPriority == priority)
                    matchingPropertyOwnerships.Add(propertyOwnership);
            }
            return matchingPropertyOwnerships;
        }
        
        public float GetValueOf(Property property)
        {
            PropertyOwnership foundProperty = GetOwnedPropertyAndAddItIfNotFound(property);
            return foundProperty.value;
        }
        public List<Property> GetListOfAllCurrentProperties()
        {
            List<Property> returnList = new List<Property>();
            foreach (PropertyOwnership propertyOwnership in propertyOwnerships)
            {
                returnList.Add(propertyOwnership.property);
            }
            return returnList;
        }
    }
}