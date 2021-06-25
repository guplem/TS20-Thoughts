using System;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using UnityEngine;

namespace Thoughts.Game.GameMap
{
    /// <summary>
    /// The execution configuration for a MapEvent. A plan to execute a MapEvent owned by a MapElement, executed by a MapElement and with a targeted MapElement.
    /// </summary>
    public class ExecutionPlan
    {
        /// <summary>
        /// The MapEvent to execute
        /// </summary>
        public MapEvent mapEvent { get; private set; }
        
        /// <summary>
        /// The MapElement that owns the MapEvent to execute (in one of its owned Attributes)
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
        public ExecutionPlan(MapEvent mapEvent, MapElement executer, MapElement target, MapElement eventOwner, int executionTimes = 1)
        {
            this.mapEvent = mapEvent;
            this.executer = executer;
            this.target = target;
            this.eventOwner = eventOwner;
            this.executionTimes = executionTimes;
        }

        public override string ToString()
        {
            return $"'{mapEvent}' (x{executionTimes} times by '{executer}' to target '{target}' at/owned by '{eventOwner}')";
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
                mapEvent.Execute(executer, target, eventOwner);
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
            List<OwnedAttribute> requirementsNotMet = GetRequirementsNotMet(out List<int> temp);
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
        /// <param name="remainingValueToCoverInRequirementsNotMet">A list of the value missing for each one of the requirements that are not met (in the same order than the returned list of requirements not met)</param>
        /// <returns>A list of the requirements that are not met at the moment to execute the event.</returns>
        public List<OwnedAttribute> GetRequirementsNotMet(out List<int> remainingValueToCoverInRequirementsNotMet)
        {
            return mapEvent.GetRequirementsNotMet(executer, target, eventOwner, executionTimes, out remainingValueToCoverInRequirementsNotMet);
        }

        /// <summary>
        /// Calculates the amount of times that the execution of this ExecutionPlan's MapElement is needed to cover a given attribute (to increase its value a defined amount).
        /// </summary>
        /// <param name="attributeToCover">The Attribute that is desired to cover (to increase its value in the MapElement's AttributeManager)</param>
        /// <param name="remainingValueToCover">The remaining value to cover for the given Attribute.</param>
        /// <returns></returns>
        private int CalculateExecutionsNeededToCover(OwnedAttribute attributeToCover, int remainingValueToCover)
        {
            int coveredPerExecution = 0;

            //Debug.LogWarning($"Calculating how many times '{this.mapEvent}' must be executed to cover '{ownedAttributeToCover.attribute}'...");

            foreach (AttributeUpdate consequence in mapEvent.consequences)
            {
                //Debug.Log($"CHECKING {consequence.attribute} against {ownedAttributeToCover.attribute}");
                if (consequence.attribute == attributeToCover.attribute)
                {
                    switch (consequence.affected)
                    {
                        case AttributeUpdate.AttributeUpdateAffected.eventOwner:
                            if (this.eventOwner == attributeToCover.owner)
                                coveredPerExecution += consequence.value;

                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventExecuter:
                            if (this.executer == attributeToCover.owner)
                                coveredPerExecution += consequence.value;

                            break;
                        case AttributeUpdate.AttributeUpdateAffected.eventTarget:
                            if (this.target == attributeToCover.owner)
                                coveredPerExecution += consequence.value;

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

            // Debug.LogWarning($"'{this}' can not cover '{ownedAttributeToCover}'.");
            return -1;
        }

        /// <summary>
        /// Sets the amount of times remaining to execute this plan's MapEvent to the same amount needed to to cover a given attribute (to increase its value a defined amount).
        /// </summary>
        /// <param name="attributeToCover">The Attribute that is desired to cover (to increase its value in the MapElement's AttributeManager)</param>
        /// <param name="remainingValueToCover">The remaining value to cover for the given Attribute.</param>
        public void SetExecutionTimesToCover(OwnedAttribute attributeToCover, int remainingValueToCover)
        {
            this.executionTimes = CalculateExecutionsNeededToCover(attributeToCover, remainingValueToCover);
        }

    }
}
