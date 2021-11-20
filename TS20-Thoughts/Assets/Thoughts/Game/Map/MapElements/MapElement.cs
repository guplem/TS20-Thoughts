using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using JetBrains.Annotations;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.MapElements.Properties.MapEvents;
using Thoughts.Game.Map.Properties;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.MapElements
{
     /// <summary>
     /// A spawned element of the Map.
     /// </summary>
     [SelectionBase]
     public class MapElement : MonoBehaviour
     {
          [Header("Animations")]
          [SerializeField] internal AnimationsManager animationsManager;
          
          /// <summary>
          /// Collection (and manager) of the owned properties of this MapElement.
          /// </summary>
          
          [Header("Properties")]
          [Tooltip("Collection (and manager) of the owned properties of this 'MapElement'")]
          [SerializeField] public PropertyManager propertyManager = new PropertyManager();

          /// <summary>
          /// The manager of the current state of this MapElement
          /// </summary>
          public StateManager stateManager { get; private set; }

          #region Behaviour
          
               /// <summary>
               /// Sets up and the MapElement
               /// <para>Initializes its property manager and looks up for its NavMeshAgent (initializes the variable).</para>
               /// </summary>
               private void Awake()
               {
                    stateManager = new StateManager(this);
                    propertyManager.Initialize(this);
                    navMeshAgent = GetComponent<NavMeshAgent>();
               }
               
               /// <summary>
               /// Starts running the MapElement's clock.
               /// <para>Starts the "ClockCoroutine" coroutine.</para>
               /// </summary>
               private void Start()
               {
                    // To ensure that all properties follow the expected behaviour/settings
                    foreach (Property property in propertyManager.GetListOfAllCurrentProperties())
                    {
                         propertyManager.UpdateProperty(property,0);
                    }
                    
                    // Ensure that we are not going to lose the track of a previous coroutine 
                    // if we lose it, we'll not be able to stop it.
                    if (updateCoroutineHolder != null)
                         StopCoroutine(updateCoroutineHolder);

                    //Assign the coroutine to the holder
                    updateCoroutineHolder = ClockCoroutine();
                    //Run the coroutine
                    StartCoroutine(updateCoroutineHolder);
               }
               
               /// <summary>
               /// Holder of the ClockCoroutine of this MapElement
               /// </summary>
               private IEnumerator updateCoroutineHolder;
               
               /// <summary>
               /// Controls the internal clock of the MapElement.
               /// <para>Executes the map events with "execute with time elapse" enabled, updates the objective property to cover and tries to execute the next planned event.</para>
               /// </summary>
               private IEnumerator ClockCoroutine()
               {
                    // Initial delay so not all of them start at the same time
                    yield return new WaitForSeconds(new RandomEssentials(transform.position.GetHashCode()).GetRandomFloat(5f)); // Maximum 5 seconds of delay to start the clock
                    
                    while (true)
                    {
                         yield return new WaitForSeconds(GameManager.instance.gameClockInterval);

                         if (stateManager.currentState == State.None)
                         {
                              UpdateObjectivePropertyToCover();
                              if (!DoNextPlanedMapEvents())
                              {
                                   Debug.LogWarning("The next planned event could not be executed. Updating the execution plans to cover the objective property");
                                   UpdateExecutionPlansToCoverObjectiveProperty();
                              }
                         }
                         
                         // Order is important:
                         propertyManager.ExecuteMapEventsWithTimeElapseEnabled();
                         animationsManager.UpdateAnimationsUpdates(this);
                         stateManager.Step(GameManager.instance.gameClockInterval);
                         animationsManager.PlayStateAnimation(stateManager.currentState, this);
                    }
                    
                    // ReSharper disable once IteratorNeverReturns
               }

          #endregion
          
          #region Objective Property

               /// <summary>
               /// Remaining execution plans to cover the objective property to cover
               /// </summary>
               public List<ExecutionPlan> executionPlans { get => _executionPlans; private set { _executionPlans = value; /*Debug.Log("*** UPDATE ORIGIN - New execution plan"); */onExecutionPlansUpdated?.Invoke(_executionPlans); } }
               private List<ExecutionPlan> _executionPlans = new List<ExecutionPlan>();
               public Action<List<ExecutionPlan>> onExecutionPlansUpdated;

               /// <summary>
               /// The current MAIN goal of the MapElement. After setting it, an overwrite of the executionPlans is going to be done.
               /// <para>Property whose value is to be increased. The property's value that is going to be increased must be owned by this MapElement.</para>
               /// </summary>
               public PropertyOwnership propertyOwnershipToCover
               {
                    get => _propertyOwnershipToCover;
                    private set
                    {
                         if (_propertyOwnershipToCover == value)
                              return;
                         _propertyOwnershipToCover = value;
                         
                         Debug.Log($"► Updating current objective property for '{this.ToString()}' to '{(value!=null?value.property.name:"null")}'.");

                         onObjectivePropertyUpdated?.Invoke(_propertyOwnershipToCover);

                         UpdateExecutionPlansToCoverObjectiveProperty();
                    }
               }
               [CanBeNull] private PropertyOwnership _propertyOwnershipToCover;
               public Action<PropertyOwnership> onObjectivePropertyUpdated;
               
               /// <summary>
               /// Overwrites the executionPlans with a plan to cover (at least increasing the value by 1) of the current objectivePropertyToCover (in this MapElement).
               /// </summary>
               private void UpdateExecutionPlansToCoverObjectiveProperty()
               {
                    if (propertyOwnershipToCover == null)
                    {
                         executionPlans = null;
                    }
                    else
                    {
                         executionPlans = GameManager.instance.GetExecutionPlanToCover(propertyOwnershipToCover, 1, this);
                    
                         if (executionPlans.IsNullOrEmpty())
                              Debug.LogWarning($"└> An action path to cover the property '{propertyOwnershipToCover.property}' was not found.\n");
                         else
                              Debug.Log($"└> Map Events to execute to cover '{propertyOwnershipToCover.property}':\n    ● {executionPlans.ToStringAllElements("\n    ● ")}\n", gameObject);
                    }
               }
               
               /// <summary>
               /// Sets the objectivePropertyToCover to the most needy property owned by this MapElement.
               /// </summary>
               private void UpdateObjectivePropertyToCover()
               {
                    List<PropertyOwnership> propertiesThatNeedCare = propertyManager.GetPropertiesThatNeedCare();
                    
                    if (propertiesThatNeedCare.IsNullOrEmpty())
                    {
                         propertyOwnershipToCover = null;      
                    }
                    else
                    {
                         foreach (PropertyOwnership needyProperty in propertiesThatNeedCare)
                         {
                              PropertyOwnership mostPrioritaryPropertyOwnership = propertyOwnershipToCover;

                              if (mostPrioritaryPropertyOwnership == null)
                                   mostPrioritaryPropertyOwnership = needyProperty;
                              
                              else if (!mostPrioritaryPropertyOwnership.NeedsCare() || needyProperty.property.IsMorePrioritaryThan(mostPrioritaryPropertyOwnership.property))
                                   mostPrioritaryPropertyOwnership = needyProperty;

                              propertyOwnershipToCover = mostPrioritaryPropertyOwnership;
                         }
                    }
               }
               
               /// <summary>
               /// Tries to execute the next planned even in the executionPlans list.
               /// <para>If the distance to execute the MapEvent is not met, an order to move this MapElement is given instead.</para>
               /// <para>After executing the next planned event, an order to execute the next one is going to be given immediately.</para>
               /// <para>It is considered the "next" map event the last one added in the list.</para>
               /// </summary>
               /// <returns>True if the behaviour was expected (the planned event was executed successfully, the distance was not met so the object was moved, ...). False if the planned event could not be executed due to unexpected reasons.</returns>
               private bool DoNextPlanedMapEvents()
               {
                    if (stateManager.currentState != State.None)
                         return true;
                    
                    if (executionPlans.IsNullOrEmpty())
                    {
                         // Debug.LogError($"Trying to execute the next map event in the execution plan of '{this}', but it does not exist. The Execution Plan is null or empty.");
                         return true;
                    }
                    
                    // new MoveAction(elementToCoverNeed.gameObject.transform.position)
                    int indexNextAction = executionPlans.Count-1;
                    ExecutionPlan executionPlan = executionPlans.ElementAt(indexNextAction);
                    
                    if (!executionPlan.IsDistanceMet())
                    {
                         // Debug.Log($"Moving '{this}' to '{executionPlan.executionLocation}' to execute '{executionPlan.mapEvent}'");
                         executionPlan.executer.MoveTo(executionPlan.executionLocation);
                         return true;
                    }
                    
                    Debug.Log($"        ◯ Executing next planed map event: {executionPlan}.", gameObject);
                         
                    if (!executionPlan.Execute())
                         return false;

                    RemoveExecutionPlanElement(indexNextAction);
                    
                    //currentExecutionPlans.DebugLog("\n    ● ", $"└> Remaining Map Events to execute to cover '{currentObjectiveProperty.property}':\n    ● ", gameObject);
                    return DoNextPlanedMapEvents(); // To enable chain of actions automatically done like "drop and use (before the materials are consumed)"
               }
          
               /// <summary>
               /// Removes an ExecutonPlan from the executionPlans list 
               /// </summary>
               /// <param name="elementIndex">The index of the ExecutionPlan to remove from the executionPlans list</param>
               public void RemoveExecutionPlanElement(int elementIndex)
               {
                    //Debug.Log("*** UPDATE ORIGIN - Element removed");
                    executionPlans.RemoveAt(elementIndex);
                    onExecutionPlansUpdated?.Invoke(_executionPlans); 
               }
               
          #endregion
          
          #region Default methods Overrides

               /// <summary>
               /// Returns a string that represents the current object.
               /// </summary>
               /// <returns>A string that represents the current object.</returns>
               public override string ToString()
               {
                    return name;
               }

          #endregion
          
          #region Movement

          /// <summary>
          /// A reference to this MapElement's NavMeshAgent. Can be null if the GameObject does not have it as a component.
          /// </summary>
          public NavMeshAgent navMeshAgent { get; private set; }

          /// <summary>
          /// The last location requested as destination for this MapElement. 
          /// </summary>
          private Vector3 lastRequestedDestination;

          /// <summary>
          /// Sets the destination of this object's NavMeshAgent, resumes its movement and plays the 'Move' animation.
          /// </summary>
          /// <param name="location">The location to move to.</param>
          /// <returns>True if the destination was requested successfully. Otherwise, false.</returns>
          private bool MoveTo(Vector3 location)
          {
               if (location == lastRequestedDestination && !navMeshAgent.isStopped)
                    return true;
               
               Debug.Log($"Moving MapElement ({this.ToString()}) to {location}. Previous destination = {navMeshAgent.destination}");;
               if (navMeshAgent == null)
               {
                    Debug.LogWarning($"Trying to move a MapElement ({this.ToString()}) that can not be moved (NavMeshAgent == null).");
                    return false;
               }
               
               if (navMeshAgent.SetDestination(location))
               {
                    lastRequestedDestination = location;
                    navMeshAgent.isStopped = false;
                    return true;
               }
               else
               {
                    Debug.LogWarning($"The location {location} could not be requested successfully as a destination for {this.ToString()}");
                    return false;
               }
               
          }

          #endregion

          public string GetStatus()
          {
               if (IsMoving())
               {
                    if (executionPlans.Count - 1 >= 0)
                    {
                         ExecutionPlan executionPlan = executionPlans.ElementAt(executionPlans.Count-1);
                         return $"Moving towards '{executionPlan.eventOwner}' to '{executionPlan.mapEvent}' and end up covering '{propertyOwnershipToCover.property}'";
                    }
                    Debug.LogWarning($"The map element '{this.ToString()}' is moving towards '{navMeshAgent.destination}' (closest MapElement: '{GameManager.instance.mapManager.GetClosestMapElementTo(navMeshAgent.destination)}') but it has no execution plan. Current property to cover: '{propertyOwnershipToCover.property}'");
                    return $"Moving towards '{navMeshAgent.destination}'. Current property to cover: '{propertyOwnershipToCover.property}'";
               }
               else
               {
                    return $"Status: {stateManager.currentState} ({stateManager.remainingStateTime}s)";
               }
          }

          public bool IsMoving()
          {
               
               return navMeshAgent != null && navMeshAgent.velocity.magnitude > 0.15f;
          }
     }
}