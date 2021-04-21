using Thoughts.Game.GameMap;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private UIPovManager uiPovManager;

    public void DisplayUIFor(MapElement mapElement)
    {
        Debug.Log($"DisplayUIFor '{mapElement}'");
        Mob mobMapElement = mapElement as Mob;
        uiPovManager.ShowUIFor(mobMapElement);
    }
    
}
