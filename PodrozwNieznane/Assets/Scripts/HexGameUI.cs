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

    public bool lookingat = false;
    public float wait = 0;


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
                if (!currentCell.Unit)
                {
                    OnGUI();
                }
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

                if (GameManager.instance.LogAnsWindow.isActiveAndEnabled == false && GameManager.instance.LogWindow.isActiveAndEnabled == false && !selectedUnit.travelling)
                {
                    DoPathfinding();
                }
            }
        }
    }

    void OnGUI()
    {
        // Bail out immediately if not moused over:
        if (!lookingat) return;
        // Get the screen position of the NPC's origin:
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        // Define a 100x100 pixel rect going up and to the right:
        Rect menuRect = new Rect(screenPos.x, screenPos.y - 100, 100, 100);
        // Draw a label in the rect:
        GUI.Label(menuRect, "Menu Goes Here");



        /*HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

        if (cell.ItemLevel != 0 && cell.ItemLevel != -1)
        {
            Rect menuRect = new Rect(cell.Position.x, cell.Position.y - 10, 10, 10);
            GUI.Label(menuRect, "Menu Goes Here");
        }*/
    }


    void HighlightPlayer(bool state)
    {
        if (state)
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
        else if (GameManager.instance.IsActiveUnit(selectedUnit))
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

    void checkPopUp()
    {

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
                wait = path.Count;
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
        Debug.Log("ACTIOOOOOOOOOON");
        StartCoroutine(Action(dest));
    }

    IEnumerator Action(HexCell dest)
    {


        //czekaj az pionek sie skonczy ruszac
        yield return new WaitForSeconds(wait);

        //przygotuj sciezke
        grid.FindPath(selectedUnit.Location, dest, selectedUnit);
        selectedUnit.Travel(grid.GetPath(selectedUnit));
        
        //czekaj az sie ruszy
        yield return new WaitForSeconds(1);


        grid.ClearPath();
        HighlightPlayer(true);
        wait = 0;
    }

    public void EndTurn()//exitState()
    {
        //zablokuj sciezkowanie i pionka
        grid.ClearPath();
        SetSelectedUnit(null);

        //zakoncz ture/zmien gracza
        GameManager.instance.NextPlayer();
    }

}
