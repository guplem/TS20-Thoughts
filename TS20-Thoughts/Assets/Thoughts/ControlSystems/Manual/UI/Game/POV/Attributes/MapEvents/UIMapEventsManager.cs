using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class UIMapEventsManager : UIPovRow
    {
        [SerializeField] private GameObject uiMapEventPrefab;
        private List<UIMapEvent> uiMapEvents = new List<UIMapEvent>();

        public void ShowUIFor(MapElement mapElement, AttributeOwnership attributeOwnership)
        {
            this.gameObject.SetActive(mapElement != null && attributeOwnership != null);
            if (mapElement == null || attributeOwnership == null )
                return;

            Clear();

            List<MapEvent> mapEvents = attributeOwnership.attribute.mapEvents.Cast<MapEvent>().ToList();
            for (int mapEventIndex = 0; mapEventIndex < attributeOwnership.attribute.mapEvents.Count; mapEventIndex++)
            {
                UIMapEvent uiMapEvent = Instantiate(uiMapEventPrefab, GetLocationPosition(mapEventIndex),Quaternion.identity , this.transform).GetComponentRequired<UIMapEvent>();
                uiMapEvent.Initialize(mapEvents[mapEventIndex], mapElement.povCameraPosition);
                uiMapEvents.Add(uiMapEvent);
            }
        
        }

        public override void Clear()
        {
            base.Clear();
            uiMapEvents.Clear();
        }
    }
}
