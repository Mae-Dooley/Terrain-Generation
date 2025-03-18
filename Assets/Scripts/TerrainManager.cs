using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [Space(10)]
    [Header("Mesh Variables")]
    [Space(5)]
    public int meshWidth;
    public int meshDepth;

    [Space(10)]
    [Header("Terrain Variables")]
    [Space(5)]
    public float verticalScale;
    public float resolution;
    public int numberOfOctaves;
    public float lacunarity;
    [Range(0, 1)]
    public float persistance;

    private float[,] noiseHeights;

    void OnValidate()
    {
        if (meshWidth < 1) meshWidth = 1;
        if (meshDepth < 1) meshDepth = 1;
        
        if (lacunarity < 1) lacunarity = 1;
        if (numberOfOctaves < 0) numberOfOctaves = 0;
    }

    void Start()
    {
        //Create the noise map
        noiseHeights = TerrainGeneration.GenerateHeightMap(meshWidth, meshDepth, resolution, 
                                                                    numberOfOctaves, lacunarity, persistance);

        GetComponent<MeshFilter>().mesh = TerrainGeneration.GenerateMesh(noiseHeights, verticalScale);
    }
}
