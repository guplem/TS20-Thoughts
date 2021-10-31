using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map;
using UnityEngine;
using UnityEngine.AI;

public class MapNavigationManager : MonoBehaviour
{
    private List<NavMeshSurface> generatedNavMeshSurfaces = new List<NavMeshSurface>();

    public GameObject environmentParent;
    [SerializeField] private LayerMask usedLayers;

    public NavMeshSurface SetupNewNavMeshFor(NavMeshAgent navMeshAgent, MapConfiguration mapConfiguration, bool skipIfRepeated = true)
    {
        if (navMeshAgent == null)
        {
            Debug.LogWarning("Tying to create the NavMesh surface for a null navMeshAgent", this);
            return null;
        }
        
        NavMeshSurface navMeshSurface = null;
        bool repeatedAgent = false;

        foreach (NavMeshSurface generatedNavMeshSurface in generatedNavMeshSurfaces)
        {
            if (generatedNavMeshSurface.agentTypeID == navMeshAgent.agentTypeID)
            {
                repeatedAgent = true;
                navMeshSurface = generatedNavMeshSurface;
            }
        }

        if (repeatedAgent && skipIfRepeated)
        {
            Debug.LogWarning($"Skipping the creation of the NavMeshSurface for the agent {navMeshAgent.ToString()} because it is duplicated.");
            return null;
        }
        if (repeatedAgent && !skipIfRepeated)
        {
            Debug.LogWarning($"Recreating a NavMeshSurface for a duplicated agent {navMeshAgent.ToString()}.");
        }
        else if (!repeatedAgent)
        {
            navMeshSurface = environmentParent.AddComponent<NavMeshSurface>();
        }
        
        navMeshSurface.agentTypeID = navMeshAgent.agentTypeID;
        navMeshSurface.collectObjects = CollectObjects.Volume;
        float walkableAreaHeight = mapConfiguration.terrainHeightSettings.maxHeight - mapConfiguration.seaHeightAbsolute;
        float walkableHeightUnderSea = navMeshAgent.height*0.5f; // I want the agents to be able to submerge up to half of its height
        navMeshSurface.size = new Vector3(mapConfiguration.mapRadius*2,walkableAreaHeight + walkableHeightUnderSea*2,mapConfiguration.mapRadius*2);
        navMeshSurface.center = new Vector3(0,mapConfiguration.seaHeightAbsolute + walkableAreaHeight/2,0);
        navMeshSurface.layerMask = usedLayers;
        navMeshSurface.BuildNavMesh();
        //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData); //To update the whole NavMesh at runtime
        generatedNavMeshSurfaces.Add(navMeshSurface);
        return navMeshSurface;
    }
    
    /// <summary>
    /// Delete all nav mesh data and related components
    /// </summary>
    public void RemoveAllNavMesh()
    {
        NavMesh.RemoveAllNavMeshData();
            
        foreach (NavMeshSurface navMeshSurface in generatedNavMeshSurfaces)
            if (Application.isPlaying)
                Destroy(navMeshSurface);
            else
                DestroyImmediate(navMeshSurface);
        generatedNavMeshSurfaces.Clear();
            
        if (Application.isPlaying)
            StartCoroutine(nameof(DeleteCurrentMapCheckCoroutine));
    }
    
    /// <summary>
    /// Coroutine that checks that the full deletion of the map has been successful
    /// </summary>
    private IEnumerator  DeleteCurrentMapCheckCoroutine()
    {
        if (Application.isPlaying) // Important(?). Coroutines only work in Play mode
            yield return new WaitForSecondsRealtime(3f); // To give a chance to the "Destroy" method. It is not immediate.
            
        NavMeshSurface[] remainingSurfaces = environmentParent.GetComponents<NavMeshSurface>();
        if (remainingSurfaces.Length > 0)
            Debug.LogWarning($"Not all NavMeshSurfaces from {gameObject} have been deleted. {remainingSurfaces.Length} still exist.", remainingSurfaces[0]);
    }
}
