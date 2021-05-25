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

        public void ShowUIFor(MapElement mapElement, OwnedAttribute attribute)
        {
            this.gameObject.SetActive(mapElement != null && attribute != null);
            if (mapElement == null || attribute == null )
                return;

            Clear();

            List<MapEvent> mapEvents = attribute.attribute.mapEvents.Cast<MapEvent>().ToList();
            for (int mapEventIndex = 0; mapEventIndex < attribute.attribute.mapEvents.Count; mapEventIndex++)
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
