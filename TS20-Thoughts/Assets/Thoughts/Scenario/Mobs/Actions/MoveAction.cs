using System.Collections;
using System.Collections.Generic;
using Thoughts;
using Thoughts.Mobs;
using UnityEngine;

public class MoveAction : MobAction
{
    private Vector3 destination;
    
    public MoveAction(Vector3 destination)
    {
        this.destination = destination;
    }

    public override void Execute(Mob mob)
    {
        mob.navMeshAgent.SetDestination(destination);
        mob.navMeshAgent.isStopped = false;
    }
}
