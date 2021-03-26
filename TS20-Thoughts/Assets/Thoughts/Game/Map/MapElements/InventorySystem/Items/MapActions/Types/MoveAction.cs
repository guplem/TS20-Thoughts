using System.Collections;
using Thoughts.Game.GameMap;
using UnityEngine;

public class MoveAction : MapAction
{
    public MoveAction() : this(Vector3.zero) { } 

    [SerializeField] private Vector3 destination;
    
    public MoveAction(Vector3 destination)
    {
        this.destination = destination;
    }

    public override void Execute(MapElement executer)
    {
        executer.navMeshAgent.SetDestination(destination);
        executer.navMeshAgent.isStopped = false;
    }

}
