using TMPro;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIAttribute : UIPovRowElement
{
    public OwnedAttribute attribute { get; private set; }
    [SerializeField] private new TextMeshPro name;
    
    public void Initialize(OwnedAttribute attribute, Transform visualizer)
    {
        Initialize(visualizer);
        this.attribute = attribute;
        UpdateVisuals();
    }
    
    
    private string GetText()
    {
        string ret = attribute.attribute.name;
        ret += '\n';

        ret += $"{attribute.value}";
        
        return ret;
    }
    protected override void UpdateVisuals()
    {
        name.text = GetText();
    }
}