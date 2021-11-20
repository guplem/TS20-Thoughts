using System;
using System.Collections.Generic;
using Thoughts.Game.Map.MapEvents;
using Thoughts.Game.Map.Properties;
using UnityEngine;

namespace Thoughts.Game.Map.MapElements.Properties.MapEvents
{
    /// <summary>
    /// The execution configuration for a MapEvent. A plan to execute a MapEvent (of a Property) owned by a MapElement, executed by a MapElement and with a targeted MapElement.
    /// </summary>
    public class ExecutionPlan
    {
        /// <summary>
        /// The MapEvent to execute
        /// </summary>
        public MapEvent mapEvent { get; private set; }
        
        /// <summary>
        /// The Property containing the MapEvent to execute
        /// </summary>
        public Property property { get; private set; }
        
        /// <summary>
        /// The MapElement that owns the MapEvent to execute (in one of its owned Properties)
        /// </summary>
        public MapElement eventOwner { get; private set; }
        
        /// <summary>
        /// The executor MapElement of the MapEvent
        /// </summary>
        public MapElement executer { get; private set; }
        
        /// <summary>
        /// The targeted MapElement with the execution of the MapEvent
        /// </summary>
        public MapElement target { get; private set; }
        
        /// <summary>
        /// The amount of times remaining to execute this plan's MapEvent.
        /// <para>It is automatically updated</para>
        /// </summary>
        public int executionTimes { get; private set; }

