using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class VegetationGenerator : MonoBehaviour
    {
        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.
        /// </summary>
        [Tooltip("Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.")]
        [SerializeField] private MapGenerator mapGenerator;
        
        /// <summary>
        /// How close the vegetation can be to the sea shore.
        /// </summary>
        [Tooltip("How close the vegetation can be to the sea shore. 1 means that it can get in the sea. 0.5 means a long distance to the sea shore.")]
        [SerializeField] private float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea
        
        public void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteVegetation();
            
            float[,] noise = Noise.GenerateNoiseMap((int)mapGenerator.mapConfiguration.mapRadius*2, (int)mapGenerator.mapConfiguration.mapRadius*2, mapGenerator.mapConfiguration.vegetationNoiseSettings, Vector2.zero, mapGenerator.mapConfiguration.seed);
            float rayOriginHeight = mapGenerator.mapConfiguration.heightMapSettings.heightMultiplier * 2f;
            float rayDistance = rayOriginHeight * closenessToShore; //[0,1], 1 being that the vegetation can get on the sea
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (noise[x, y] > 0.5f)
                    {
                        Vector2 positionCheck = new Vector2(x - mapGenerator.mapConfiguration.mapRadius, y - mapGenerator.mapConfiguration.mapRadius);
                        RaycastHit hit;
                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), mapGenerator.transform.TransformDirection(Vector3.down), out hit, rayDistance*closenessToShore))
                        {
                            //Todo: be able to get more than just the first mapElement in the collection. Maybe even each one of the elements in the collection could have its own noise settings, prefab reference and treshold
                            Object.Instantiate((Object)mapGenerator.mapConfiguration.vegetationCollection.mapElements[0], hit.point, Quaternion.identity, this.transform);

                        }
                    }
                }
            }
        }
        public void DeleteVegetation()
        {
            if (Application.isPlaying)
                this.transform.DestroyAllChildren(); 
            else
                this.transform.DestroyImmediateAllChildren();
        }
    }
}
