using Thoughts.Game.GameMap;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] public UIAttributesManager attributesManager;
    
    public void DisplayUIFor(MapElement mapElement)
    {
        Debug.Log($"Displaying UI for '{mapElement}'");
        attributesManager.ShowUIFor(mapElement);
    }
    
}
