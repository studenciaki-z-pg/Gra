using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
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
                //klikasz prawym i odpalasz chodzenie, ktore zatrzyma sie przed ewentualnym action itemem
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                //klikasz lewym by wyjsc z pokazywania sciezki
                else if (Input.GetMouseButtonDown(0))
                {
                    SetSelectedUnit(null);
                }

                if (GameManager.instance.LogAnsWindow.isActiveAndEnabled == false
                    && GameManager.instance.LogWindow.isActiveAndEnabled == false
                    && !selectedUnit.Travelling)
                {
                    DoPathfinding();
                }
            }
        }
    }

    public void HighlightPlayer(bool state)
    {
        if (state && !selectedUnit.Travelling)
        {
            if (GameManager.instance.IsActiveUnit(selectedUnit))
            {
                if (GameManager.instance.LogAnsWindow.isActiveAndEnabled == false &&
                    GameManager.instance.LogWindow.isActiveAndEnabled == false)
                {
                    selectedUnit.Location.EnableHighlight(Color.blue);
                }
            }
        }
        else if (GameManager.instance.IsActiveUnit(selectedUnit) && selectedUnit.Travelling)
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
                HighlightPlayer(true);
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
        if (grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition)))
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
            //Pobieranie sciezki:
            var path = grid.GetFixedPath(selectedUnit);
            HexCell last = null;

            if (path.Count > 1)
            {
                //Chcesz isc do action itema
                if (path[path.Count - 1].ItemLevel != 0)
                {
                    //szczegolny przypadek gdy odleglosc do action itema wynosi 1
                    if (path.Count == 2)
                    {
                        DoAction(path[1]);
                        grid.ClearPath();
                        HighlightPlayer(true);
                        return;
                    }

                    //norm przypadek z action itemem
                    last = path[path.Count - 1];
                    path.Remove(last);
                }

                //Domyslne poruszanie sie bez action itema
                selectedUnit.Travel(path);

            }
            else Debug.Log("Path too short");

            //Ruch konczy sie na action itemie jezeli choc jeden byl w sciezce
            if(last)DoAction(last);
        }
        else
        {
            Debug.Log("Not enough SPEED");
        }

        grid.ClearPath();
        HighlightPlayer(true);
    }

    void DoAction(HexCell dest)
    {
        StartCoroutine(selectedUnit.Action(dest));
    }

    public void EndTurn()//exitState()
    {
        //zablokuj sciezkowanie i pionka
        grid.ClearPath();
        SetSelectedUnit(null);

        //zakoncz ture/zmien gracza
        GameManager.instance.NextPlayer();
    }

    void DoRetreat()
    {

    }

}
