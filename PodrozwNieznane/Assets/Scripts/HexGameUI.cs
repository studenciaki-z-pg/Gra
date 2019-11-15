using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    public bool lookingat = false;
    public int wait = 0;


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

                if (GameManager.instance.LogAnsWindow.isActiveAndEnabled == false && GameManager.instance.LogWindow.isActiveAndEnabled == false)
                {
                    DoPathfinding();
                }
            }

            if (selectedUnit && selectedUnit.action)
            {
                DoAction();
                HighlightPlayer(true);
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
        HexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
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
            var fixedPath = grid.GetFixedPath(selectedUnit);

            //Domyslne poruszanie sie
            if (fixedPath.Count > 1)
            {
                wait = fixedPath.Count / 2;
                selectedUnit.Travel(fixedPath);
                grid.ClearPath();

                /*
                //Sprawdzanie interakcji
                if (selectedUnit.action)
                {
                    //zatrzymalismy sie przed polem z interakcja i sprawdzamy czy mamy dosc ruchu by ja wykonac
                    if (selectedUnit.Speed >= selectedUnit.GetMoveCost(selectedUnit.Location, selectedUnit.action))
                    {
                        //Utworz sciezke do tego miejsca
                        grid.FindPath(selectedUnit.Location, selectedUnit.action, selectedUnit);

                        //pzrejdz tam i wykonaj interakcje
                        selectedUnit.Travel(grid.GetPath(selectedUnit));
                    }
                    else
                    {
                        Debug.Log("Not enough SPEED to do interaction");
                    }

                    selectedUnit.action = null;
                }*/
            }
            else Debug.Log("Not enough SPEED");
  
        }
        else
        {
            Debug.Log("Sorry, that's unreachable");
        }

        HighlightPlayer(true);
    }

    public void EndTurn()//exitState()
    {
        //zablokuj sciezkowanie i pionka
        grid.ClearPath();
        SetSelectedUnit(null);

        //zakoncz ture/zmien gracza
        GameManager.instance.NextPlayer();
    }

    IEnumerator Action(List<HexCell> path)
    {
        yield return new WaitForSeconds(wait);
        //GameManager.instance.LogAnsWindow.SendLog("Czy chcesz wykonac akcje?"); //TODO: Rodzaje akcji do wyswietlenia w komunikacie
        //while (GameManager.instance.LogAnsWindow.answer == -1) yield return null;
        //if (GameManager.instance.LogAnsWindow.answer == 1)
        //{
        //na koniec ruchu zostanie zrobiona inba
        selectedUnit.Travel(path);
        yield return new WaitForSeconds(1);
        selectedUnit.action = false;
        HighlightPlayer(true);
        wait = 0;

        //}

    }

    void DoAction()
    {
        //czy tam jest ACTION ITEM
        if (currentCell.ItemLevel != 0)
        {
            Debug.Log("TU JEST ACTION ITEM");
            grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
            var path = grid.GetPath(selectedUnit);

            //Jezeli jest blisko
            if (path.Count == 2)
            {
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
