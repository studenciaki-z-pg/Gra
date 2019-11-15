using System;
using System.Collections;
using System.Collections.Generic;
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
                //highlight players unit
                HighlightPlayer(true);

                //klikasz prawym i odpalasz chodzenie, ktore zatrzyma sie przed ewentualnym action itemem
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                //klikasz lewym by wyjsc z pokazywania sciezki
                else if (Input.GetMouseButtonDown(0))
                {
                    selectedUnit = null;
                }
                else
                {
                    DoPathfinding();
                }

                if (Input.GetMouseButtonDown(1) && grid.HasPath && grid.GetPath(selectedUnit).Count == 2)
                {
                    DoAction();
                }

            }
        }
    }


    void HighlightPlayer(bool state)
    {
        if (state)
        {
            if (GameManager.instance.IsActiveUnit(selectedUnit))
            {
                selectedUnit.Location.EnableHighlight(Color.blue);
            }

            return;
        }
        if (GameManager.instance.IsActiveUnit(selectedUnit))
        {
            selectedUnit.Location.DisableHighlight();
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
        if (selectedUnit.Speed > 0)
        {
            var path = grid.GetFixedPath(selectedUnit);
            if (path.Count > 1)
            {
                selectedUnit.Travel(path);
                grid.ClearPath();
            }
            else Debug.Log("Not enough points");

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

    IEnumerator Action(List<HexCell> path)
    {

        GameManager.instance.LogAnsWindow.SendLog("Czy chcesz wykonac akcje?"); //TODO: Rodzaje akcji do wyswietlenia w komunikacie
        while (GameManager.instance.LogAnsWindow.answer == -1) yield return null;
        if (GameManager.instance.LogAnsWindow.answer == 1)
        {
            //na koniec ruchu zostanie zrobiona inba
            selectedUnit.Travel(path);
        }
    }

    void DoAction()
    {
        List<HexCell> path = new List<HexCell>();
        path = grid.GetPath(selectedUnit);
        grid.ClearPath();
        //czy tam jest ACTION ITEM
        if (currentCell.ItemLevel != 0)
        {
            Debug.Log("TU JEST ACTION ITEM");

            //Jezeli jest blisko
            if (path.Count == 2)
            {
                //Znajdz sciezke do itema
                Debug.Log("JEST BLISKO");

                //Czy moge zrobic action?
                if (selectedUnit.Speed >= selectedUnit.GetMoveCost(selectedUnit.Location, currentCell))
                {
                    Debug.Log("ACTIOOOOOOOOOON");
                    StartCoroutine(Action(path));
                }
            }
        }
    }

    
}
