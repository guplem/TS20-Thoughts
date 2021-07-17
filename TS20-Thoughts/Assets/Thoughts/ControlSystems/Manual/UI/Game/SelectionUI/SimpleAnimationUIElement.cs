using System;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimationsManager))]
public abstract class SimpleAnimationUIElement : MonoBehaviour
{
    protected SimpleAnimationsManager simpleAnimationsManager;

    private void Awake()
    {
        simpleAnimationsManager = gameObject.GetComponentRequired<SimpleAnimationsManager>();
        
        
    }

    public void Show()
    {
        Debug.Log($"Showing {gameObject.name}");
        simpleAnimationsManager.Play("Show", true);
    }

    public void Hide()
    {
        Debug.Log($"Hiding {gameObject.name}");
        simpleAnimationsManager.Play("Hide", true);
    }
    
    
}
