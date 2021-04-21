using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEditor;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public abstract class UIPovRow : MonoBehaviour
{

    
    public virtual void Clear()
    {
        this.transform.DestroyAllChildren();
    }
    
    [Header("Row elements' placement")]
    [Range(0, 100)]
    [SerializeField] private int locationsPreviewAmount = 3;
    [SerializeField] private float elementsSeparation = 1.5f;
    [SerializeField] private float povDistance = 1f;
    private void OnDrawGizmosSelected()
    {
        for (int locationIndex = 0; locationIndex < locationsPreviewAmount; locationIndex++)
        {
            Gizmos.color = new Color(0.3f, 0.5f, 0.8f);
            Gizmos.DrawSphere(GetLocationPosition(locationIndex), 0.2f);
        }
    }
    protected Vector3 GetLocationPosition(int locationIndex)
    {
        bool leftSide = locationIndex != 0 && locationIndex % 2 == 0;
        float div = ((float) locationIndex) / 2f;
        int locationGridPosition = -(int)div;
        if (!leftSide)
            locationGridPosition = ((int)Math.Ceiling(div));
        Vector3 temPos = new Vector3(locationGridPosition * elementsSeparation, 0f, povDistance);
        temPos = transform.rotation * temPos;
        return transform.position + temPos;
    }
}
