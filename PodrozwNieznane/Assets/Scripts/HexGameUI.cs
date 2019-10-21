using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;


    HexCell currentCell;
    HexUnit selectedUnit;


    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
                
            }
        }
    }

    void DoPathfinding()
    {
        if(UpdateCurrentCell())
        {
            // avoiding unpassable terrains/ other units
            if(currentCell && currentCell.IsValidDestination(selectedUnit))
            {
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            }
            else
            {
                grid.ClearPath();
            }
            
        }
        
    }


    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        grid.ShowUI(!toggle);
        grid.ClearPath();
        if (toggle)
        {
            Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        }
        else
        {
            Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
        }
    }
    bool UpdateCurrentCell()
    {
        HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        grid.ClearPath();
        UpdateCurrentCell();
        if (currentCell)
        {
            //Wybrano jednostke
            if (currentCell.Unit)
            {
                if (GameManager.instance.IsItMyUnit(currentCell.Unit))
                    selectedUnit = currentCell.Unit;
                else
                {
                    Debug.Log("Sorry, that is not yours");
                }
            }
            //Wybrano cos innego
            else
            {
                Debug.Log("This is not an unit");
            }

        }
    }

    void DoMove()
    {
        if (grid.HasPath && selectedUnit.Speed > 0)
        {
            //selectedUnit.Location = currentCell;
            selectedUnit.Travel(grid.GetFixedPath(selectedUnit.Speed));
            grid.ClearPath();
        }
        else
        {
            Debug.Log("Sorry, that's unreachable");
        }
    }

    public void EndTurn()
    {
        GameManager.instance.NextPlayer();
    }
}
