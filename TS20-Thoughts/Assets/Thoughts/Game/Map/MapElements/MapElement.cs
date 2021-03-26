
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{
     public abstract class MapElement : MonoBehaviour
     {
          [SerializeField] public Inventory inventory = new Inventory();
          
          private Need currentObjectiveNeed
          {
               get => _currentObjectiveNeed;
               set
               {
                    if (_currentObjectiveNeed == value)
                         Debug.Log($" >>> Skipping update to switch the current objective need from '{_currentObjectiveNeed}' to '{value}'");
                    else
                    {
                         _currentObjectiveNeed = value;
                         Debug.Log($"Updating current objective need for '{this}' from '{value}'.");
                         //Debug.Log($"Current working need is '{currentWorkingNeed}'");
                         currentActionPath = currentObjectiveNeed.GetActionsSatisfy(this);
                         if (currentActionPath == null)
                              Debug.LogWarning($"An action path to take care of the need '{currentObjectiveNeed}' was not found.");
                         else
                         {
                              currentActionPath.DebugLog(", ", $"Found action path to cover need '{currentObjectiveNeed}': ", gameObject);
                              currentActionPath.DebugLog(", ","   |Found action path: ");
                              DoNextAction();    
                         }
                    }

               }
          }
          [CanBeNull]
          private Need _currentObjectiveNeed;
          private List<MapActionFromMapElement> currentActionPath = new List<MapActionFromMapElement>();

          private IEnumerator coroutineHolder;
          
          public NavMeshAgent navMeshAgent { get; private set; }
          
          private void Awake()
          {
               inventory.Initialize();
               
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
               if (currentActionPath == null)
               {
                    Debug.LogError($"Trying to execute the next action of '{this}' but it does not exist.");
                    return;
               }
               
               // new MoveAction(elementToCoverNeed.gameObject.transform.position)
               int indexNextAction = currentActionPath.Count-1;
               MapActionFromMapElement mapActionFromMapElement = currentActionPath.ElementAt(indexNextAction);

               Debug.Log($"Executing action '{mapActionFromMapElement.mapAction}'");
               mapActionFromMapElement.mapAction.Execute(this, indexNextAction >= 1 ? currentActionPath.ElementAt(indexNextAction-1) : null);
            
               currentActionPath.RemoveAt(0);
          }
          
          // Method/Corroutine used as example
          private IEnumerator InventoryTimeElapse()
          {
               while (true)
               {
                    // Set the time to wait until continuing the execution
                    yield return new WaitForSeconds(1f);
                    inventory.ExecuteTimeElapseActions(this);

                    Need neededNeed = inventory.GetRelatedNeedToTakeCare();
                    if (neededNeed != null)
                         currentObjectiveNeed = neededNeed;

               }
               
          }
          public bool SatisfiesNeed(RequiredNeed need)
          {
               return inventory.CanSatisfyNeed(need);
          }
     }
     
     
}
