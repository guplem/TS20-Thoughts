using System;
using Thoughts.Game.GameMap;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    //[SerializeField] private UIPovManager uiPovManager;

    public void DisplayUIFor(MapElement mapElement)
    {
        Debug.Log($"DisplayUIFor '{mapElement}'");
        Mob mobMapElement = mapElement as Mob;
        throw new NotImplementedException();
        //Todo: remove next comment and the comment disabling the variable at the top of the class
        //uiPovManager.ShowUIFor(mobMapElement);
    }
    
}
