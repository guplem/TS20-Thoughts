using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float noiseScale = 0.3f;

    public bool autoRegenerate;

    [SerializeField] private TerrainGenerator terrainGenerator;

    public void GenerateMap()
    {
        terrainGenerator.GenerateTerrain(width, height, noiseScale);
    }

}
