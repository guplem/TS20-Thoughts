using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIMapEventsManager : UIPovRow
{
    [SerializeField] private GameObject uiMapEventPrefab;
    private List<UIMapEvent> uiMapEvents = new List<UIMapEvent>();

    public void ShowUIFor(MapElement mapElement, Attribute attribute)
    {
        this.gameObject.SetActive(mapElement != null && attribute != null);
        if (mapElement == null || attribute == null )
            return;

        Clear();

        List<MapEvent> mapEvents = attribute.mapEvents.Cast<MapEvent>().ToList();
        for (int mapEventIndex = 0; mapEventIndex < attribute.mapEvents.Count; mapEventIndex++)
        {
            UIMapEvent uiMapEvent = Instantiate(uiMapEventPrefab, GetLocationPosition(mapEventIndex),Quaternion.identity , this.transform).GetComponentRequired<UIMapEvent>();
            Transform visualizer = mapElement.transform;
            if (mapElement is Mob mobMapElement)
                visualizer = mobMapElement.povCameraPrentTransform;
            uiMapEvent.Initialize(mapEvents[mapEventIndex], visualizer);
            uiMapEvents.Add(uiMapEvent);
        }
        
    }

    public override void Clear()
    {
        base.Clear();
        uiMapEvents.Clear();
    }
}
