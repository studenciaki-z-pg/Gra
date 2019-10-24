using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Instatiates plants and item chests (handles logic for selecting prefabs)
/// </summary>
public class HexFeatureManager : MonoBehaviour
{
    public HexFeatureCollection[] urbanCollections, itemCollections, plantCollections, chestCollections, strengthCollections, intelligenceCollections, agilityCollections;
    Transform container;


    public void Clear()
    {
        if (container)
        {
            Destroy(container.gameObject);
        }
        container = new GameObject("Features Container").transform;
        container.SetParent(transform, false);
    }


    public void Apply() { }

    public void AddFeatures(HexCell cell, Vector3 position)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);

        AddItemFeature(cell, position, hash);

        //AddPlantOrUrbanFeature(cell, cell.Position); //middle of cell
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Vector3 center = cell.Position;
            Vector3 corner1 = center + HexMetrics.GetFirstSolidCorner(d);
            Vector3 corner2 = center + HexMetrics.GetSecondSolidCorner(d);
            AddPlantOrUrbanFeature(cell, (center + corner1 + corner2) * 1f / 3f);
        }
    }

    public void AddItemFeature(HexCell cell, Vector3 position, HexHash hash)
    {
        switch (cell.ItemLevel)
        {
            case 1:
                HexFeatureCollection chestCollection = chestCollections[0];
                Instantiating(chestCollection.Pick(hash.a), position, 360f * hash.e);
                break;
            case 2:
                HexFeatureCollection intelligenceCollection = intelligenceCollections[0];
                Instantiating(intelligenceCollection.Pick(hash.a), position, 360f * hash.e);
                break;
            case 3:
                HexFeatureCollection strengthCollection = strengthCollections[0];
                Instantiating(strengthCollection.Pick(hash.a), position, 360f * hash.e);
                break;
            case 4:
                HexFeatureCollection agilityCollection = agilityCollections[0];
                Instantiating(agilityCollection.Pick(hash.a), position, 360f * hash.e);
                break;
            case 5:
                HexFeatureCollection collection = itemCollections[0];
                Instantiating(collection.Pick(hash.a), position, 360f * hash.e);
                break;
            default:
                break;
        }
    }

    public void AddPlantOrUrbanFeature(HexCell cell, Vector3 position)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);
        Transform urbanPrefab = PickPrefab(urbanCollections, cell.UrbanLevel, hash.a, hash.d);
        Transform plantPrefab = PickPrefab(plantCollections, cell.PlantLevel, hash.b, hash.d);
        Transform winningPrefab = null;
        float usedHash = hash.a;
        if (urbanPrefab)
        {
            if (plantPrefab && hash.b < hash.a) //if both prefabs exist, choose the one with the lowest hash value
            {
                winningPrefab = plantPrefab;
                usedHash = hash.b;
            }
        }
        else if (plantPrefab)
        {
            winningPrefab = plantPrefab;
        }
        else
        {
            return;
        }

        Instantiating(winningPrefab, position, 360f * hash.e);
    }

    void Instantiating(Transform prefab, Vector3 position, float rotation)
    {
        Transform instance = Instantiate(prefab);
        position.y += instance.localScale.y * 0.5f;
        instance.localPosition = HexMetrics.Perturb(position);
        instance.localRotation = Quaternion.Euler(0f, rotation, 0f);
        instance.SetParent(container, false);
    }


    Transform PickPrefab(HexFeatureCollection[] collection, int level, float hash, float choice)
    {
        if (level > 0)
        {
            float[] thresholds = GetFeatureThresholds(level - 1);
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (hash < thresholds[i])
                {
                    return collection[i].Pick(choice);
                }
            }
        }
        return null;
    }


    /*static float[][] featureThresholds = {
        new float[] {0.0f, 0.0f, 0.4f},
        new float[] {0.0f, 0.4f, 0.6f},
        new float[] {0.4f, 0.6f, 0.8f}
    };*/
    static float[][] featureThresholds = {
        new float[] {0.0f, 0.0f, 0.2f},
        new float[] {0.0f, 0.2f, 0.3f},
        new float[] {0.2f, 0.3f, 0.4f}
    };

    static float[] GetFeatureThresholds(int level)
    {
        if (level >= featureThresholds.Length)
            level = featureThresholds.Length;
        return featureThresholds[level];
    }


}



[System.Serializable]
public struct HexFeatureCollection
{

    public Transform[] prefabs;

    public Transform Pick(float choice)
    {
        return prefabs[(int)(choice * prefabs.Length)];
    }
    public Transform Get(int choice)
    {
        return prefabs[choice];
    }
    public int Length
    {
        get
        {
            return prefabs.Length;
        }
    }

}