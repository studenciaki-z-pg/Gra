using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    public bool useFixedSeed;
    public int seed;

    int cellCount;
    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;
    int xMin, xMax, zMin, zMax;

    [Range(0f, 0.5f)]
    public float jitterProbability = 0.25f;

    [Range(20, 200)]
    public int chunkSizeMin = 30;

    [Range(20, 200)]
    public int chunkSizeMax = 100;

    [Range(5, 95)]
    public int landPercentage = 50;

    [Range(1, 5)]
    public int waterLevel = 1;

    [Range(0f, 1f)]
    public float highRiseProbability = 0.25f;

    [Range(0f, 0.4f)]
    public float sinkProbability = 0.2f;

    [Range(-4, 0)]
    public int elevationMinimum = -2;

    [Range(6, 10)]
    public int elevationMaximum = 8;

    [Range(0, 10)]
    public int mapBorderX = 2;

    [Range(0, 10)]
    public int mapBorderZ = 2;


    public void GenerateMap(int x, int z)
    {
        Random.State originalRandomState = Random.state;
        if (!useFixedSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            seed ^= (int)System.DateTime.Now.Ticks;
            seed ^= (int)Time.unscaledTime;
            seed &= int.MaxValue;
        }
        Random.InitState(seed);
        cellCount = x * z;
        xMin = mapBorderX;
        xMax = x - mapBorderX;
        zMin = mapBorderZ;
        zMax = z - mapBorderZ;
        grid.CreateMap(grid.chunkCountX, grid.chunkCountZ);
        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).WaterLevel = waterLevel;
        }
        CreateLand();
        SetTerrainType();
        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).SearchPhase = 0;
        }
        Random.state = originalRandomState;

        //AddMountainBorder(x, z);
    }

    void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        while (landBudget > 0)
        {
            int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax + 1);
            if (Random.value < sinkProbability)
            {
                landBudget = SinkTerrain(chunkSize, landBudget);
            }
            else
            {
                landBudget = RaiseTerrain(chunkSize, landBudget);
            }
        }
    }

    int RaiseTerrain(int chunkSize, int budget)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell();
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.coordinates;

        int rise = Random.value < highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = originalElevation + rise;
            if (newElevation > elevationMaximum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation < waterLevel && newElevation >= waterLevel && --budget == 0)
            {
                break;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }
        searchFrontier.Clear();
        return budget;
    }

    int SinkTerrain(int chunkSize, int budget)
    {
        searchFrontierPhase += 1;
        HexCell firstCell = GetRandomCell();
        firstCell.SearchPhase = searchFrontierPhase;
        firstCell.Distance = 0;
        firstCell.SearchHeuristic = 0;
        searchFrontier.Enqueue(firstCell);
        HexCoordinates center = firstCell.coordinates;

        int sink = Random.value < highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
            int newElevation = current.Elevation - sink;
            if (newElevation < elevationMinimum)
            {
                continue;
            }
            current.Elevation = newElevation;
            if (originalElevation >= waterLevel && newElevation < waterLevel)
            {
                budget+=1;
            }
            size += 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < jitterProbability ? 1 : 0;
                    searchFrontier.Enqueue(neighbor);
                }
            }
        }
        searchFrontier.Clear();
        return budget;
    }

    void SetTerrainType()
    {
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            {
                if (!cell.IsUnderwater)
                {
                    int newTerrainTypeIndex = cell.Elevation - cell.WaterLevel +1;
                    if (HexMetrics.colors.Length > newTerrainTypeIndex)
                        cell.TerrainTypeIndex = newTerrainTypeIndex;
                    else
                        cell.TerrainTypeIndex = HexMetrics.colors.Length - 1;
                }
            }
        }
    }

    HexCell GetRandomCell()
    {
        /*One way of making borsers is to limit the centres of random splats*/
        
        //return grid.GetCell(Random.Range(0, cellCount));
        return grid.GetCell(Random.Range(xMin, xMax), Random.Range(zMin, zMax));
    }

    void AddMountainBorder(int x, int z)
    {
        /*Another way of making borders is to artificially raise some cells to the maximum*/
        int mountainBorderX = 1;//mapBorderX;
        int mountainBorderZ = 1;//mapBorderZ;
        for (int i=0; i<x; i++)
        {
            for (int j = 0; j < mountainBorderZ; j++)
            {
                grid.GetCell(i, j).Elevation = elevationMaximum;
                grid.GetCell(i, z - j - 1).Elevation = elevationMaximum;
            }
        }
        for (int i = 0; i < z; i++)
        {
            for (int j = 0; j < mountainBorderX; j++)
            {
                grid.GetCell(j, i).Elevation = elevationMaximum;
                grid.GetCell(x - j - 1, i).Elevation = elevationMaximum;
            }
        }

    }

    //****************Attributes management
    //We actually get to choose them in HexGrid.Awake()

    public static MapAttributes defaultAttributes = MapAttributes.Default();
    public static MapAttributes islanderAttributes = new MapAttributes(0f, 0f, 0f, 20, 20, 50, 1, -2, 8, 4, 4);


    public void ApplyAttributes(MapAttributes mapAttributes)
    {
        jitterProbability = mapAttributes.jitterProbability;
        highRiseProbability = mapAttributes.highRiseProbability;
        sinkProbability = mapAttributes.sinkProbability;
        chunkSizeMin = mapAttributes.chunkSizeMin;
        chunkSizeMax = mapAttributes.chunkSizeMax;
        landPercentage = mapAttributes.landPercentage;
        waterLevel = mapAttributes.waterLevel;
        elevationMinimum = mapAttributes.elevationMinimum;
        elevationMaximum = mapAttributes.elevationMaximum;
        mapBorderX = mapAttributes.mapBorderX;
        mapBorderZ = mapAttributes.mapBorderZ;
    }

}


public struct MapAttributes
{
    public float jitterProbability, highRiseProbability, sinkProbability;
    public int chunkSizeMin, chunkSizeMax, landPercentage, waterLevel,
        elevationMinimum, elevationMaximum, mapBorderX, mapBorderZ;

    public static MapAttributes Default()
    {
        MapAttributes ma = new MapAttributes();
        ma.jitterProbability = 0.25f;
        ma.highRiseProbability = 0.25f;
        ma.sinkProbability = 0.2f;
        ma.chunkSizeMin = 20;
        ma.chunkSizeMax = 30;
        ma.landPercentage = 50;
        ma.waterLevel = 1;
        ma.elevationMinimum = -2;
        ma.elevationMaximum = 8;
        ma.mapBorderX = 2;
        ma.mapBorderZ = 2;
        return ma;
    }

    public MapAttributes (float a, float b, float c, int d, int e, int f, int g, int h, int i, int j, int k)
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
    }
}
