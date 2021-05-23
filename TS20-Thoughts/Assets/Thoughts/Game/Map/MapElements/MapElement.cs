using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{
     [SelectionBase]
     public abstract class MapElement : MonoBehaviour
     {
          [SerializeField] public AttributeManager attributeManager = new AttributeManager();

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
          
          private void UpdateExecutionPlans()
          {
               currentExecutionPlans = AppManager.gameManager.GetExecutionPlanToCoverThisAttribute(currentObjectiveAttribute, 1, this);
               if (currentExecutionPlans.IsNullOrEmpty())
                    Debug.LogWarning($"└> An action path to cover the attribute '{currentObjectiveAttribute.attribute}' was not found.\n");
               else
                    Debug.Log($"└> Map Events to execute to cover '{currentObjectiveAttribute.attribute}':\n    ● {currentExecutionPlans.ToStringAllElements("\n    ● ")}\n", gameObject);
          }
          
          [CanBeNull] private OwnedAttribute _currentObjectiveAttribute;

          private IEnumerator coroutineHolder;
          
          public NavMeshAgent navMeshAgent { get; private set; }
          
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
          private void MoveTo(Vector3 location)
          {
               navMeshAgent.SetDestination(location);
               navMeshAgent.isStopped = false;
          }

          private IEnumerator Clock()
          {
               while (true)
               {
                    // Set the time to wait until continuing the execution
                    yield return new WaitForSeconds(1f);
                    attributeManager.ExecuteSelfTimeElapseActions();
                    SetObjectiveAttribute();
                    DoNextPlanedMapEvent();
               }
               
               // ReSharper disable once IteratorNeverReturns
          }
          
          private void SetObjectiveAttribute()
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

          public void UpdateAttribute(Attribute attribute, int deltaValue)
          {
               attributeManager.UpdateAttribute(attribute, deltaValue);
          }

          public override string ToString()
          {
               return name;
          }
     }
     
     
}
