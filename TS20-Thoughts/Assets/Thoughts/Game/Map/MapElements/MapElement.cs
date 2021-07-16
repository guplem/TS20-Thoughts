using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Thoughts.Game.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{
     /// <summary>
     /// A spawned element of the Map.
     /// </summary>
     [SelectionBase]
     public class MapElement : MonoBehaviour
     {
          
          /// <summary>
          /// Collection (and manager) of the owned attributes of this MapElement.
          /// </summary>
          [Tooltip("Collection (and manager) of the owned attributes of this 'MapElement'")]
          [SerializeField] public AttributeManager attributeManager = new AttributeManager();

          /// <summary>
          /// Reference to the Animator handling the animations of this MapElement.
          /// </summary>
          [Tooltip("Reference to the Animator handling the animations of this MapElement")]
          [SerializeField] public Animator animator;

          /// <summary>
          /// The manager of the current state of this MapElement
          /// </summary>
          public StateManager stateManager { get; private set; }

          #region Behaviour
          
               /// <summary>
               /// Sets up and the MapElement
               /// <para>Initializes its attribute manager and looks up for its NavMeshAgent (initializes the variable).</para>
               /// </summary>
               private void Awake()
               {
                    stateManager = new StateManager(this);
                    attributeManager.Initialize(this);
                    navMeshAgent = GetComponent<NavMeshAgent>();
               }
               
               /// <summary>
               /// Starts running the MapElement's clock.
               /// <para>Starts the "UpdateCoroutine" coroutine.</para>
               /// </summary>
               private void Start()
               {
                    // Ensure that we are not going to lose the track of a previous coroutine 
                    // if we lose it, we'll not be able to stop it.
                    if (updateCoroutineHolder != null)
                         StopCoroutine(updateCoroutineHolder);

                    //Assign the coroutine to the holder
                    updateCoroutineHolder = UpdateCoroutine();
                    //Run the coroutine
                    StartCoroutine(updateCoroutineHolder);
               }
               
               /// <summary>
               /// Holder of the UpdateCoroutine of this MapElement
               /// </summary>
               private IEnumerator updateCoroutineHolder;
               
               /// <summary>
               /// Controls the internal clock of the MapElement.
               /// <para>Executes the map events with "execute with time elapse" enabled, updates the objective attribute to cover and tries to execute the next planned event.</para>
               /// </summary>
               private IEnumerator UpdateCoroutine()
               {
                    while (true)
                    {
                         yield return new WaitForSeconds(AppManager.gameManager.gameClockInterval);
                         
                         stateManager.Step(AppManager.gameManager.gameClockInterval);
                         attributeManager.ExecuteMapEventsWithTimeElapseEnabled();
                         if (stateManager.currentState == State.None)
                         {
                              UpdateObjectiveAttributeToCover();
                              if (!DoNextPlanedMapEvents())
                              {
                                   Debug.LogError("The next planned event could not be executed for unexpected reasons.");
                              }
                         }
                    }
                    
                    // ReSharper disable once IteratorNeverReturns
               }

          #endregion
          
          #region Objective Attribute

               /// <summary>
               /// Remaining execution plans to cover the objective attribute to cover
               /// </summary>
               public List<ExecutionPlan> executionPlans { get => _executionPlans; private set { _executionPlans = value; /*Debug.Log("*** UPDATE ORIGIN - New execution plan"); */onExecutionPlansUpdated?.Invoke(_executionPlans); } }
               private List<ExecutionPlan> _executionPlans = new List<ExecutionPlan>();
               public Action<List<ExecutionPlan>> onExecutionPlansUpdated;

               /// <summary>
               /// The current MAIN goal of the MapElement. After setting it, an overwrite of the executionPlans is going to be done.
               /// <para>Attribute whose value is to be increased. The attribute's value that is going to be increased must be owned by this MapElement.</para>
               /// </summary>
               public AttributeOwnership attributeOwnershipToCover
               {
                    get => _attributeOwnershipToCover;
                    private set
                    {
                         if (_attributeOwnershipToCover == value)
                              return;
                         _attributeOwnershipToCover = value;
                         
                         Debug.Log($"► Updating current objective attribute for '{this.ToString()}' to '{(value!=null?value.attribute.name:"null")}'.");

                         onObjectiveAttributeUpdated?.Invoke(_attributeOwnershipToCover);

                         UpdateExecutionPlansToCoverObjectiveAttribute();
                    }
               }
               [CanBeNull] private AttributeOwnership _attributeOwnershipToCover;
               public Action<AttributeOwnership> onObjectiveAttributeUpdated;
               
               /// <summary>
               /// Overwrites the executionPlans with a plan to cover (at least increasing the value by 1) of the current objectiveAttributeToCover (in this MapElement).
               /// </summary>
               private void UpdateExecutionPlansToCoverObjectiveAttribute()
               {
                    if (attributeOwnershipToCover == null)
                    {
                         executionPlans = null;
                    }
                    else
                    {
                         executionPlans = AppManager.gameManager.GetExecutionPlanToCover(attributeOwnershipToCover, 1, this);
                    
                         if (executionPlans.IsNullOrEmpty())
                              Debug.LogWarning($"└> An action path to cover the attribute '{attributeOwnershipToCover.attribute}' was not found.\n");
                         else
                              Debug.Log($"└> Map Events to execute to cover '{attributeOwnershipToCover.attribute}':\n    ● {executionPlans.ToStringAllElements("\n    ● ")}\n", gameObject);
                    }
               }
               
               /// <summary>
               /// Sets the objectiveAttributeToCover to the most needy attribute owned by this MapElement.
               /// </summary>
               private void UpdateObjectiveAttributeToCover()
               {
                    List<AttributeOwnership> attributesThatNeedCare = attributeManager.GetAttributesThatNeedCare();
                    
                    if (attributesThatNeedCare.IsNullOrEmpty())
                    {
                         attributeOwnershipToCover = null;      
                    }
                    else
                    {
                         foreach (AttributeOwnership needyAttribute in attributesThatNeedCare)
                         {
                              AttributeOwnership mostPrioritaryAttributeOwnership = attributeOwnershipToCover;

                              if (mostPrioritaryAttributeOwnership == null)
                                   mostPrioritaryAttributeOwnership = needyAttribute;
                              
                              else if (!mostPrioritaryAttributeOwnership.NeedsCare() || needyAttribute.attribute.IsMorePrioritaryThan(mostPrioritaryAttributeOwnership.attribute))
                                   mostPrioritaryAttributeOwnership = needyAttribute;

                              attributeOwnershipToCover = mostPrioritaryAttributeOwnership;
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
                    
                    //currentExecutionPlans.DebugLog("\n    ● ", $"└> Remaining Map Events to execute to cover '{currentObjectiveAttribute.attribute}':\n    ● ", gameObject);
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
               private NavMeshAgent navMeshAgent;

               /// <summary>
               /// The last location requested as destination for this MapElement. 
               /// </summary>
               private Vector3 lastRequestedDestination;
               
               /// <summary>
               /// Id of the trigger for the animation 'Move' used in the Animator 
               /// </summary>
               private static readonly int moveAnimTriggerId = Animator.StringToHash("Move");

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
                         animator.SetTrigger(moveAnimTriggerId);
                         return true;
                    }
                    else
                    {
                         Debug.LogWarning($"The location {location} could not be requested successfully as a destination for {this.ToString()}");
                         return false;
                    }
                    
               }

          #endregion
          
     }
}