using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    int activeElevation;

    int activeUrbanLevel, activeFarmLevel, activePlantLevel;

    bool editMode;

    HexCell previousCell, searchFromCell, searchToCell;

    void Awake()
    {
        SetColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }



    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (editMode)
            {
                EditCell(currentCell);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell)
            {
                if (searchFromCell)
                {
                    searchFromCell.DisableHighlight();
                }
                searchFromCell = currentCell;
                searchFromCell.EnableHighlight(Color.blue);
                if (searchToCell)
                {
                    hexGrid.FindPath(searchFromCell, searchToCell);
                }
            }
            else if (searchFromCell && searchFromCell != currentCell)
            {
                searchToCell = currentCell;
                hexGrid.FindPath(searchFromCell, searchToCell);
            }
        }
    }
    /*void HandleInput() //what this function should look like (but it doesn't):
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            if (editMode)
            {
                EditCells(currentCell);
            }
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }*/
    void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.Elevation = activeElevation;
        cell.UrbanLevel = activeUrbanLevel;
        cell.FarmLevel = activeFarmLevel;
        cell.PlantLevel = activePlantLevel;
    }


    public void SetColor(int index)
    {
        activeColor = colors[index];
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }
    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }
    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }
    public void SetEditMode(bool toggle)
    {
        editMode = toggle;
    }

}