        /// <summary>
        /// The location where the MapEvent must be executed
        /// </summary>
        public Vector3 executionLocation
        {
            get
            {
                if (eventOwner == executer)
                    return target.transform.position;

                if (target != executer)
                    return target.transform.position;

                return eventOwner.transform.position;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionPlan"/> class.
        /// </summary>
        /// <param name="mapEvent">The MapEvent to execute</param>
        /// <param name="executer">The executor MapElement of the MapEvent</param>
        /// <param name="target">The targeted MapElement with the execution of the MapEvent</param>
        /// <param name="eventOwner">The MapElement that owns the MapEvent to execute (in one of its owned Properties)</param>
        /// <param name="property">The Property containing the MapEvent to execute</param>
        /// <param name="executionTimes">The amount of times remaining to execute this plan's MapEvent. 1 by default.</param>
        public ExecutionPlan(MapEvent mapEvent, MapElement executer, MapElement target, MapElement eventOwner, Property property, int executionTimes = 1)
        {
            this.mapEvent = mapEvent;
            this.executer = executer;
            this.target = target;
            this.eventOwner = eventOwner;
            this.property = property;
            this.executionTimes = executionTimes;
        }

        public override string ToString()
        {
            return $"x{executionTimes} '{mapEvent}' of property '{property}'. Executed by '{executer}' to target '{target}' at/owned by '{eventOwner}'.";
        }

        /// <summary>
        /// If possible, executes the MapEvent of this ExecutionPlan with its configuration the amount of times set in the executionTimes field of this instance.
        /// </summary>
        /// <returns>True if the plan could be executed all the desired times. False otherwise.</returns>
        public bool Execute()
        {
            string requirementsNotMetMessage;
            if (CanBeExecuted(out requirementsNotMetMessage))
            {
                mapEvent.Execute(executer, target, eventOwner, property);
                executionTimes--;
                
                if (executionTimes > 0)
                    return this.Execute();
                
                return true;
            }

            Debug.LogWarning($"Trying to execute an ExecutionPlan that can not be executed (stored data of executionTimes = {executionTimes}).  Reasons:\n{requirementsNotMetMessage}");
            return false;
        }

        /// <summary>
        /// Determines if all the requirements to execute this ExecutionPlan's MapElement are met.
        /// </summary>
        /// <param name="requirementsNotMetMessage">A debug message listing the requirements that are not met.</param>
        /// <returns>True, if the requirements to execute this ExecutionPlan are met. False, otherwise.</returns>
        private bool CanBeExecuted(out string requirementsNotMetMessage)
        {
            Dictionary<PropertyOwnership, float> requirementsNotMet = GetRequirementsNotMet();
            bool allRequirementsMet = requirementsNotMet.IsNullOrEmpty();
            bool isDistanceMet = IsDistanceMet();

            requirementsNotMetMessage = "";
            if (!allRequirementsMet)
                requirementsNotMetMessage += $"  ◍ Requirements not met:\n      - {requirementsNotMet.ToStringAllElements("\n      - ")}\n";
            if (!isDistanceMet)
                requirementsNotMetMessage += $"  ◍ Distance is not met not met.\n";

            return allRequirementsMet && isDistanceMet;
        }

        /// <summary>
        /// Determines if the distance to execute this ExecutionPlan's MapElement is met.
        /// </summary>
        /// <returns>True, if the distance to execute this ExecutionPlan's MapElement is met. False, otherwise.</returns>
        public bool IsDistanceMet()
        {
            return mapEvent.IsDistanceMet(executer, target, eventOwner);
        }

        /// <summary>
        /// Returns a list of the requirements that are not met at the moment to execute the event, it and outputs a list of the value missing for each one of the requirements that are not met (in the same order).
        /// </summary>
        /// <returns>A list of the requirements that are not met at the moment to execute the event (the keys), each one of them related to the value missing (value to cover)</returns>
        public Dictionary<PropertyOwnership, float> GetRequirementsNotMet()
        {
            return mapEvent.GetRequirementsNotMet(property, executer, target, eventOwner, executionTimes);
        }

        /// <summary>
        /// Calculates the amount of times that the execution of this ExecutionPlan's MapElement is needed to cover a given property (to increase its value a defined amount).
        /// </summary>
        /// <param name="propertyOwnershipToCover">PropertyOwnership that is desired to cover (to increase its value in the MapElement's PropertyManager)</param>
        /// <param name="remainingValueToCover">The remaining value to cover for the given Property.</param>
        /// <returns></returns>
        private int CalculateExecutionsNeededToCover(PropertyOwnership propertyOwnershipToCover, float remainingValueToCover)
        {
            float coveredPerExecution = 0;

            //Debug.LogWarning($"Calculating how many times '{this.mapEvent}' must be executed to cover '{ownedPropertyToCover.property}'...");

            foreach (Consequence consequence in mapEvent.consequences)
            {
                //Debug.Log($"CHECKING {consequence.property} against {ownedPropertyToCover.property}");
                if (consequence.GetProperty(this.property) == propertyOwnershipToCover.property)
                {
                    switch (consequence.affectedMapElement)
                    {
                        case AffectedMapElement.eventOwner:
                            if (this.eventOwner == propertyOwnershipToCover.owner)
                                coveredPerExecution += consequence.deltaValue;

                            break;
                        case AffectedMapElement.eventExecuter:
                            if (this.executer == propertyOwnershipToCover.owner)
                                coveredPerExecution += consequence.deltaValue;

                            break;
                        case AffectedMapElement.eventTarget:
                            if (this.target == propertyOwnershipToCover.owner)
                                coveredPerExecution += consequence.deltaValue;

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            int result = -1;
            if (coveredPerExecution > 0)
                result = (int) Math.Ceiling(((double) remainingValueToCover) / ((double) coveredPerExecution));

            //Debug.LogWarning($"It needs to be executed {result} times to cover {remainingValueToCover} of remaining value. It covers {coveredPerExecution} per execution.");

            if (coveredPerExecution >= remainingValueToCover)
                return 1;

            if (coveredPerExecution > 0)
                return result;

            // Debug.LogWarning($"'{this}' can not cover '{ownedPropertyToCover}'.");
            return -1;
        }

        /// <summary>
        /// Sets the amount of times remaining to execute this plan's MapEvent to the same amount needed to to cover a given property (to increase its value a defined amount).
        /// </summary>
        /// <param name="propertyOwnershipToCoverr">PropertyOwnership that is desired to cover (to increase its value in the MapElement's PropertyManager)</param>
        /// <param name="remainingValueToCover">The remaining value to cover for the given Property.</param>
        public void SetExecutionTimesToCover(PropertyOwnership propertyOwnershipToCover, float remainingValueToCover)
        {
            this.executionTimes = CalculateExecutionsNeededToCover(propertyOwnershipToCover, remainingValueToCover);
        }

    }
}
