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
     /// A spawned element of the map.
     /// </summary>
     [SelectionBase]
     public abstract class MapElement : MonoBehaviour
     {
          
          /// <summary>
          /// Collection (and manager) of the owned attributes of this "MapElement".
          /// </summary>
          [SerializeField] public AttributeManager attributeManager = new AttributeManager();
          
          /// <summary>
          /// The location of the camera for the POV view of the MapElement.
          /// </summary>
          [SerializeField] public Transform povCameraPrentTransform;
          
          #region Behaviour

               private IEnumerator coroutineHolder;
               
               private void Awake()
               {
                    attributeManager.Initialize(this);
                    
                    // Ensure that we are not going to lose the track of a previous coroutine 
                    // if we lose it, we'll not be able to stop it.
                    if (coroutineHolder != null)
                         StopCoroutine(coroutineHolder);

                    //Assign the coroutine to the holder
                    coroutineHolder = Clock();
                    //Run the coroutine
                    StartCoroutine(coroutineHolder);


                    //If you are not going yo need to stop the coroutine, you do not need the "coroutineHolder":
                    /*
                     * StartCoroutine(CourutineMethod());
                    */
                    
                    navMeshAgent = GetComponent<NavMeshAgent>();
               }
               
               private IEnumerator Clock()
               {
                    while (true)
                    {
                         // Set the time to wait until continuing the execution
                         yield return new WaitForSeconds(1f);
                         attributeManager.ExecuteSelfTimeElapseActions();
                         UpdateObjectiveAttribute();
                         DoNextPlanedMapEvent();
                    }
                    
                    // ReSharper disable once IteratorNeverReturns
               }

          #endregion
          
          #region Objective Attribute 
          
               private List<ExecutionPlan> currentExecutionPlans = new List<ExecutionPlan>();
               
               private OwnedAttribute currentObjectiveAttribute
               {
                    get => _currentObjectiveAttribute;
                    set
                    {
                         if (_currentObjectiveAttribute == value)
                              return;
                         
                         _currentObjectiveAttribute = value;
                         Debug.Log($"► Updating current objective attribute for '{this}' to '{(value!=null?value.attribute.name:"null")}'.");
                         if (value != null)
                         {
                              UpdateExecutionPlans();
                         }
                         else
                         {
                              currentExecutionPlans = null;
                         }

                    }
               }
               [CanBeNull] private OwnedAttribute _currentObjectiveAttribute;
               
               private void UpdateExecutionPlans()
               {
                    currentExecutionPlans = AppManager.gameManager.GetExecutionPlanToCover(currentObjectiveAttribute, 1, this);
                    if (currentExecutionPlans.IsNullOrEmpty())
                         Debug.LogWarning($"└> An action path to cover the attribute '{currentObjectiveAttribute.attribute}' was not found.\n");
                    else
                         Debug.Log($"└> Map Events to execute to cover '{currentObjectiveAttribute.attribute}':\n    ● {currentExecutionPlans.ToStringAllElements("\n    ● ")}\n", gameObject);
               }
               
               private void UpdateObjectiveAttribute()
               {
                    List<OwnedAttribute> attributesThatNeedCare = attributeManager.GetAttributesThatNeedCare();
                    if (attributesThatNeedCare.IsNullOrEmpty())
                    {
                         currentObjectiveAttribute = null;      
                    }
                    else
                    {
                         foreach (OwnedAttribute ownedAttribute in attributesThatNeedCare)
                         {
                              OwnedAttribute mostPrioritary = currentObjectiveAttribute;

                              if (mostPrioritary == null)
                                   mostPrioritary = ownedAttribute;
                              else if (!mostPrioritary.NeedsCare() || ownedAttribute.attribute.IsMorePrioritaryThan(mostPrioritary.attribute))
                                   mostPrioritary = ownedAttribute;

                              currentObjectiveAttribute = mostPrioritary;
                         }
                    }
                         
               }
               
               private void DoNextPlanedMapEvent()
               {
                    if (currentExecutionPlans.IsNullOrEmpty())
                    {
                         // Debug.LogError($"Trying to execute the next map event in the execution plan of '{this}', but it does not exist. The Execution Plan is null or empty.");
                         return;
                    }
                    
                    // new MoveAction(elementToCoverNeed.gameObject.transform.position)
                    int indexNextAction = currentExecutionPlans.Count-1;
                    ExecutionPlan executionPlan = currentExecutionPlans.ElementAt(indexNextAction);

                    List<OwnedAttribute> requirementsNotMet = executionPlan.GetRequirementsNotMet(out List<int> temp);
                    if (!requirementsNotMet.IsNullOrEmpty())
                    {
                         //ToDo: Do something when the requirements are not met
                         Debug.LogWarning($" > Not executing planed map event {executionPlan}.", gameObject);
                         requirementsNotMet.DebugLog("\n    ● ", $" > Requirements not met:\n    ● ", gameObject);
                    }
                    else if (!executionPlan.IsDistanceMet())
                    {
                         // Debug.Log($"Moving '{this}' to '{executionPlan.executionLocation}' to execute '{executionPlan.mapEvent}'");
                         executionPlan.executer.MoveTo(executionPlan.executionLocation);
                    }
                    else
                    {
                         Debug.Log($"        ◯ Executing next planed map event: {executionPlan}.", gameObject);
                         if (executionPlan.Execute())
                         {
                              currentExecutionPlans.RemoveAt(indexNextAction);
                              DoNextPlanedMapEvent(); // To enable chain of actions automatically done like "drop and use (before the materials are consumed)"
                              //currentExecutionPlans.DebugLog("\n    ● ", $"└> Remaining Map Events to execute to cover '{currentObjectiveAttribute.attribute}':\n    ● ", gameObject);
                         }
                         else
                         {
                              UpdateExecutionPlans();
                         }
                         
                    }
               }
          
          #endregion
          
          #region Default methods Overrides

               public override string ToString()
               {
                    return name;
               }

          #endregion
          
          #region Movement

               /// <summary>
               /// A reference to this MapElement's "NavMeshAgent". Can be null if the GameObject does not have it as a component.
               /// </summary>
               private NavMeshAgent navMeshAgent;

               /// <summary>
               /// Sets the destination of this object's "NavMeshAgent" and resumes its movement.
               /// </summary>
               /// <param name="location"></param>
               private void MoveTo(Vector3 location)
               {
                    navMeshAgent.SetDestination(location);
                    navMeshAgent.isStopped = false;
               }

          #endregion
          
     }
}