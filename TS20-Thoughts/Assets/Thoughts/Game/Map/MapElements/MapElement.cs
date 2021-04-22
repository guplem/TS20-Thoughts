
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{
     [SelectionBase]
     public abstract class MapElement : MonoBehaviour
     {
          [SerializeField] public AttributeManager attributeManager = new AttributeManager();
          
          private Stat currentObjectiveStat
          {
               get => _currentObjectiveStat;
               set
               {
                    _currentObjectiveStat = value;
                    Debug.Log($"► Updating current objective stat for '{this}' to '{value}'.");
                    currentActionPath = currentObjectiveStat.GetEventsToSatisfyThisStat(this);
                    if (currentActionPath == null)
                         Debug.LogWarning($"└> An action path to take care of the stat '{currentObjectiveStat}' was not found.");
                    else
                    {
                         currentActionPath.DebugLog(", ", $"└> Map Events to perform to cover '{currentObjectiveStat}' stat: ", gameObject);
                         DoNextAction();    
                    }

               }
          }
          [CanBeNull]
          private Stat _currentObjectiveStat;
          private List<MapEventInAttributeAtMapElement> currentActionPath = new List<MapEventInAttributeAtMapElement>();

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
          
        
          private void DoNextAction() // false if distance is too big
          {
               if (currentActionPath == null || currentActionPath.Count <=0 )
               {
                    Debug.LogError($"Trying to execute the next action of '{this}' but it does not exist.");
                    return;
               }
               
               // new MoveAction(elementToCoverNeed.gameObject.transform.position)
               int indexNextAction = currentActionPath.Count-1;
               MapEventInAttributeAtMapElement mapEventInAttributeAtMapElement = currentActionPath.ElementAt(indexNextAction);

               Debug.Log($"        ◯ Executing action '{mapEventInAttributeAtMapElement.mapEvent}' by '{this}' in '{mapEventInAttributeAtMapElement.mapElement}' with attribute '{mapEventInAttributeAtMapElement.attribute}'");
               MapEventInAttributeAtMapElement nextEnqueuedEventInExecuter = indexNextAction >= 1 ? currentActionPath.ElementAt(indexNextAction - 1) : null;
               mapEventInAttributeAtMapElement.mapEvent.Execute(this, mapEventInAttributeAtMapElement.mapElement, mapEventInAttributeAtMapElement.attribute, nextEnqueuedEventInExecuter);
            
               currentActionPath.RemoveAt(0);
          }
          
          // Method/Corroutine used as example
          private IEnumerator InventoryTimeElapse()
          {
               while (true)
               {
                    // Set the time to wait until continuing the execution
                    yield return new WaitForSeconds(1f);
                    attributeManager.ExecuteTimeElapseActions(this);

                    Stat neededStat = attributeManager.GetRelatedStateToTakeCare();
                    if (neededStat != null)
                         currentObjectiveStat = neededStat;

               }
               
          }
          
          public bool SatisfiesNeed(RequiredStat stat)
          {
               return attributeManager.CanSatisfyStat(stat);
          }
     }
     
     
}
