namespace Thoughts.Utils.Maths
{
    public static class FalloffGenerator // Delete?
    {
        /*public static float[,] GenerateFalloffMap(int size, float percentageOfMapWithoutMaxFalloff, float mapRadius)
    {
        Debug.Log($"Generating FalloffMap. Size: {size}, p = {percentageOfMapWithoutMaxFalloff}, mapRadius = {mapRadius}");
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // OLD METHOD => Falloff only applied to one chucnk
                //float x = i/(float)size * 2 -1;
                //float y = j/(float)size * 2 -1;
                //float value = Mathf.Min(Mathf.Abs(x), Mathf.Abs(y));
                //map[i, j] = Evaluate(value);
                
                Vector2 realCoords = new Vector2( - ((float)size / 2) + i,  - ((float)size / 2) + j);
                float distanceToCenter = Mathf.Sqrt(realCoords.x*realCoords.x + realCoords.y*realCoords.y);
                float falloffValue = ((distanceToCenter*distanceToCenter)/(mapRadius*mapRadius*percentageOfMapWithoutMaxFalloff));
                float value = Mathf.Clamp01(falloffValue);
                map[i, j] = value;
                //if (distanceToCenter < 500) Debug.Log($"distanceToCenter: {distanceToCenter}, falloffValue: {falloffValue}, realCoords: {realCoords}, FINAL value: {value}" );
            }
        }

        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;
        foreach (float f in map) {
            if (f < min) min = f;
            if (f > max) max = f;
        }
        Debug.Log($"Generated FalloffMap. Size: {size}, p = {percentageOfMapWithoutMaxFalloff}, mapRadius = {mapRadius}. Min = {min}, Max = {max}");
        
        return map;
    }
    
    static float Evaluate(float value) 
    {
        float a = 3;
        float b = 2.2f;

        // This formula defines the "shape" of the falloff
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, 1));
    }*/
    }
}
