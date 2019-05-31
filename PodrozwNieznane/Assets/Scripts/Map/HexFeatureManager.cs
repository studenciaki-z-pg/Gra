using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFeatureManager : MonoBehaviour
{
    public HexFeatureCollection[] urbanCollections, itemCollections, plantCollections;
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

    public void AddFeatures(HexCell cell, Vector3 position, HexGrid grid)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);

        switch (cell.ItemLevel)
        {
            case 1:
                HexFeatureCollection c = itemCollections[0];
                int index = Random.Range(0, c.Length);
                cell.ItemType = index;
                Instantiating(c.Get(index), position, 360f * hash.e);
                grid.AddItem(index);
                break;
            case 2:
            case 3:
                //not supported yet
                //either  c = itemCollections[1]  or  index = (different random)
                break;
            case 0:
            default:
                break;
        }

        //ItemLevel ^
        //PlantLevel and Urban Level - work as always:

        //-->middle of cell:
        //let's leave that empty in case there would be a item.
        //AddFeature(cell, cell.Position);

        //-->around the cell:
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Vector3 center = cell.Position;
            EdgeVertices e = new EdgeVertices(
                center + HexMetrics.GetFirstSolidCorner(d), center + HexMetrics.GetSecondSolidCorner(d));
            AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
        }
    }

    public void AddFeature(HexCell cell, Vector3 position)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);
        Transform prefab = PickPrefab(urbanCollections, cell.UrbanLevel, hash.a, hash.d);
        Transform otherPrefab = PickPrefab(plantCollections, cell.PlantLevel, hash.b, hash.d);
        float usedHash = hash.a;
        if (prefab)
        {
            if (otherPrefab && hash.b < hash.a) //if both prefab and otherPrefab exists, choose the one with the lowest hash value
            {
                prefab = otherPrefab;
                usedHash = hash.b;
            }
        }
        else if (otherPrefab)
        {
            prefab = otherPrefab;
        }

        /*otherPrefab = PickPrefab(itemCollections, cell.ItemLevel, hash.c, hash.d);
        if (prefab)
        {
            if (otherPrefab && hash.c < usedHash)
            {
                prefab = otherPrefab;
            }
        }
        else if (otherPrefab)
        {
            prefab = otherPrefab;
        }*/
        else
        {
            return;
        }

        Instantiating(prefab, position, 360f * hash.e);
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
            float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
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