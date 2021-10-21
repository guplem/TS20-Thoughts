using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapElementsCollection", menuName = "Thoughts/Map/MapElement/Collection", order = 100)]
public class MapElementsCollection : ScriptableObject
{
    [SerializeField] public GameObject[] mapElements;
}
