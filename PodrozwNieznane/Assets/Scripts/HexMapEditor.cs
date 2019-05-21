using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    //public HexUnit unitPrefab;

    private Color activeColor;

    int activeElevation;

    int activeUrbanLevel, activeFarmLevel, activePlantLevel;

   // bool editMode;

    HexCell previousCell;

    int brushSize = 0;

    HexCell location;


    void Awake()
    {
        SetColor(0);
        SetEditMode(false);
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyUnit();
                }
                else
                {
                    CreateUnit();
                }
                return;
            }
        }
        previousCell = null;
    }

    void HandleInput()
    {
        var speed = 24; //We do not have Units with diffrent movement at the moment
        HexCell currentCell = GetCellUnderCursor();
        if (currentCell)
        {

            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        
        }
    }

    HexCell GetCellUnderCursor()
    {
        return
            hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }
    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            hexGrid.AddUnit(
                Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f)
            );
        }
    }
    void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			hexGrid.RemoveUnit(cell.Unit);
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
    void EditCell(HexCell cell) //we have a copy of this function in HexCell, please keep it up to date (EditItself())
    {
        if (cell)
        {
            cell.Color = activeColor;
            cell.Elevation = activeElevation;
            cell.UrbanLevel = activeUrbanLevel;
            cell.FarmLevel = activeFarmLevel;
            cell.PlantLevel = activePlantLevel;
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;
        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
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
        enabled = toggle;
        //grid.ShowUI(!toggle);
    }
    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

}