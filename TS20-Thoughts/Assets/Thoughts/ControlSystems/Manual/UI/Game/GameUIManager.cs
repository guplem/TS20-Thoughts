using Thoughts.Game.GameMap;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private UIPovManager uiPovManager;

    public void DisplayUIFor(MapElement mapElement)
    {
        Debug.Log($"DisplayUIFor '{mapElement}'");
        Mob mobMapElement = mapElement as Mob;
        if (mobMapElement != null)
            uiPovManager.ShowUIFor(mobMapElement);
    }
    
}
