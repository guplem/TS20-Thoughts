using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Attributes
{
    /// <summary>
    /// Manages a set of AttributeOwnerships all owned by the same MapElement
    /// </summary>
    [Serializable]
    public class AttributeManager
    {
        /// <summary>
        /// MapElement owner of this AttributeManager
        /// </summary>
        public MapElement owner { get; private set; }

        /// <summary>
        /// List of AttributeOwnerships 
        /// </summary>
        public List<AttributeOwnership> attributeOwnerships
        {
            get { return _attributeOwnerships; }
            private set { _attributeOwnerships = value; }
        }
        [FormerlySerializedAs("_ownedAttributes")]
        [SerializeField] private List<AttributeOwnership> _attributeOwnerships = new List<AttributeOwnership>();

        /// <summary>
        /// Initialized the AttributeManager by setting up the owner of all AttributeOwnerships to the given MapElement.
        /// </summary>
        /// <param name="owner">The MapElement owner of this AttributeManager</param>
        public void Initialize(MapElement owner)
        {
            this.owner = owner;
            foreach (AttributeOwnership attribute in attributeOwnerships)
                attribute.UpdateOwner(this.owner);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"AttributeManager of {owner} with attributes '{attributeOwnerships}'";
        }

        /// <summary>
        /// Executes the MapEvents of all the Attributes that are part of the AttributeOwnership of this AttributeManager that have the field executeWithTimeElapse enabled
        /// <para>This method is periodically called by the MapElement owner of this AttributeManager.</para>
        /// </summary>
        public void ExecuteMapEventsWithTimeElapseEnabled()
        {
            if (attributeOwnerships.IsNullOrEmpty())
                return;
            
            foreach (AttributeOwnership attribute in attributeOwnerships)
            {
                foreach (MapEvent attributeMapEvent in attribute.attribute.mapEvents)
                {
                    if (attributeMapEvent.executeWithTimeElapse &&
                        attributeMapEvent.GetRequirementsNotMet(owner, owner, owner, 1, out List<int> temp).IsNullOrEmpty())
                    {
                        //Debug.Log($"        · Executing mapEvent '{attributeMapEvent}' of '{attribute}' in '{mapElement}'.");
                        attributeMapEvent.Execute(owner, owner, owner);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the value of the AttributeOwnerships with the given Attribute. If no AttributeOwnerships related to the given Attribute is found, a new AttributeOwnerships will be created in this AttributeManager with the given value.
        /// </summary>
        /// <param name="attributeToUpdate">The Attribute that the updated AttributeOwnership must have.</param>
        /// <param name="deltaValue">The difference that is wanted to apply to the current value of the AttributeOwnership. Can be positive and negative.</param>
        public void UpdateAttribute(Attributes.Attribute attributeToUpdate, int deltaValue)
        {
            bool found = false;
            foreach (AttributeOwnership managerAttribute in attributeOwnerships)
            {
                if (managerAttribute.attribute == attributeToUpdate)
                {
                    managerAttribute.UpdateValue(deltaValue);
                    //Debug.Log($"         > The new value for the attribute '{managerAttribute}' in '{ownerMapElement}' is = {managerAttribute.value}");
                    found = true;
                }
            }
            if (!found)
            {
                attributeOwnerships.Add(new AttributeOwnership(attributeToUpdate, deltaValue, owner, false));
            }
        }
        
        /// <summary>
        /// Returns a list of all the AttributeOwnership in this AttributeManager that need care. All the returned AttributeOwnership have a value of 0 or less and, for all of them, it has been indicated that it is intended to have the value always higher than 0.
        /// </summary>
        /// <returns>A list of all the AttributeOwnership in this AttributeManager that need care.</returns>
        public List<AttributeOwnership> GetAttributesThatNeedCare()
        {
            List<AttributeOwnership> attributesThatNeedCare = new List<AttributeOwnership>();
            foreach (AttributeOwnership attribute in attributeOwnerships)
            {
                if (attribute.NeedsCare())
                    attributesThatNeedCare.Add(attribute);
            }
            return attributesThatNeedCare;
        }

        /// <summary>
        /// Indicates if this AttributeManager can cover the Requirement a given amount of times. 
        /// </summary>
        /// <param name="requirement">The Requirement to check if can be covered.</param>
        /// <param name="times">The amount of times the requirement will have to be met.</param>
        /// <param name="remainingValueToCover">The remaining value of the Requirement that can not be currently covered by this AttributeManager.</param>
        /// <returns>True if it contains an attribute with a value higher or equal than the one in the requirement/AttributeUpdate n times</returns>
        public bool CanCover(Requirement requirement, int times, out int remainingValueToCover)
        {
            remainingValueToCover = requirement.minValue * times;

            if (times <= 0)
                Debug.LogWarning($"   - Attention: Checking if the AttributeManager of '{owner}' can cover the requirement '{requirement.ToString()}' {times} times!.\n");
            //else Debug.Log($"   - Checking if the AttributeManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");


            foreach (AttributeOwnership ownedAttribute in attributeOwnerships)
            {
                if (requirement.attribute != ownedAttribute.attribute)
                    continue;

                remainingValueToCover -= ownedAttribute.value;

                if (remainingValueToCover <= 0) // No value is missing, so the requirement can be covered
                {
                    // Debug.Log($"   - The AttributeManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");
                    return true;
                }
            }

            // Debug.Log($"   - The AttributeManager of '{ownerMapElement}' can NOT cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");
            return false;
        }

        /// <summary>
        /// Returns an AttributeOwnership of the given Attribute. If no AttributeOwnerships related to the given Attribute is found, a new AttributeOwnerships will be created in this AttributeManager with a value of 0.
        /// </summary>
        /// <param name="attribute">The Attribute of the AttributeOwnership.</param>
        /// <returns>Returns an AttributeOwnership of the given Attribute.</returns>
        public AttributeOwnership GetOwnedAttributeOf(Attributes.Attribute attribute)
        {
            foreach (AttributeOwnership ownedAttribute in attributeOwnerships)
            {
                if (ownedAttribute.attribute == attribute)
                    return ownedAttribute;
            }
            //ToDo: adding the attribute (next lines) should be done in another method. Maybe calling a new method calling 'GetOwnedAttributeAndAddItIfNotFound' should  be created to call them both
            Debug.Log($"   Attribute '{attribute}' not found in '{owner}' owned attributes. Adding the attribute with a value of 0.\n", owner);
            AttributeOwnership newAttributeOwnership = new AttributeOwnership(attribute, 0, owner, false);
            attributeOwnerships.Add(newAttributeOwnership);
            return newAttributeOwnership;
        }
        
        /// <summary>
        /// Looks for an ExecutionPlan to cover the Attribute in the given AttributeOwnership relation using the Attributes this AttributeManager.
        /// </summary>
        /// <param name="attributeOwnershipToCover">The AttributeOwnership to which it is wanted to increase the value.</param>
        /// <param name="remainingValueToCover">The amount of value that is desired to increase with the obtained ExecutionPlan</param>
        /// <param name="executer">The MapElement that will execute the ExecutionPlan</param>
        /// <returns></returns>
        public ExecutionPlan GetExecutionPlanToCover(AttributeOwnership attributeOwnershipToCover, int remainingValueToCover, MapElement executer)
        {
            MapElement target = attributeOwnershipToCover.owner;

            // Debug.Log($" >>> Searching for an execution plan to cover '{remainingValueToCover}' of '{attributeToCover.attribute}' owned by '{attributeToCover.ownerMapElement}' executed by '{executer}'.\n");

            foreach (AttributeOwnership ownedAttribute in attributeOwnerships)
            {
                foreach (MapEvent mapEvent in ownedAttribute.attribute.mapEvents)
                {
                    // Debug.Log($" >>> In '{ownerMapElement}', checking if mapEvent '{mapEvent}' in attribute '{currentOwnedAttribute.attribute}' can cover {remainingValueToCover} missing of the attribute '{ownedAttributeToCover.attribute}'.\nTarget: {target}, Executer: {executer}, EventOwner still unknown.\n");
                    if (!mapEvent.executerMustOwnAttribute || (mapEvent.executerMustOwnAttribute && ownedAttribute.owner == executer))
                    {
                        MapElement eventOwner = ownedAttribute.owner;
                        ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, eventOwner);
                        executionPlan.SetExecutionTimesToCover(attributeOwnershipToCover, remainingValueToCover);
                        int executionsToCover = executionPlan.executionTimes;
                        if (executionsToCover < 0) // The executionPlan can not cover the attribute
                            continue;

                        List<AttributeOwnership> mapEventRequirementsNotMet = mapEvent.GetRequirementsNotMet(executer, target, owner, executionPlan.executionTimes, out List<int> temp);

                        if (mapEvent.tryToCoverRequirementsIfNotMet || (!mapEvent.tryToCoverRequirementsIfNotMet && mapEventRequirementsNotMet.IsNullOrEmpty()))
                        {
                            // If reached here, the mapEvent can be executed - Now choose if it is the appropriate one
                            Debug.Log($"   > The mapEvent '{mapEvent}' can be executed ({mapEventRequirementsNotMet.Count} requirements must be covered before):\n    - {mapEventRequirementsNotMet.ToStringAllElements("\n    - ")}\n");

                            if (mapEvent.ConsequencesCover(attributeOwnershipToCover, target, executer, eventOwner))
                            {
                                Debug.Log($" ● Found Execution Plan: {executionPlan}\n");
                                return executionPlan;
                            }

                        }
                    }
                    else
                    {
                        // Debug.Log($"    The executer ({executer}) must own the attribute '{currentOwnedAttribute.attribute}' to execute '{mapEvent}' but it does not. MapEvent owned by '{currentOwnedAttribute.ownerMapElement}'.\n");
                        if (mapEvent.ConsequencesCover(attributeOwnershipToCover, target, executer, executer))
                        {
                            ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, executer);
                            executionPlan.SetExecutionTimesToCover(attributeOwnershipToCover, remainingValueToCover);
                            Debug.Log($" ● Found 'forced' Execution Plan: {executionPlan}\n");
                            return executionPlan;
                        }
                    }

                }
            }


            return null;
        }
    }
}