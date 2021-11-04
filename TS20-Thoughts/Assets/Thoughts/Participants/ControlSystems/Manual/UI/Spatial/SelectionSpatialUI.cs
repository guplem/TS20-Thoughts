using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Thoughts.Game.Map.MapElements;
using UnityEngine;

public class SelectionSpatialUI : MonoBehaviour
{
    [SerializeField] private Disc selectionDisc;
    private MapElement selectedMapElement;

    public void Setup(MapElement selectedMapElement)
    {
        this.selectedMapElement = selectedMapElement;
        
        Update(); // So if it is inactive, it gets reactivated again
    }
    
    private void Update()
    {
        if (selectedMapElement == null)
        {
            selectionDisc.gameObject.SetActive(false);
        }
        else
        {
            selectionDisc.gameObject.SetActive(true);
            selectionDisc.transform.position = selectedMapElement.transform.position;
        }
    }

}
