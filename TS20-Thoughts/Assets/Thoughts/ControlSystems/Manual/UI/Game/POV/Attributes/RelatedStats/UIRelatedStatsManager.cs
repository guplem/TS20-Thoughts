using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIRelatedStatsManager : UIPovRow
{
    [SerializeField] private GameObject uiRelatedStatPrefab;
    private List<UIRelatedStat> uiRelatedStats = new List<UIRelatedStat>();
    
    public void ShowUIFor(MapElement mapElement, Attribute attribute)
    {
        
        this.gameObject.SetActive(mapElement != null && attribute != null);
        if (mapElement == null || attribute == null )
            return;

        Clear();
        
        for (int relatedStatIndex = 0; relatedStatIndex < attribute.relatedStats.Count; relatedStatIndex++)
        {
            UIRelatedStat uiRelatedStat = Instantiate(uiRelatedStatPrefab, GetLocationPosition(relatedStatIndex),Quaternion.identity , this.transform).GetComponentRequired<UIRelatedStat>();
            Transform visualizer = mapElement.transform;
            if (mapElement is Mob mobMapElement)
                visualizer = mobMapElement.povCameraPrentTransform;
            uiRelatedStat.Initialize(attribute.relatedStats[relatedStatIndex], visualizer);
            uiRelatedStats.Add(uiRelatedStat);
        }
        
    }

    public override void Clear()
    {
        base.Clear();
        uiRelatedStats.Clear();
    }
}
