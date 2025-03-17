using UnityEngine;


//Static classes are unable to have instances. They are useful when the class doesn't need to get or 
// set its own fields. Statics have only one instance and that is the main class'
public static class TerrainGeneration
{
    public static float[,] GenerateTerrain(int width, int height, float resolution)
    {
        //Create an empty 2D array to store the height values
        float[,] terrainArray = new float[width, height];

        //Make sure the resolution is not 0 or less
        resolution = Mathf.Max(resolution, 0.001f);

        //Iterate through the array and assign each point to some noise
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                //We don't want to use the integers i and j as we want the gradual changes that come 
                //from values close to one another in perlin noise
                float x = i / resolution;
                float y = j / resolution;

                terrainArray[i, j] = Mathf.PerlinNoise(x, y);
            }
        }

        //Return the noise values
        return terrainArray;
    }
}
