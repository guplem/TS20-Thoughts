using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game.Attributes
{
    [Serializable]
    public class AttributeManager
    {
        public MapElement owner { get; private set; }

        public List<AttributeOwnership> ownedAttributes
        {
            get { return _ownedAttributes; }
            private set { _ownedAttributes = value; }
        }
        [SerializeField] private List<AttributeOwnership> _ownedAttributes = new List<AttributeOwnership>();

        public void Initialize(MapElement owner)
        {
            this.owner = owner;
            foreach (AttributeOwnership attribute in ownedAttributes)
                attribute.UpdateOwner(this.owner);
        }

        public override string ToString()
        {
            return $"AttributeManager of {owner} with attributes '{ownedAttributes}'";
        }

        public void ExecuteMapEventsWithTimeElapseEnabled()
        {
            if (ownedAttributes.IsNullOrEmpty())
                return;
            
            foreach (AttributeOwnership attribute in ownedAttributes)
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

        public void UpdateAttribute(Attributes.Attribute attributeToUpdate, int deltaValue)
        {
            bool found = false;
            foreach (AttributeOwnership managerAttribute in ownedAttributes)
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
                ownedAttributes.Add(new AttributeOwnership(attributeToUpdate, deltaValue, owner, false));
            }
        }
        public List<AttributeOwnership> GetAttributesThatNeedCare()
        {
            List<AttributeOwnership> attributesThatNeedCare = new List<AttributeOwnership>();
            foreach (AttributeOwnership attribute in ownedAttributes)
            {
                if (attribute.NeedsCare())
                    attributesThatNeedCare.Add(attribute);
            }
            return attributesThatNeedCare;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requirement"></param>
        /// <param name="times">The amount of times the requirement will have to be met</param>
        /// <param name="missingValueToCoverInThisAttributeManager"></param>
        /// <returns>True if it contains an attribute with a value higher or equal than the one in the requirement/AttributeUpdate n times</returns>
        public bool CanCover(Requirement requirement, int times, /*out OwnedAttribute attributeInThisAttributeManagerThatCanCoverTheMostTheRequirement, */out int missingValueToCoverInThisAttributeManager)
        {
            missingValueToCoverInThisAttributeManager = requirement.minValue * times;

            if (times <= 0)
                Debug.LogWarning($"   - Attention: Checking if the AttributeManager of '{owner}' can cover the requirement '{requirement.ToString()}' {times} times!.\n");
            //else Debug.Log($"   - Checking if the AttributeManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");


            foreach (AttributeOwnership ownedAttribute in ownedAttributes)
            {
                if (requirement.attribute != ownedAttribute.attribute)
                    continue;

                missingValueToCoverInThisAttributeManager -= ownedAttribute.value;

                if (missingValueToCoverInThisAttributeManager <= 0) // No value is missing, so the requirement can be covered
                {
                    // Debug.Log($"   - The AttributeManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");
                    return true;
                }
            }

            // Debug.Log($"   - The AttributeManager of '{ownerMapElement}' can NOT cover the requirement '{requirement.ToString()}' {times} times. Remaining amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");
            return false;
        }

        public AttributeOwnership GetOwnedAttributeOf(Attributes.Attribute attribute)
        {
            foreach (AttributeOwnership ownedAttribute in ownedAttributes)
            {
                if (ownedAttribute.attribute == attribute)
                    return ownedAttribute;
            }
            //ToDo: adding the attribute (next lines) should be done in another method. Maybe calling a new method calling 'GetOwnedAttributeAndAddItIfNotFound' should  be created to call them both
            Debug.Log($"   Attribute '{attribute}' not found in '{owner}' owned attributes. Adding the attribute with a value of 0.\n", owner);
            AttributeOwnership newAttributeOwnership = new AttributeOwnership(attribute, 0, owner, false);
            ownedAttributes.Add(newAttributeOwnership);
            return newAttributeOwnership;
        }
        public ExecutionPlan GetExecutionPlanToCover(AttributeOwnership attributeOwnershipToCover, int remainingValueToCover, MapElement executer)
        {
            MapElement target = attributeOwnershipToCover.owner;

            // Debug.Log($" >>> Searching for an execution plan to cover '{remainingValueToCover}' of '{attributeToCover.attribute}' owned by '{attributeToCover.ownerMapElement}' executed by '{executer}'.\n");

            foreach (AttributeOwnership ownedAttribute in ownedAttributes)
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