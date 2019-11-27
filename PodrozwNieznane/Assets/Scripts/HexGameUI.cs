using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;


    private GameObject[] pauseObjects, finishObjects, menuObjects, swapObjects;
    HexCell currentCell;
    HexUnit selectedUnit;

    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        finishObjects = GameObject.FindGameObjectsWithTag("ShowOnFinish");
        swapObjects = GameObject.FindGameObjectsWithTag("ShowOnSwap");
        hidePaused();
        hideFinished();
        hideSwapped();
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

                if (!GameManager.instance.Travelling && !GameManager.instance.Interacting)
                {
                    DoPathfinding();
                }
            }
        }

        //uses the Esc button to pause and unpause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseControl();
        }
    }

    //managing highlight on cell with unit
    void HighlightPlayer(bool state)
    {
        //enable cyan highlight
        if (state)
        {
            GameManager.instance.players[GameManager.instance.activePlayer].HexUnit.Location.EnableHighlight(Color.cyan);
            return;
        }
        //disable highlight
        GameManager.instance.players[GameManager.instance.activePlayer].HexUnit.Location.DisableHighlight();
    }

    //managing whole cell highlight
    public void Highlighting(bool state)
    {
        //enable
        if (state)
        {
            HighlightPlayer(true);
            return;

        }

        //disable
        GameManager.instance.hexGrid.ClearPath();
        HighlightPlayer(false);
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

    public void SetSelectedUnit(HexUnit unit)
    {
        selectedUnit = unit;
        HighlightPlayer(selectedUnit);
    }

    void DoMove()
    {
        if (selectedUnit.Speed > 0)
        {
            //Pobieranie sciezki:
            var path = grid.GetFixedPath(selectedUnit);
            HexCell last = null;

            if (path != null && path.Count > 1)
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

        showSwapped();

        //zakoncz ture/zmien gracza
        GameManager.instance.NextPlayer();
    }

    #region Menu

    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //shows objects with ShowOnFinish tag
    public IEnumerator showFinished()
    {
        yield return new WaitUntil(() => !GameManager.instance.Travelling && !GameManager.instance.Interacting);


        foreach (GameObject g in finishObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnFinish tag
    public void hideFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(false);
        }
    }

    //hides objects with ShowOnSwap tag
    public void hideSwapped()
    {
        foreach (GameObject g in swapObjects)
        {
            g.SetActive(false);
        }
    }

    //shows objects with ShowOnSwap tag
    public void showSwapped()
    {
        foreach (GameObject g in swapObjects)
        {
            g.SetActive(true);
        }
    }

    //loads inputted level
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

}
