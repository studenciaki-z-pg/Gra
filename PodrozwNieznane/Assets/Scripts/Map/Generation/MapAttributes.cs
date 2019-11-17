using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class for storing various landscapes parameters (for HexMapGenerator landscapes)
/// </summary>
public struct MapAttributes
{
    //textureOffset and textureLimit refer to the current Texture Array
    //curret texture array: blue, darkgreen, lightgreen, yellow, brown, white, 
                            //blue, yellow, lightgreen, darkgreen, darkgreen, brown, gray, white

    public float jitterProbability, highRiseProbability, sinkProbability;
    public int chunkSizeMin, chunkSizeMax, landPercentage, waterLevel,
        elevationMinimum, elevationMaximum, mapBorderX, mapBorderZ,
        erosionPercentage, textureOffset, textureLimit, uncrossableElevation;
    public int[] plantLevel;

    public MapAttributes(float a, float b, float c, int d, int e, int f, int g, int h, int i, int j, int k, int l,
        int[] m, int n, int o, int p)
    {
        jitterProbability = a;
        highRiseProbability = b;
        sinkProbability = c;
        chunkSizeMin = d;
        chunkSizeMax = e;
        landPercentage = f;
        waterLevel = g;
        elevationMinimum = h;
        elevationMaximum = i;
        mapBorderX = j;
        mapBorderZ = k;
        erosionPercentage = l;
        plantLevel = m;
        textureOffset = n;
        textureLimit = o;
        uncrossableElevation = p;
    }


    public static MapAttributes Default()
    {
        return new MapAttributes
        {
            jitterProbability = 0.25f,
            highRiseProbability = 0.25f,
            sinkProbability = 0.2f,
            chunkSizeMin = 20,
            chunkSizeMax = 30,
            landPercentage = 60,
            waterLevel = 1,
            elevationMinimum = -2,
            elevationMaximum = 6,
            mapBorderX = 2,
            mapBorderZ = 2,
            erosionPercentage = 50,
            plantLevel = new int[6] { 0, 0, 1, 2, 3, 0 },
            textureOffset = 0,
            textureLimit = 6,
            uncrossableElevation = 0
        };
    }

    public static MapAttributes GetSwampy()
    {
        return new MapAttributes(0f, 0f, 0f, 20, 20, 50, 1, -2, 8, 0, 0, 0, new int[6] { 0, 0, 0, 0, 0, 3 }, 1, 3, int.MaxValue);
    }
    public static MapAttributes GetIsland()
    {
        return new MapAttributes(0f, 0f, 0f, 20, 20, 25, 1, -2, 8, 3, 3, 0, new int[6] { 0, 0, 0, 1, 2, 0 }, 6, 4, int.MaxValue);
    }
    public static MapAttributes GetMountain()
    {
        return new MapAttributes(0.25f, 0.77f, 0f, 20, 30, 66, 5, -2, 10, 2, 2, 50, new int[6] { 0, 0, 0, 1, 2, 0 }, 7, 7, 10);
    }
    public static MapAttributes GetPlains()
    {
        return new MapAttributes(0.25f, 0.25f, 0f, 20, 30, 95, 4, -2, 6, 0, 0, 75, new int[6] { 0, 2, 1, 2, 3, 0 }, 0, 5, 1);
    }
    public static MapAttributes GetCanyon()
    {
        return new MapAttributes(0.5f, 0.5f, 0f, 20, 30, 64, 1, -2, 6, 8, 1, 25, new int[6] { 0, 0, 0, 1, 2, 0 }, 7, 6, int.MaxValue);
    }


}


