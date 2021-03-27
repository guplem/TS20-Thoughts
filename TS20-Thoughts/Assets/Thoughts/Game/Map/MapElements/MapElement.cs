
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
                    /*if (_currentObjectiveNeed == value) {
                         Debug.Log($" >>> Skipping update to switch the current objective need from '{_currentObjectiveNeed}' to '{value}'");
                         currentActionPath.DebugLog(", ","   |Current action path: ");
                    }
                    else
                    {*/
                    _currentObjectiveStat = value;
                    Debug.Log($"Updating current objective need for '{this}' to '{value}'.");
                    //Debug.Log($"Current working need is '{currentWorkingNeed}'");
                    currentActionPath = currentObjectiveStat.GetEventsToSatisfyThisStat(this);
                    if (currentActionPath == null)
                         Debug.LogWarning($"An action path to take care of the need '{currentObjectiveStat}' was not found.");
                    else
                    {
                         currentActionPath.DebugLog(", ", $"   |Found action path to cover need '{currentObjectiveStat}': ", gameObject);
                         DoNextAction();    
                    }
                    //}

               }
          }
          [CanBeNull]
          private Stat _currentObjectiveStat;
          private List<MapEventFromAttributeAtMapElement> currentActionPath = new List<MapEventFromAttributeAtMapElement>();

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
               MapEventFromAttributeAtMapElement mapEventFromAttributeAtMapElement = currentActionPath.ElementAt(indexNextAction);

               Debug.Log($"Executing action '{mapEventFromAttributeAtMapElement.mapEvent}'");
               MapEventFromAttributeAtMapElement elementOwnerOfEventFromAttribute = indexNextAction >= 1 ? currentActionPath.ElementAt(indexNextAction - 1) : null;
               mapEventFromAttributeAtMapElement.mapEvent.Execute(this, mapEventFromAttributeAtMapElement.mapElement, mapEventFromAttributeAtMapElement.attribute, elementOwnerOfEventFromAttribute);
            
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
