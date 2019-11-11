using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

//TODO: Wyswietlenie komunikatu o turze na ekranie (UI/Camera)
//TODO: Ujednolicenie grafiki ekwipunku (UI/Camera)
//TODO: Zablokowanie dostepu do pionka drugiego gracza(?)
//TODO: Sprawdzenie warunkow zwyciestwa/porazki
//TODO: Usprawnić metodę losowania wydarzeń/skrzynek/przeciwników
//TODO: Losowanie przedmiotów do skrzynek

public class GameManager : MonoBehaviour
{
    [SerializeField] Character char1;
    [SerializeField] Character char2;
    [SerializeField] HexMapCamera hexMapCamera;
    [SerializeField] MapPicker mapPicker;


    //referencje
    public HexGrid hexGrid;
    public HexGameUI hexGameUI;
    public EquippableItem[] ListOfItems;
    public static GameManager instance;


    //zmienne
    private int Size;
    private int LvlCap;
    private bool EndTurnButtonPressed = false;//?

    public int activePlayer;
    public Player[] players = new Player[2];



    //struktury
    public struct Player
    {
        public Character Character;
        public HexUnit HexUnit;
        public Color color;
    }

    //funkcje
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //-> utworzenie mapy(pierwszej) -> mozna dodac by jej nie wyswietlac zanim nie skonczy sie menu!
        //Inicjalizacja mapy
        MapType initType = (MapType)Random.Range(0, Enum.GetValues(typeof(MapType)).Length);
        hexGrid.mapGenerator.SetLandscape(initType);
        hexGrid.CreateMap();

        //Inicjalizacja graczy
        players[0].Character = char1;
        players[1].Character = char2;
        ItemList itemList = new ItemList();
        ListOfItems = itemList.getItemList();

        players[0].color = Color.white;
        players[1].color = Color.black;
        

        //Pionek - ustawienia -> Ustawia sie przy kazdorazowym przejsciu mapy, stad oddzielna funkcja
        InitializePlayerUnit();

        //Rozpoczecie gry
        activePlayer = 0;
        hexMapCamera.SetCameraPosition(players[activePlayer].HexUnit.Location.Position.x, players[activePlayer].HexUnit.Location.Position.z, players[activePlayer].HexUnit.Location.Position);
    }

    public void InitializePlayerUnit()
    {
        //przypisanie pionka
        players[0].HexUnit = hexGrid.units[0];
        players[1].HexUnit = hexGrid.units[1];
        
        //przypisania poczatkowej predkosci(punkty ruchu) w oparciu o statystyki
        players[0].HexUnit.Speed = players[0].Character.SpeedValue();
        players[1].HexUnit.Speed = players[1].Character.SpeedValue();

        //ustawienie kolorow
        players[0].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[0].color);
        players[1].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[1].color);
    }

    public void NextPlayer()
    {
        activePlayer = (activePlayer + 1) % 2;
        players[activePlayer].HexUnit.Speed = players[activePlayer].Character.SpeedValue();
        hexMapCamera.SetCameraPosition(players[activePlayer].HexUnit.Location.Position.x, players[activePlayer].HexUnit.Location.Position.z, players[activePlayer].HexUnit.Location.Position);
        hexGameUI.SetSelectedUnit(players[activePlayer].HexUnit);
    }

    public void NextRound()
    {
        hexGrid.mapGenerator.SetLandscape(mapPicker.MapChoice);
        hexGrid.CreateMap();
        InitializePlayerUnit();
        //TODO: zwrócić uwagę czyja ma być kolej
        hexGameUI.gameObject.SetActive(true);
        hexGameUI.SetSelectedUnit(players[activePlayer].HexUnit);
    }

    public bool IsActiveUnit(HexUnit unit)
    {
        return players[activePlayer].HexUnit == unit;
    }

    public void OnFinish(HexUnit unit)
    {
        players[activePlayer].Character.LevelUp();
        mapPicker.ShowPicker(hexGrid.mapGenerator.GetLandscapeType()); //NextRound() is caled inside MapPicker

        hexGrid.ClearPath();
        hexGameUI.gameObject.SetActive(false);
    }

}
