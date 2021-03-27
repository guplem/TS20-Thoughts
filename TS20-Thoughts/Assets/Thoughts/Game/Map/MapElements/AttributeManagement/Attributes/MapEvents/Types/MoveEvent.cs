using System.Collections;
using Thoughts;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MoveEvent : MapEvent
{
    public override void Execute(MapElement executer, MapElement elementOwnerOfEvent, Attribute attributeOwnerOfEvent, MapEventFromAttributeAtMapElement nextEventFromAttributeAtMapElement)
    {
        if (nextEventFromAttributeAtMapElement == null)
        {
            Debug.LogWarning($"Trying to move {executer} to the location of the next action (which does not exist).");
            return;
        }
        
        Vector3 movePosition = nextEventFromAttributeAtMapElement.mapElement.transform.position;
        //Debug.Log($"Executing MoveAction at {executer} to go to {movePosition}");
        executer.navMeshAgent.SetDestination(movePosition);
        executer.navMeshAgent.isStopped = false;
    }
}
