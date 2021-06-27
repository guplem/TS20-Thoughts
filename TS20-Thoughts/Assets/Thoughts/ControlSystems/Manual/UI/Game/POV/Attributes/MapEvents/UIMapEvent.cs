using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using TMPro;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class UIMapEvent : UIPovRowElement
    {
        public MapEvent mapEvent { get; private set; }
        [SerializeField] private new TextMeshPro name;
    
        public void Initialize(MapEvent mapEvent, Transform visualizer)
        {
            Initialize(visualizer);
            this.mapEvent = mapEvent;
            UpdateVisuals();
        }
    
        private string GetText()
        {
            string ret = mapEvent.name;
            ret += '\n';
        
            if (mapEvent.requirements.Count > 0)
                ret += "Requirements:\n";
            foreach (Requirement requirement in mapEvent.requirements)
                ret += $" - {requirement.attribute}: {requirement.minValue}\n";
        
            if (mapEvent.consequences.Count > 0)
                ret += "Consequences:\n";
            foreach (Consequence consequence in mapEvent.consequences)
                ret += $" - {consequence.attribute} ({consequence.deltaValue})\n";
        
            return ret;
        }
    
        protected override void UpdateVisuals()
        {
            name.text = GetText();
        }
    }
}
