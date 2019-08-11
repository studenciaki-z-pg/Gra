using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles post-generation (placing items and players, determining plant level)
/// </summary>
public class HexMapFeatureGenerator: MonoBehaviour
{
    public HexMapGenerator generator;
    public HexGrid grid;

    int itemsAmount = 10;
    int playersAmount = 2;


    public void GenerateFeatures()
    {
        DistributePlayers();
        DistributeItems();
        DistributePlantLevels(generator.GetPlantLevels());
    }


    void DistributePlayers()
    {
        for (int i = 0; i < 2; i++)
        {
            HexCell homeCell;
            do
            {
                homeCell = grid.GetRandomCell();
            }
            while (!(homeCell.Explorable && homeCell.Walkable));
            homeCell.ItemLevel = 0;
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
                cell = grid.GetRandomCell();
            }
            while (!(cell.Explorable && cell.Walkable));

            cell.ItemLevel = 1;

            //cell.interableObject = Instantiate<InterableObject>(cell.interableObjectPrefab);
            cell.interableObject = Instantiate<ItemChest>(cell.ItemChestPrefab);
            cell.interableObject.transform.SetParent(grid.transform);
            grid.AddItem(cell.interableObject as ItemChest);
        }
    }

    void DistributePlantLevels(int[] plantLevel)
    {
        int cellCount = grid.cellCountX * grid.cellCountZ;
        List<ClimateData> climate = new HexMapClimate().CreateClimate(cellCount, grid, generator.elevationMaximum);
        float[] moistureThresholds = new float[] { 0.05f, 0.12f, 0.28f, 0.85f, float.MaxValue };
        for (int i = 0; i < cellCount; i++)
        {
            HexCell cell = grid.GetCell(i);
            float moisture = climate[i].moisture;
            if (!cell.IsUnderwater)
            {
                int j = 0;
                while (moisture > moistureThresholds[j])
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
