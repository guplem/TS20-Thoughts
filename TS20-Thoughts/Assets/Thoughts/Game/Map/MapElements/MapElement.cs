
using System;
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
                         currentExecutionPlans = currentObjectiveAttribute.GetExecutionPlanToCoverThisAttribute(this);
                         if (currentExecutionPlans == null)
                              Debug.LogWarning($"└> An action path to take care of the stat '{currentObjectiveAttribute.attribute}' was not found.");
                         else
                              currentExecutionPlans.DebugLog(", ", $"└> Map Events to execute to cover '{currentObjectiveAttribute.attribute}': ", gameObject);

                    }
                    else
                    {
                         currentExecutionPlans = null;
                    }

               }
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
               if (currentExecutionPlans == null || currentExecutionPlans.Count <=0 )
               {
                    //Debug.LogError($"Trying to execute the next map event in the execution plan of '{this}', but it does not exist. The Execution Plan is null or empty.");
                    return;
               }
               
               // new MoveAction(elementToCoverNeed.gameObject.transform.position)
               int indexNextAction = currentExecutionPlans.Count-1;
               ExecutionPlan executionPlan = currentExecutionPlans.ElementAt(indexNextAction);
               
               if (!executionPlan.AreRequirementsMet())
               {
                    throw new NotImplementedException();
               }
               else if (!executionPlan.IsDistanceMet())
               {
                    // Debug.Log($"Moving '{this}' to '{executionPlan.executionLocation}' to execute '{executionPlan.mapEvent}'");
                    executionPlan.executer.MoveTo(executionPlan.executionLocation);
               }
               else
               {
                    Debug.Log($"        ◯ Executing next planed map event: {executionPlan}.");
                    if (executionPlan.Execute())
                         currentExecutionPlans.RemoveAt(0);
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
               
          }
          private void SetObjectiveAttribute()
          {
               List<OwnedAttribute> attributesThatNeedCare = attributeManager.GetAttributesThatNeedCare();
               if (attributesThatNeedCare == null || attributesThatNeedCare.Count <= 0)
                    currentObjectiveAttribute = null;
               else
                    currentObjectiveAttribute = attributesThatNeedCare.ElementAt(0);
          }

          public void UpdateAttribute(Attribute attribute, int deltaValue)
          {
               attributeManager.UpdateAttribute(attribute, deltaValue);
          }
          public MapEvent GetMapEventToTakeCareOf(Attribute attribute, AttributeUpdate.AttributeUpdateAffected affected, out MapElement ownerOfFoundMapEvent)
          {
               // Debug.Log($">>> Searching to take care of {attribute} in {this}", this);
               foreach (OwnedAttribute attributeManagerAttribute in attributeManager.attributes)
                    foreach (MapEvent mapEvent in attributeManagerAttribute.attribute.mapEvents)
                         foreach (AttributeUpdate consequence in mapEvent.consequences)
                              if (consequence.attribute == attribute && consequence.affected == affected && consequence.value > 0)
                              {
                                   ownerOfFoundMapEvent = attributeManager.ownerMapElement;
                                   return mapEvent;
                              }
               ownerOfFoundMapEvent = null;
               return null;
          }
     }
     
     
}
