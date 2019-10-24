using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Handles post-generation (placing items and players, determining plant level)
/// </summary>
public class HexMapFeatureGenerator: MonoBehaviour
{
    public HexMapGenerator generator;
    public HexGrid grid;

    readonly int itemsAmount = 30;
    readonly int playersAmount = 2;
    readonly int probesAmount = 5;

    List<HexCell> playersLocations = new List<HexCell>();
    List<HexCell> itemsLocations = new List<HexCell>();
    List<HexCell> largestFlatGround;


    public bool GenerateFeatures()
    {
        int flatGroundMinimumSize = (int)(grid.cellCountX * grid.cellCountZ * 0.5f);

        FindFlatGround();
        if (largestFlatGround.Count < flatGroundMinimumSize)
        {
            return false;
        }

        playersLocations = ListPool<HexCell>.Get();
        itemsLocations = ListPool<HexCell>.Get();

        DistributePlayers();
        DistributeItems();
        DistributePlantLevels(generator.GetPlantLevels());

        ListPool<HexCell>.Add(playersLocations);
        ListPool<HexCell>.Add(itemsLocations);
        ListPool<HexCell>.Add(largestFlatGround);

        return true;
    }

    private void FindFlatGround()
    {
        largestFlatGround = ListPool<HexCell>.Get();
        for (int i = 0; i < probesAmount; i++)
        {
            HexCell probe = grid.GetRandomCell();
            List<HexCell> reachableGround = GetReachableGround(probe);

            if (largestFlatGround.Count < reachableGround.Count)
                largestFlatGround = reachableGround;
        }
    }

    private bool IsReachable(HexCell fromCell, HexCell toCell)
    {
        grid.BeginSearch(fromCell);
        while (!grid.EndOfSearch())
        {
            HexCell current = grid.GetCurrentlySearchedCell();

            if (current == toCell) //end of search
            {
                return true;
            }

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = grid.GetNeighborToSearch(current, d);
                if (neighbor == null || !neighbor.Walkable || !neighbor.Explorable)
                    continue;

                HexEdgeType edgeType = current.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff)
                {
                    continue;
                }

                int distance = current.Distance + 1;

                bool success = grid.PutNeighborToSearch(neighbor, distance, 0);
                if (success == false)
                    grid.UpdateNeighborToSearch(neighbor, distance);
            }
        }
        return false;
    }

    private List<HexCell> GetReachableGround(HexCell fromCell)
    {
        List<HexCell> reachableGroundCells = ListPool<HexCell>.Get();
        grid.BeginSearch(fromCell);
        while (!grid.EndOfSearch())
        {
            HexCell current = grid.GetCurrentlySearchedCell();
            reachableGroundCells.Add(current);

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = grid.GetNeighborToSearch(current, d);
                if (neighbor == null || !neighbor.Walkable || !neighbor.Explorable)
                   continue;

                HexEdgeType edgeType = current.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff)
                {
                    continue;
                }

                int distance = current.Distance + 1;

                bool success = grid.PutNeighborToSearch(neighbor, distance, 0);
                if (success == false)
                    grid.UpdateNeighborToSearch(neighbor, distance);
            }
        }
        return reachableGroundCells;
    }


    void DistributePlayers()
    {
        for (int i = 0; i < playersAmount; i++)
        {
            HexCell homeCell;
            do
            {
                homeCell = largestFlatGround[Random.Range(0, largestFlatGround.Count)];
            }
            while (!(homeCell.Explorable && homeCell.Walkable) || itemsLocations.Contains(homeCell) || playersLocations.Contains(homeCell));

            playersLocations.Add(homeCell);

            grid.AddUnit(Instantiate(HexUnit.unitPrefab), homeCell, Random.Range(0f, 360f));
        }
    }

    void DistributeItems()
    {
        for (int i = 0; i < itemsAmount; i++)
        {
            HexCell cell;
            do
            {
                cell = largestFlatGround[Random.Range(0, largestFlatGround.Count)];
            }
            while (!(cell.Explorable && cell.Walkable) || itemsLocations.Contains(homeCell) || playersLocations.Contains(homeCell));

            switch (Random.Range(1, 6))
            {
                case 1:
                    cell.ItemLevel = 1;
                    cell.interableObject = Instantiate<ItemChest>(cell.ItemChestPrefab);
                    cell.interableObject.transform.SetParent(grid.transform);
                    grid.AddItem(cell.interableObject as ItemChest);
                    Debug.Log("1");
                    break;
                case 2:
                    cell.ItemLevel = 2;
                    cell.interableObject = Instantiate<IntelligenceTest>(cell.IntelligenceTestPrefab);
                    cell.interableObject.transform.SetParent(grid.transform);
                    grid.AddItem(cell.interableObject as IntelligenceTest);
                    Debug.Log("2");
                    break;
                case 3:
                    cell.ItemLevel = 3;
                    cell.interableObject = Instantiate<StrengthTest>(cell.StrengthTestPrefab);
                    cell.interableObject.transform.SetParent(grid.transform);
                    grid.AddItem(cell.interableObject as StrengthTest);
                    Debug.Log("3");
                    break;
                case 4:
                    cell.ItemLevel = 4;
                    cell.interableObject = Instantiate<AgilityTest>(cell.AgilityTestPrefab);
                    cell.interableObject.transform.SetParent(grid.transform);
                    grid.AddItem(cell.interableObject as AgilityTest);
                    Debug.Log("4");
                    break;
                default:
                    cell.ItemLevel = 5;
                    cell.interableObject = Instantiate<InterableObject>(cell.ItemChestPrefab); //wydażenie
                    cell.interableObject.transform.SetParent(grid.transform);
                    grid.AddItem(cell.interableObject as InterableObject);
                    Debug.Log("5");
                    break;
            }

            itemsLocations.Add(cell);
        }
    }

    /// <summary>
    /// Assigns plant levels (obtained from MapAttributes; values from 0 to 3) according to predefined moisture thresholds
    /// </summary>
    /// <param name="plantLevel"></param>
    void DistributePlantLevels(int[] plantLevel)
    {
        int cellCount = grid.cellCountX * grid.cellCountZ;
        List<ClimateData> climate = new HexMapClimate().CreateClimate(cellCount, grid, generator.elevationMaximum);
        float[] moistureThresholds = new float[] { 0.05f, 0.12f, 0.28f, 0.85f, float.MaxValue };
        // moistureThresholds.Length == plantLevel.Length - 1

        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            float cellMoisture = climate[i].moisture;
            if (!cell.IsUnderwater)
            {
                int j = 0;
                while (cellMoisture > moistureThresholds[j])
                    j++;
                cell.PlantLevel = plantLevel[j];
            }
            else
            {
                cell.PlantLevel = plantLevel[5];
            }
        }
    }



}
