using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    readonly int columns = 20;
    readonly int rows = 40;
    // Start is called before the first frame update
    void Start()
    {

        Player.GetComponent<Unit>().tileX = (int)Player.transform.position.x;
        Player.GetComponent<Unit>().tileY = (int)Player.transform.position.y;
        Player.GetComponent<Unit>().map = this;
        GenerateMap();
        Pathfinder();

    }

    public GameObject HexPrefab;
    public GameObject Player;

   public Material[] HexMaterials;
    

    public Hex[,] hexes;
    Node[,] graph;
    

    private void GenerateMap()
    {
        hexes = new Hex[columns, rows];

        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                Hex h = new Hex(this, column, row);

                hexes[column, row] = h;
                GameObject hexObject = (GameObject)
                Instantiate(
                    HexPrefab,
                    h.Position(),
                    Quaternion.identity,
                    this.transform
                    );
                hexObject.name = "Hex_" + column + "_" + row;
                MeshRenderer mr = hexObject.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[UnityEngine.Random.Range(0,HexMaterials.Length)];
               hexObject.GetComponent<HexBeh>().x = column;
               hexObject.GetComponent<HexBeh>().y = row;
            }
        }
        //gameObject.isStatic = false;
        //StaticBatchingUtility.Combine(this.gameObject);
    }

    public Hex[] GetHexesInRange(Hex center, int range)
    {
        List<Hex> results = new List<Hex>();
        for (int x = -range; x < range; x++)
        {
            for (int y = Mathf.Max(-range,-x-range); y < Mathf.Min(range, -x+range); y++)
            {
                results.Add(GetHexAt(center.Q + x, center.R + y));
            }

        }
        return results.ToArray();
    }

    public void MoveTo (int x, int y)
    {
        //Player.transform.position = new Vector3(x,0,y);
    }

    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
            Debug.LogError("Hexes array not yet instantiated.");
            return null;
        }

        try
        {
            return hexes[x, y];
        }
        catch
        {
            Debug.LogError("GetHexAt: " + x + "," + y);
            return null;
        }
    }
    
   float TileDebuff(int x, int y)
    {
        float debuff = 0;
        Material mat = HexPrefab.GetComponent<Material>();
        if (mat.name == "Swamp")
        {
            debuff = 0.5f;
        }
        else if (mat.name == "Mountain")debuff = 10;
        return debuff+1;
    }

    public Vector3 TileToCoord(int x, int y)
    {
        return new Vector3(x,0,y);
    }
   
    public void Pathfinder()
    {
        graph = new Node[columns,rows];
        for (int column = 0; column < columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                graph[column, row] = new Node
                {
                    x = column,
                    y = row
                }; 
            }
        }
        for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                   
                    //6 directions
                    //      NW/ \NE
                    //      W|   |E
                    //      SW\ /SE

                    //NW
                    if (column > 0 && row < rows-1)
                    graph[column,row].neighbours.Add(graph[column - 1, row + 1]);
                    //NE
                    if (row < rows-1)
                        graph[column, row].neighbours.Add(graph[column, row +1]);
                    //E
                    if (column < columns-1)
                        graph[column, row].neighbours.Add(graph[column + 1, row]);
                    //SE
                    if (row > 0 && column < columns-1)
                        graph[column, row].neighbours.Add(graph[column + 1, row - 1]);
                    //SW
                    if (row > 0)
                        graph[column, row].neighbours.Add(graph[column, row - 1]);
                    //W
                    if (column > 0)
                        graph[column, row].neighbours.Add(graph[column - 1, row]);

            }
        }
    }
    public void PathMaker (int x, int y)
    {
        //DIJKSRA NA KIJU
        Player.GetComponent<Unit>().currentPath = null;

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        List<Node> unvisited = new List<Node>();

        Node source = graph[
            Player.GetComponent<Unit>().tileX,
           Player.GetComponent<Unit>().tileY
            ];
        Node target = graph[x, y];
        dist[source] = 0;
        prev[source] = null;

        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }
        while (unvisited.Count > 0)
        {
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target) break;
            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                float alt = dist[u] + u.DistanceTo(v);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
        if (prev[target] == null)
        {
            //No route
            return;
        }

        List<Node> currentPath = new List<Node>();
        Node curr = target;
        while (prev[curr] != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        currentPath.Reverse();
        Player.GetComponent<Unit>().currentPath = currentPath;
    }
}
