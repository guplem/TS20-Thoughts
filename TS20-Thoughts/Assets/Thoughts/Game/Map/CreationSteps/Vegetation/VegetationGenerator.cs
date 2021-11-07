namespace Thoughts.Game.Map.CreationSteps.Vegetation
{
    public class VegetationGenerator : CreationStepGenerator
    {

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed => _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        private void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                Delete();

            for (int v = 0; v < mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn.Length; v++)
            {
                mapManager.mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(
                    mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].mapElementPrefab, 
                    vegetationSeed, 
                    mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].spawningHeightRange, 
                    mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].probability, 
                    mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].density, 
                    this.transform,
                    mapManager.mapConfiguration.vegetationSettings.mapElementsToSpawn[v].noiseSettings,
                    false
                );
            }
            
            InvokeOnFinishStepGeneration();
        }
        
        
        protected override void _DeleteStep()
        {
            mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
            InvokeOnFinishStepDeletion();
        }
        
        protected override void _GenerateStep(bool clearPrevious)
        {
            GenerateVegetation(clearPrevious);
        }
    }
}
