using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HexGrid : MonoBehaviour
{
    public int cellCountX;
    public int cellCountZ;

    HexCell[] cells;
    HexGridChunk[] chunks;
    public List<HexUnit> units = new List<HexUnit>();
    List<InterableObject> items = new List<InterableObject>();
    HexCellShaderData cellShaderData;


    public HexMapGenerator mapGenerator;

    public int chunkCountX = 4, chunkCountZ = 3;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    //public Color defaultColor = Color.white;

    public Texture2D noiseSource;
    public HexGridChunk chunkPrefab;
    public int seed;

    public HexUnit unitPrefab;



    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        HexUnit.unitPrefab = unitPrefab;
        cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        cellShaderData.Grid = this;
        mapGenerator.SetLandscape(0);
        CreateMap();
    }


    public void CreateMap() //called every time by "refresh" button
    {
        RemoveAllUnits();
        RemoveAllItems();
        ClearPath();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateMap(chunkCountX, chunkCountZ);
        mapGenerator.GenerateMap(cellCountX, cellCountZ);
    }

    public void CreateMap(int chunkCountX, int chunkCountZ)
    {

        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                Destroy(chunks[i].gameObject);
            }
        }

        cellShaderData.Initialize(cellCountX, cellCountZ);
        CreateChunks();
        CreateCells();
    }

    void OnEnable()
    {
        if (!HexMetrics.noiseSource)
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexUnit.unitPrefab = unitPrefab;
            ResetVisibility();
        }
    }



    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }




    //After hex edit we need to refresh pyramids around



    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Index = i;

        cell.ShaderData = cellShaderData;
        cell.Explorable = x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;


        if (x > 0) {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0) {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1) {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;
        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public void Load(BinaryReader reader, int header)
    {
        //Loading a map
        ClearPath();
    }

    #region Cell getters

    //Changing Touching into getting (bitches) <3
    //I'm not longer a weirdo xD
    /*void TouchCell(Vector3 position)//Color cell if added color
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        HexCell cell = cells[index];
        //cell.color = touchedColor;
        hexMesh.Triangulate(cells);
    }*/


    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        //Debug.Log("Got you babe: " + coordinates.ToString());     ///////////////////////////////////

        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        HexCell cell = cells[index];
        return cells[index];
    }

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    public HexCell GetCell(int xOffset, int zOffset)
    {
        return cells[xOffset + zOffset * cellCountX];
    }
    public HexCell GetCell(int cellIndex)
    {
        return cells[cellIndex];
    }
    public HexCell GetRandomCell()
    {
        return cells[Random.Range(0, cells.Length)];
    }

    #endregion

    #region Items adding and removing

    public void AddItem(InterableObject item)
    {
        items.Add(item);
    }

    public void RemoveAllItems()
    {
        foreach (InterableObject item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    #endregion

    #region Units adding and removing

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.Grid = this;
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }
    void RemoveAllUnits()
    {
        foreach (HexUnit hexUnit in units)
        {
            hexUnit.Die();
        }
        units.Clear();
    }

    #endregion

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }


    #region  Dijkstra na kiju etc

    HexCell currentPathFrom, currentPathTo;
    bool currentPathExists;

    public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        //saving the path
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, unit); //checking if path exist
        ShowPath(unit?.Speed ?? HexUnit.initSpeed);//movement points
    }

    public List<HexCell> GetPath()
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();
        return path;
    }

    public List<HexCell> GetFixedPath(HexUnit unit)
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();

        var movementPoints = unit.Speed;
        var moveCost = 0;

        for (int i=0;i<path.Count-1;i++)
        {
            moveCost += unit.GetMoveCost(path[i], path[i + 1]);

            /*//specjalna sytuacja(wchdozenie na zbocze gdy ma sie 2p ruchu)
            if (((movementPoints - moveCost) == -1 && unit.GetMoveCost(path[i], path[i + 1]) == 3))
            {
                Debug.Log("cliffing: " + moveCost);
                i++;
                path.RemoveRange(i, path.Count - i);
                return path;
            }*/

            //standardowa sytuacja
            if ((movementPoints - moveCost) < 0)
            {
                path.RemoveRange(i, path.Count - i);
                return path;
            }    
        }
        return path;
    }

    //Using saved path we can visualize it
    void ShowPath(int speed)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(((current.Distance-1)/(speed)).ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }
        currentPathFrom.EnableHighlight(Color.blue);
        currentPathTo.EnableHighlight(Color.red);
    }

    //cleaning visualisation of a path
    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        //int speed = unit.Speed;
        BeginSearch(fromCell);
        while (!EndOfSearch())
        {
            HexCell current = GetCurrentlySearchedCell();

            if (current == toCell) //end of search, coming back & highlighting the path
            {
                return true;
            }

            //int currentTurn = (current.Distance)/(speed+1);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = GetNeighborToSearch(current, d);
                if (neighbor == null) continue;

                if (!neighbor.IsValidDestination(unit))
                {
                    continue;
                }

                int moveCost = unit.GetMoveCost(current, neighbor);
                if (moveCost < 0)
                {
                    continue;
                }

                int distance = current.Distance + moveCost;
                //int turn = (distance)/speed;
                //if (turn > currentTurn)
                //{
                //    distance = turn + moveCost + speed;
                //}

                int neighborHeuristics = neighbor.coordinates.DistanceTo(toCell.coordinates);
                bool success = PutNeighborToSearch(neighbor, distance, neighborHeuristics, current);
                if (success == false)
                    UpdateNeighborToSearch(neighbor, distance, current);
            }
        }
        return false;
    }

    #endregion

    #region Search Manager

    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    public void BeginSearch(HexCell fromCell)
    {
        searchFrontierPhase += 2;
        searchFrontier = searchFrontier ?? new HexCellPriorityQueue();
        searchFrontier.Clear();

        fromCell.SearchHeuristic = 0;
        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.Distance = 0;

        searchFrontier.Enqueue(fromCell);
    }
    public bool EndOfSearch()
    {
        return !(searchFrontier.Count > 0);
    }
    public HexCell GetCurrentlySearchedCell()
    {
        //yield return delay;
        HexCell current = searchFrontier.Dequeue();
        current.SearchPhase += 1;
        return current;
    }
    public HexCell GetNeighborToSearch(HexCell current, HexDirection d)
    {
        HexCell neighbor = current.GetNeighbor(d);
        if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
        {
            return null;
        }
        return neighbor;
    }
    public bool PutNeighborToSearch(HexCell neighbor, int distance, int searchHeuristic, HexCell pathFrom = null)
    {
        if (neighbor.SearchPhase < searchFrontierPhase) //check if neighbour wasn't put in the queue yet
        {
            neighbor.SearchPhase = searchFrontierPhase;

            neighbor.Distance = distance;
            neighbor.SearchHeuristic = searchHeuristic;
            neighbor.PathFrom = pathFrom ?? neighbor.PathFrom;

            searchFrontier.Enqueue(neighbor);
            return true;
        }
        return false; //was in the queue already
    }
    public void UpdateNeighborToSearch(HexCell neighbor, int distance, HexCell pathFrom = null)
    {
        if (distance < neighbor.Distance)
        {
            int oldPriority = neighbor.SearchPriority;
            neighbor.Distance = distance;
            neighbor.PathFrom = pathFrom ?? neighbor.PathFrom;

            searchFrontier.Change(neighbor, oldPriority);
        }
    }
    public void ClearSearch()
    {
        searchFrontier.Clear();
    }

    #endregion

    #region Fog implementation

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].IncreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DecreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    List<HexCell> GetVisibleCells(HexCell fromCell, int range)
    {
        HexCoordinates fromCoordinates = fromCell.coordinates;
        List<HexCell> visibleCells = ListPool<HexCell>.Get();
        range += fromCell.ViewElevation;

        BeginSearch(fromCell);
        while (!EndOfSearch())
        {
            HexCell current = GetCurrentlySearchedCell();
            visibleCells.Add(current);

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = GetNeighborToSearch(current, d);
                if (neighbor == null || !neighbor.Explorable) continue;

                int distance = current.Distance + 1;
                if (distance + neighbor.ViewElevation  > range ||
                    distance > fromCoordinates.DistanceTo(neighbor.coordinates))
                {
                    continue;
                }

                bool success = PutNeighborToSearch(neighbor, distance, 0);
                if (success == false)
                    UpdateNeighborToSearch(neighbor, distance);
            }
        }
        return visibleCells;
    }

    public void ResetVisibility()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].ResetVisibility();
        }
        for (int i = 0; i < units.Count; i++)
        {
            HexUnit unit = units[i];
            IncreaseVisibility(unit.Location, unit.VisionRange);
        }
    }

    #endregion

}

