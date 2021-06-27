using Thoughts.Game.Attributes;
using TMPro;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class UIAttribute : UIPovRowElement
    {
        public AttributeOwnership attributeOwnership { get; private set; }
        [SerializeField] private new TextMeshPro name;
    
        public void Initialize(AttributeOwnership attributeOwnership, Transform visualizer)
        {
            Initialize(visualizer);
            this.attributeOwnership = attributeOwnership;
            UpdateVisuals();
        }
    
    
        private string GetText()
        {
            string ret = attributeOwnership.attribute.name;
            ret += '\n';

            ret += $"{attributeOwnership.value}";
        
            return ret;
        }
        protected override void UpdateVisuals()
        {
            name.text = GetText();
        }
    }
}
