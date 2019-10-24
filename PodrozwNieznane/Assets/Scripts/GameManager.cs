using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Random = System.Random;

//TODO: Wyswietlenie komunikatu o turze na ekranie (UI/Camera)
//TODO: Handler do przycisku konca tury
//TODO: Zmiana statystyk interface'u (UI/Camera)
//TODO: Wycentrowanie kamery na pionku (UI/Camera)
//TODO: Zablokowanie dostepu do pionka drugiego gracza(?)
//TODO: Sprawdzenie warunkow zwyciestwa/porazki
//TODO: liczenie punktów
//TODO: Zmiana aktywnego gracza i przekazanie tury  X
//TODO: Usuniecie pionkow graczy    X
//TODO: Generowanie mapy            X
//TODO: Przypisanie pionkow         X

public class GameManager : MonoBehaviour
{
    //referencje
    public HexGrid hexGrid;                 //-> utworzenie mapy(pierwszej) -> mozna dodac by jej nie wyswietlac zanim nie skonczy sie menu!
    public HexGameUI hexGameUI;
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
        //Inicjalizacja graczy
        players[0].Character = new Character();
        players[1].Character = new Character();
        players[0].color = Color.white;
        players[1].color = Color.black;

        //Statystyki - ustawienia -> Ustawia sie tylko raz podczas inicjalizacji


        //Pionek - ustawienia -> Ustawia sie przy kazdorazowym przejsciu mapy, stad oddzielna funkcja
        InitializePlayerUnit();

        //Rozpoczecie gry
        activePlayer = 0;
    }

    public void InitializePlayerUnit()
    {
        //przypisanie pionka
        players[0].HexUnit = hexGrid.units[0];
        players[1].HexUnit = hexGrid.units[1];

        //przypisania poczatkowej predkosci(punkty ruchu) w oparciu o statystyki
        players[0].HexUnit.Speed = 7;
        players[1].HexUnit.Speed = 7;

        //ustawienie kolorow
        players[0].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[0].color);
        players[1].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[1].color);
    }

    public void NextPlayer()
    {
        activePlayer = (activePlayer + 1) % 2;
        players[activePlayer].HexUnit.Speed = 7; //TODO: Uzaleznic od statystyk
        hexGameUI.SetSelectedUnit(players[activePlayer].HexUnit);
    }

    public void NextRound()
    {
        hexGrid.CreateMap();
        InitializePlayerUnit();
        //TODO: zwrócić uwagę czyja ma być kolej
        hexGameUI.SetSelectedUnit(players[activePlayer].HexUnit);
    }

    public bool IsActiveUnit(HexUnit unit)
    {
        return players[activePlayer].HexUnit == unit;
    }

}
