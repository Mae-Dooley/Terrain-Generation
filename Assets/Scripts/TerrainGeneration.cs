using UnityEngine;

//Static classes are unable to have instances. They are useful when the class doesn't need to get or 
// set its own fields. Statics have only one instance and that is the main class'
public static class TerrainGeneration
{
    public static float[,] GenerateHeightMap(int width, int depth, float resolution, 
                                                int numOctaves, float lacunarity, float persistance)
    {
        //Create an empty 2D array to store the height values
        float[,] terrainHeightArray = new float[width, depth];

        //Make sure the resolution is not 0 or less
        resolution = Mathf.Max(resolution, 0.001f);

        //Keep track of the "highest" and "lowest" point
        float maxNoiseVal = float.MinValue;
        float minNoiseVal = float.MaxValue;
        //(max is set to min value so the fisrt value checked is definitely larger than it and similar for min)

        //Iterate through the array and assign each point to some noise
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                /* 
                Each octave (layer of noise) will have a fequency and amplitude. We start these values at 
                1 and then each octave we multiply by the lacunarity and persistance respectively. This will 
                make each layer of noise have higher frequency (more rapid changes) but lower influence on 
                the total noise. This is provided that  lacunarity > 1, 0 < persistance < 1. 
                */
                float frequency = 1f;
                float amplitude = 1f;

                //We will accumulate the noise from each octave in this variable
                float totalNoise = 0;

                //We want each octave to sample from a differnt location
                float octaveOffset = 0;

                for (int octave = 0; octave < numOctaves; octave++) {
                    //We don't want to use the integers i and j as we want the gradual changes that come 
                    //from values close to one another in perlin noise.
                    //Frequency controls how far apart our sampling points are (higher means more rapid change)
                    float x = (i / resolution) * frequency + octaveOffset*width;
                    float y = (j / resolution) * frequency + octaveOffset*depth;

                    //We want the noise to be between -1 and 1 to add OR subtract from the height
                    totalNoise += (Mathf.PerlinNoise(x, y) * 2 - 1) * amplitude;

                    //Increase the frquency and amplitude for the next octave
                    frequency *= lacunarity;
                    amplitude *= persistance;

                    //Increase octave offset for the next sanpling layer
                    octaveOffset++;
                }

                //Check if this is the highest or lowest point yet
                if (totalNoise > maxNoiseVal) {
                    maxNoiseVal = totalNoise;
                } else if (totalNoise < minNoiseVal) {
                    minNoiseVal = totalNoise;
                }

                //Assign the noise to the array
                terrainHeightArray[i, j] = totalNoise;
            }
        }

        //We now want to normalize the array to have a traditional noise result from 0 to 1;
        float terrainHeightRange = maxNoiseVal - minNoiseVal;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < depth; j++) {
                //Shift the values to a range 0 <-> range then divide by range
                terrainHeightArray[i, j] = (terrainHeightArray[i, j] - minNoiseVal) / terrainHeightRange;
            }
        }
        
        //Return the noise values
        return terrainHeightArray;
    }
    
    public static Mesh GenerateMesh(float[,] terrainHeights)
    {
        int terrainWidth = terrainHeights.GetLength(0);
        int terrainDepth = terrainHeights.GetLength(1);

        TerrainMeshData meshData = new TerrainMeshData(terrainWidth, terrainDepth);

        //Assign the coordinates of all the vertices
        for (int i = 0; i < meshData.vertices.Length; i++) {
            //Convert linear i to row and column based on the terrain grid
            int row = i / terrainWidth;
            int column = i % terrainWidth;

            meshData.vertices[i] = new Vector3(column, terrainHeights[column, row], row);
        }

        //Assign all the triangle indices
        // The triangles fit in the inner space of the grid and so we ignore the final row and column
        for (int i = 0; i < terrainDepth - 1; i++) {
            for (int j = 0; j < terrainWidth - 1; j++) {
                //We go to each vertex and create the two triangles that span to its lower right
                int topLeftIndex = i * terrainWidth + j;

                //The triangle in the top right of the cell inner space
                meshData.AddTriangleToMesh(topLeftIndex, topLeftIndex + terrainWidth + 1, topLeftIndex + 1);

                //The triangle in the bottom left of the cell inner space
                meshData.AddTriangleToMesh(topLeftIndex, topLeftIndex + terrainWidth, 
                                                    topLeftIndex + terrainWidth + 1);
            }
        }

        //Create the mesh in unity
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        //UVS?
        mesh.triangles = meshData.triangles;

        return mesh;
    }
}
