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

        public List<OwnedAttribute> ownedAttributes
        {
            get { return _ownedAttributes; }
            private set { _ownedAttributes = value; }
        }
        [SerializeField] private List<OwnedAttribute> _ownedAttributes = new List<OwnedAttribute>();

        public void Initialize(MapElement owner)
        {
            this.owner = owner;
            foreach (OwnedAttribute attribute in ownedAttributes)
                attribute.UpdateOwner(this.owner);
        }

        public override string ToString()
        {
            return $"AttributeManager of {owner} with attributes '{ownedAttributes}'";
        }

        public void ExecuteMapEventsWithTimeElapseEnabled()
        {
            if (!ownedAttributes.IsNullOrEmpty())
                foreach (OwnedAttribute attribute in ownedAttributes)
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
            foreach (OwnedAttribute managerAttribute in ownedAttributes)
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
                ownedAttributes.Add(new OwnedAttribute(attributeToUpdate, deltaValue, owner, false));
            }
        }
        public List<OwnedAttribute> GetAttributesThatNeedCare()
        {
            List<OwnedAttribute> attributesThatNeedCare = new List<OwnedAttribute>();
            foreach (OwnedAttribute attribute in ownedAttributes)
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
        public bool CanCover(AttributeUpdate requirement, int times, /*out OwnedAttribute attributeInThisAttributeManagerThatCanCoverTheMostTheRequirement, */out int missingValueToCoverInThisAttributeManager)
        {
            missingValueToCoverInThisAttributeManager = requirement.value * times;

            if (times <= 0)
                Debug.LogWarning($"   - Attention: Checking if the AttributeManager of '{owner}' can cover the requirement '{requirement.ToString()}' {times} times!.\n");
            //else Debug.Log($"   - Checking if the AttributeManager of '{ownerMapElement}' can cover the requirement '{requirement.ToString()}' {times} times. Amount of value to gain: {missingValueToCoverInThisAttributeManager}\n");


            foreach (OwnedAttribute ownedAttribute in ownedAttributes)
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

        public OwnedAttribute GetOwnedAttributeOf(Attributes.Attribute attribute)
        {
            foreach (OwnedAttribute ownedAttribute in ownedAttributes)
            {
                if (ownedAttribute.attribute == attribute)
                    return ownedAttribute;
            }
            //ToDo: adding the attribute (next lines) should be done in another method. Maybe calling a new method calling 'GetOwnedAttributeAndAddItIfNotFound' should  be created to call them both
            Debug.Log($"   Attribute '{attribute}' not found in '{owner}' owned attributes. Adding the attribute with a value of 0.\n", owner);
            OwnedAttribute newAttribute = new OwnedAttribute(attribute, 0, owner, false);
            ownedAttributes.Add(newAttribute);
            return newAttribute;
        }
        public ExecutionPlan GetExecutionPlanToCover(OwnedAttribute ownedAttributeToCover, int remainingValueToCover, MapElement executer)
        {
            MapElement target = ownedAttributeToCover.ownerMapElement;

            // Debug.Log($" >>> Searching for an execution plan to cover '{remainingValueToCover}' of '{ownedAttributeToCover.attribute}' owned by '{ownedAttributeToCover.ownerMapElement}' executed by '{executer}'.\n");

            foreach (OwnedAttribute currentOwnedAttribute in ownedAttributes)
            {
                foreach (MapEvent mapEvent in currentOwnedAttribute.attribute.mapEvents)
                {
                    // Debug.Log($" >>> In '{ownerMapElement}', checking if mapEvent '{mapEvent}' in attribute '{currentOwnedAttribute.attribute}' can cover {remainingValueToCover} missing of the attribute '{ownedAttributeToCover.attribute}'.\nTarget: {target}, Executer: {executer}, EventOwner still unknown.\n");
                    if (!mapEvent.executerMustOwnAttribute || (mapEvent.executerMustOwnAttribute && currentOwnedAttribute.ownerMapElement == executer))
                    {
                        MapElement eventOwner = currentOwnedAttribute.ownerMapElement;
                        ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, eventOwner);
                        int executionsToCover = executionPlan.GetAndSetExecutionTimesToExecutionsToCover(ownedAttributeToCover, remainingValueToCover);
                        if (executionsToCover < 0) // The executionPlan can not cover the attribute
                            continue;

                        List<OwnedAttribute> mapEventRequirementsNotMet = mapEvent.GetRequirementsNotMet(owner, executer, target, executionPlan.executionTimes, out List<int> temp);

                        if (mapEvent.tryToCoverRequirementsIfNotMet || (!mapEvent.tryToCoverRequirementsIfNotMet && mapEventRequirementsNotMet.IsNullOrEmpty()))
                        {
                            // If reached here, the mapEvent can be executed - Now choose if it is the appropriate one
                            Debug.Log($"   > The mapEvent '{mapEvent}' can be executed ({mapEventRequirementsNotMet.Count} requirements must be covered before):\n    - {mapEventRequirementsNotMet.ToStringAllElements("\n    - ")}\n");

                            if (mapEvent.ConsequencesCover(ownedAttributeToCover, target, executer, eventOwner))
                            {
                                Debug.Log($" ● Found Execution Plan: {executionPlan}\n");
                                return executionPlan;
                            }

                        }
                    }
                    else
                    {
                        // Debug.Log($"    The executer ({executer}) must own the attribute '{currentOwnedAttribute.attribute}' to execute '{mapEvent}' but it does not. MapEvent owned by '{currentOwnedAttribute.ownerMapElement}'.\n");
                        if (mapEvent.ConsequencesCover(ownedAttributeToCover, target, executer, executer))
                        {
                            ExecutionPlan executionPlan = new ExecutionPlan(mapEvent, executer, target, executer);
                            executionPlan.GetAndSetExecutionTimesToExecutionsToCover(ownedAttributeToCover, remainingValueToCover);
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