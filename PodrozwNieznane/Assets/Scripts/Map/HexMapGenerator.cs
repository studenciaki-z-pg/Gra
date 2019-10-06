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
    readonly int erosionTriggerThreshold = 2;

    enum InterableList
    {
        ItemChest,
        StrengthTest,
        IntelligenceTest,
        AgilityTest
    }

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

    [Range(-4, 5)]
    public int elevationMinimum = -2;

    [Range(6, 10)]
    public int elevationMaximum = 8;

    [Range(0, 10)]
    public int mapBorderX = 2;

    [Range(0, 10)]
    public int mapBorderZ = 2;

    [Range(0, 100)]
    public int erosionPercentage = 50;

    int[] plantLevels;
    int textureOffset = 0, textureLimit = 1;
    int uncrossableElevation = int.MaxValue; //maybe use a list of elevations in the future

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
        ErodeLand();
        SetTerrainType();
        SetPostGenerationFeatures();
        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).SearchPhase = 0;
        }

        Random.state = originalRandomState;

        //AddMountainBorder(x, z);
        /*for (int i = 0; i < grid.cellCountX * grid.cellCountZ; i++)
        {
            HexCell hexCell = grid.GetCell(i);
            hexCell.IncreaseVisibility();
            //grid.AddUnit(Instantiate(HexUnit.unitPrefab), hexCell, Random.Range(0f, 360f));
        }
        */
    }

    void CreateLand()
    {
        int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        for (int guard = 0; landBudget > 0 && guard < 10000; guard++)
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
        if (landBudget > 0)
        {
            Debug.LogWarning("Failed to use up " + landBudget + " land budget.");
        }
    }

    void ErodeLand()
    {
        List<HexCell> erodibleCells = ListPool<HexCell>.Get();
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            if (IsErodible(cell))
            {
                erodibleCells.Add(cell);
            }
        }
        int targetErodibleCount = (int)(erodibleCells.Count * (100 - erosionPercentage) * 0.01f);
        while (erodibleCells.Count > targetErodibleCount)
        {
            int index = Random.Range(0, erodibleCells.Count);
            HexCell cell = erodibleCells[index];
            HexCell targetCell = GetErosionTarget(cell);

            cell.Elevation -= 1;
            targetCell.Elevation += 1;

            if (!IsErodible(cell))
            {
                erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];  //removing element from list
                erodibleCells.RemoveAt(erodibleCells.Count - 1);                //
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = cell.GetNeighbor(d);
                if (
                    neighbor && neighbor.Elevation == cell.Elevation + erosionTriggerThreshold &&
                    !erodibleCells.Contains(neighbor)
                )
                {
                    erodibleCells.Add(neighbor);
                }
            }
            if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
            {
                erodibleCells.Add(targetCell);
            }
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = targetCell.GetNeighbor(d);
                if (
                    neighbor && neighbor != cell && !IsErodible(neighbor) &&
                    erodibleCells.Contains(neighbor)
                )
                {
                    erodibleCells.Remove(neighbor);
                }
            }
        }

        ListPool<HexCell>.Add(erodibleCells);
    }
    bool IsErodible(HexCell cell)
    {
        int erodibleElevation = cell.Elevation - erosionTriggerThreshold;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                return true;
            }
        }
        return false;
    }
    HexCell GetErosionTarget(HexCell cell)
    {
        List<HexCell> candidates = ListPool<HexCell>.Get();
        int erodibleElevation = cell.Elevation - 2;
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            HexCell neighbor = cell.GetNeighbor(d);
            if (neighbor && neighbor.Elevation <= erodibleElevation)
            {
                candidates.Add(neighbor);
            }
        }
        HexCell target = candidates[Random.Range(0, candidates.Count)];
        ListPool<HexCell>.Add(candidates);
        return target;
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
        //higher elevation -> next color

        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            {
                int newTerrainTypeIndex = (Mathf.Abs(cell.Elevation - cell.WaterLevel) + 1);
                if (!cell.IsUnderwater)
                {
                    // case cell.Elevation == cell.WaterLevel is here
                    if (textureLimit > newTerrainTypeIndex)
                        cell.TerrainTypeIndex = textureOffset + newTerrainTypeIndex;
                    else
                        cell.TerrainTypeIndex = textureOffset + textureLimit - 1;
                }
                else //underwater
                {
                    cell.TerrainTypeIndex = textureOffset + 0;
                }
            }
            cell.Walkable = (cell.Elevation == uncrossableElevation ? false : true);

        }
    }

    void SetPostGenerationFeatures()
    {
        ApplyMoistureDrivenFeatures(plantLevels);

        int itemsAmount = 10;
        for (int i = 0; i < itemsAmount; i++)
        {
            HexCell cell;
            do
            {
                cell = grid.GetRandomCell();
            }
            while (!(cell.Explorable && cell.Walkable));
            //cell.interableObject = Instantiate<InterableObject>(cell.interableObjectPrefab);
            cell.interableObject = Instantiate<ItemChest>(cell.ItemChestPrefab);

            cell.ItemLevel = 1;
        }
    }

    void ApplyMoistureDrivenFeatures(int[] plantLevel)
    {
        List<ClimateData> climate = (new HexMapClimate()).CreateClimate(cellCount, grid, elevationMaximum);
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            float moisture = climate[i].moisture;
            if (!cell.IsUnderwater)
            {
                if (moisture < 0.05f)
                {
                    //cell.TerrainTypeIndex = 4;
                    cell.PlantLevel = plantLevel[0] % 4;
                }
                else if (moisture < 0.12f)
                {
                    //cell.TerrainTypeIndex = 3;
                    cell.PlantLevel = plantLevel[1] % 4;
                }
                else if (moisture < 0.28f)
                {
                    //cell.TerrainTypeIndex = 2;
                    cell.PlantLevel = plantLevel[2] % 4;
                }
                else if (moisture < 0.85f)
                {
                    //cell.TerrainTypeIndex = 1;
                    cell.PlantLevel = plantLevel[3] % 4;
                }
                else
                {
                    //cell.TerrainTypeIndex = 1;
                    cell.PlantLevel = plantLevel[4] % 4;
                }
            }
            else
            {
                //cell.TerrainTypeIndex = 0;
                cell.PlantLevel = plantLevel[5] % 4;
            }
        }
    }

    bool invertBorder = false;

    HexCell GetRandomCell()
    {
        /*One way of making borsers is to limit the centres of random splats*/

        //return grid.GetCell(Random.Range(0, cellCount));

        if (invertBorder)
        {
            int a = Random.value > 0.5f ? Random.Range(0, xMin) : Random.Range(xMax, grid.cellCountX);
            int b = Random.value > 0.5f ? Random.Range(0, zMin) : Random.Range(zMax, grid.cellCountZ);
            return grid.GetCell(a, b);
        }
        return grid.GetCell(Random.Range(xMin, xMax), Random.Range(zMin, zMax));
    }

    void AddMountainBorder(int x, int z)
    {
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



    //****************Attributes management******************

    public void SetLandscape(int choice)
    {
        switch (choice)
        {
            case 0:
                invertBorder = false;
                ApplyAttributes(MapAttributes.Default()); break;
            case 1:
                invertBorder = false;
                ApplyAttributes(MapAttributes.GetSwampy()); break;
            case 2:
                invertBorder = false;
                ApplyAttributes(MapAttributes.GetIsland()); break;
            case 3:
                invertBorder = false;
                ApplyAttributes(MapAttributes.GetMountain()); break;
            case 4:
                invertBorder = false;
                ApplyAttributes(MapAttributes.GetPlains()); break;
            case 5:
                invertBorder = true;
                ApplyAttributes(MapAttributes.GetCanyon()); break;
            case 6:
            default:
                invertBorder = false;
                ApplyAttributes(MapAttributes.Default()); break;
        }
    }

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
        erosionPercentage = mapAttributes.erosionPercentage;
        plantLevels = mapAttributes.plantLevel;
        textureOffset = mapAttributes.textureOffset;
        textureLimit = mapAttributes.textureLimit;
        uncrossableElevation = mapAttributes.uncrossableElevation;
    }

}

