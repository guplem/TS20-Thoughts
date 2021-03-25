using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using Thoughts.Mobs;
using Thoughts.Needs;
using UnityEngine;

public class DummyNeedType : Need
{
    public override string GetName()
    {
        return "Dummy need";
    }
    public override bool IsSatisfiedBy(MapElement executer)
    {
        Debug.LogWarning("Trying to satisfy DummyNeed");
        return false;
    }
}
