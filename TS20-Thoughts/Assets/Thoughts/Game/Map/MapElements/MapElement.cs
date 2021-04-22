
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
          
          private Attribute currentObjectiveAttribute
          {
               get => _currentObjectiveAttribute;
               set
               {
                    _currentObjectiveAttribute = value;
                    Debug.Log($"► Updating current objective stat for '{this}' to '{value}'.");
                    currentExecutionPlans = currentObjectiveAttribute.GetExecutionPlanToSatisfyThisAttribute(this);
                    if (currentExecutionPlans == null)
                         Debug.LogWarning($"└> An action path to take care of the stat '{currentObjectiveAttribute}' was not found.");
                    else
                    {
                         currentExecutionPlans.DebugLog(", ", $"└> Map Events to perform to cover '{currentObjectiveAttribute}' stat: ", gameObject);
                         DoNextMapEvent();    
                    }

               }
          }
          [CanBeNull] private Attribute _currentObjectiveAttribute;

          private IEnumerator coroutineHolder;
          
          public NavMeshAgent navMeshAgent { get; private set; }
          
          private void Awake()
          {
               attributeManager.Initialize();
               
               // Ensure that we are not going to lose the track of a previous coroutine 
               // if we lose it, we'll not be able to stop it.
               if (coroutineHolder != null)
                    StopCoroutine(coroutineHolder);

               //Assign the coroutine to the holder
               coroutineHolder = InventoryTimeElapse();
               //Run the coroutine
               StartCoroutine(coroutineHolder);


               //If you are not going yo need to stop the coroutine, you do not need the "coroutineHolder":
               /*
                * StartCoroutine(CourutineMethod());
               */
               
               
               
               navMeshAgent = GetComponent<NavMeshAgent>();
          }
          
          private void DoNextMapEvent()
          {
               throw new NotImplementedException();
               /*if (currentExecutionPlans == null || currentExecutionPlans.Count <=0 )
               {
                    Debug.LogError($"Trying to execute the next map event of '{this}' but it does not exist. The action path is null or empty.");
                    return;
               }
               
               // new MoveAction(elementToCoverNeed.gameObject.transform.position)
               int indexNextAction = currentExecutionPlans.Count-1;
               MapEvent mapEventInAttributeAtMapElement = currentExecutionPlans.ElementAt(indexNextAction).mapEvent;

               Debug.Log($"        ◯ Executing map event '{mapEventInAttributeAtMapElement}' by '{this}' in '{mapEventInAttributeAtMapElement.mapElement}' with attribute '{mapEventInAttributeAtMapElement.attribute}'");
               MapEvent nextEnqueuedEventInExecuter = indexNextAction >= 1 ? currentExecutionPlans.ElementAt(indexNextAction - 1) : null;
               mapEventInAttributeAtMapElement.Execute(this, mapEventInAttributeAtMapElement, mapEventInAttributeAtMapElement.attribute, nextEnqueuedEventInExecuter);
            
               currentExecutionPlans.RemoveAt(0);*/
          }
          
          private IEnumerator InventoryTimeElapse()
          {
               while (true)
               {
                    // Set the time to wait until continuing the execution
                    yield return new WaitForSeconds(1f);
                    attributeManager.ExecuteTimeElapseActions(this);
               }
               
          }
          
     }
     
     
}
