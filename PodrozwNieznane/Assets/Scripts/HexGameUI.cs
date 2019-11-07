using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;


    HexCell currentCell;
    HexUnit selectedUnit;


    public void SetSelectedUnit(HexUnit unit)
    {
        selectedUnit = unit;
    }


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
            if(currentCell && currentCell.IsValidDestination(selectedUnit)&&selectedUnit.Speed>0)
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
                if (GameManager.instance.IsActiveUnit(currentCell.Unit))
                    SetSelectedUnit(currentCell.Unit);
                else
                {
                    Debug.Log("Sorry, this is not yours");
                }
            }
            //Wybrano cos innego
            else
            {
                SetSelectedUnit(null);
                Debug.Log("This is not an unit");
            }

        }
    }

    void DoMove()
    {
        
        if (grid.HasPath && selectedUnit.Speed > 0 )
        {
            var path = grid.GetFixedPath(selectedUnit);
            if (path.Count > 1)
            {
                
                //selectedUnit.Location = currentCell;
                for (var i = 0; i < path.Count - 1; i++ )
                {
                    selectedUnit.Speed -= selectedUnit.GetMoveCost(path[i], path[i+1]);
                    //Debug.Log($"speed = {selectedUnit.Speed}");
                }
                selectedUnit.Travel(path);
                grid.ClearPath();
            }
            else Debug.Log("Too far");

        }
        else
        {
            Debug.Log("Sorry, that's unreachable");
        }
    }

    public void EndTurn()//exitState()
    {
        //zablokuj sciezkowanie i pionka
        grid.ClearPath();
        selectedUnit = null;

        //zakoncz ture/zmien gracza
        GameManager.instance.NextPlayer();
    }
}
