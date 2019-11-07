using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Generates cells' elevation and terrain type (i.e. texture), according to previously set landscape
/// </summary>
public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    public HexMapFeatureGenerator featuresGenerator;
    int cellCount;

    public bool useFixedSeed;
    public int seed;

    int xMin, xMax, zMin, zMax;
    readonly int erosionTriggerThreshold = 2;

    bool invertBorder = false;

    MapType mapType;
    

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

    int[] plantLevels; //determines cell.PlantLevel depending on moisture level
    int textureOffset = 0;
    int textureLimit = 1;
    int uncrossableElevation = int.MaxValue; //maybe use a list of elevations in the future


    #region Getters

    public int[] GetPlantLevels()
    {
        return this.plantLevels;
    }

    public MapType GetLandscapeType()
    {
        return mapType;
    }

    #endregion

    public void GenerateMap(int x, int z)
    {
        cellCount = x * z;
        xMin = mapBorderX;
        xMax = x - mapBorderX;
        zMin = mapBorderZ;
        zMax = z - mapBorderZ;

        Random.State originalRandomState = Random.state;
        if (!useFixedSeed)
        {
            seed = Random.Range(0, int.MaxValue);
            seed ^= (int)System.DateTime.Now.Ticks;
            seed ^= (int)Time.unscaledTime;
            seed &= int.MaxValue;
        }
        Random.InitState(seed);


        for (int i = 0; i < cellCount; i++)
        {
            grid.GetCell(i).WaterLevel = waterLevel;
        }
        bool success = true;
        do
        {
            for (int i = 0; i < cellCount; i++)
            {
                grid.GetCell(i).Elevation = 0;
            }
            CreateLand();
            ErodeLand();
            SetTerrainType();
            success = featuresGenerator.GenerateFeatures();
        }
        while (!success);

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
        HexCell firstCell = GetRandomCell();
        HexCoordinates center = firstCell.coordinates;

        grid.BeginSearch(firstCell);

        int rise = Random.value < highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && !grid.EndOfSearch())
        {
            HexCell current = grid.GetCurrentlySearchedCell();

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
                HexCell neighbor = grid.GetNeighborToSearch(current, d);
                if (neighbor == null) continue;

                grid.PutNeighborToSearch(neighbor,
                    neighbor.coordinates.DistanceTo(center),
                    Random.value < jitterProbability ? 1 : 0);                
            }
        }
        grid.ClearSearch();
        return budget;
    }

    int SinkTerrain(int chunkSize, int budget)
    {
        HexCell firstCell = GetRandomCell();
        HexCoordinates center = firstCell.coordinates;

        grid.BeginSearch(firstCell);

        int sink = Random.value < highRiseProbability ? 2 : 1;
        int size = 0;
        while (size < chunkSize && !grid.EndOfSearch())
        {
            HexCell current = grid.GetCurrentlySearchedCell();

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
                HexCell neighbor = grid.GetNeighborToSearch(current, d);
                if (neighbor == null) continue;

                grid.PutNeighborToSearch(neighbor,
                    neighbor.coordinates.DistanceTo(center),
                    Random.value < jitterProbability ? 1 : 0);
            }
        }
        grid.ClearSearch();
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
    
    HexCell GetRandomCell()
    {
        /*One way of making borders is to limit the centres of random splats*/

        //return grid.GetCell(Random.Range(0, cellCount));
        int offsetX = 0;
        int offsetZ = 0;
        if (invertBorder)
        {
            offsetX = Random.value > 0.5f ? Random.Range(0, xMin) : Random.Range(xMax, grid.cellCountX);
            offsetZ = Random.value > 0.5f ? Random.Range(0, zMin) : Random.Range(zMax, grid.cellCountZ);
        }
        else
        {
            offsetX = Random.Range(xMin, xMax);
            offsetZ = Random.Range(zMin, zMax);
        }
        return grid.GetCell(offsetX, offsetZ);
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

    #region Attributes management

    public void SetLandscape(MapType choice)
    {
        mapType = choice;

        invertBorder = false;
        switch (choice)
        {
            case MapType.CLASSIC:
                ApplyAttributes(MapAttributes.Default()); break;
            case MapType.SWAMP:
                ApplyAttributes(MapAttributes.GetSwampy()); break;
            case MapType.SHOAL:
                ApplyAttributes(MapAttributes.GetIsland()); break;
            case MapType.MOUNTAIN:
                ApplyAttributes(MapAttributes.GetMountain()); break;
            case MapType.PLAIN:
                ApplyAttributes(MapAttributes.GetPlains()); break;
            case MapType.CANYON:
                invertBorder = true;
                ApplyAttributes(MapAttributes.GetCanyon()); break;
            default:
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

    #endregion

}

