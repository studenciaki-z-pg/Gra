﻿using System;
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
//TODO: Ujednolicenie grafiki ekwipunku (UI/Camera)
//TODO: Wycentrowanie kamery na pionku (UI/Camera)
//TODO: Zablokowanie dostepu do pionka drugiego gracza(?)
//TODO: Sprawdzenie warunkow zwyciestwa/porazki
//TODO: Zmiana aktywnego gracza i przekazanie tury
//TODO: Usuniecie pionkow graczy
//TODO: Generowanie mapy
//TODO: Przypisanie pionkow
//TODO: Dokończenie interakcji z obiektami
//TODO: Usuwanie obiektów interaktywnych
//TODO: Usprawnić metodę losowania wydarzeń/skrzynek/przeciwników
//TODO: Losowanie przedmiotów do skrzynek

public class GameManager : MonoBehaviour
{
    [SerializeField] Character char1;
    [SerializeField] Character char2;
    [SerializeField] HexMapCamera hexMapCamera;


    //referencje
    public HexGrid hexGrid;                 //-> utworzenie mapy(pierwszej) -> mozna dodac by jej nie wyswietlac zanim nie skonczy sie menu!
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
        //players[0].Character = new Character();
        //players[1].Character = new Character();
        players[0].Character = char1;
        players[1].Character = char2;

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

        Debug.Log("POTRZEBUJE VITALITY. NA TERAZ!!!"); //TO DELETE
        //przypisania poczatkowej predkosci(punkty ruchu) w oparciu o statystyki
        players[0].HexUnit.Speed = 5;// + (int)(players[0].Character.Vitality.Value / 5); //TODO dopasować wartość statystyk by była rozsądna
        players[1].HexUnit.Speed = 5;// + (int)(players[1].Character.Vitality.Value / 5); //TODO dopasować wartość statystyk by była rozsądna

        //ustawienie kolorow
        players[0].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[0].color);
        players[1].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[1].color);
    }

    public void NextPlayer()
    {
        activePlayer = (activePlayer + 1) % 2;
        players[activePlayer].HexUnit.Speed = 5 + (int)(players[activePlayer].Character.Vitality.Value/5); //TODO dopasować wartość statystyk by była rozsądna
        Debug.Log("Twoja prędkość to: "+ players[activePlayer].HexUnit.Speed); //TO DELETE
        Debug.Log("Lokacja: " + players[activePlayer].HexUnit.Location.Position);
        hexMapCamera.setCameraPosition(players[activePlayer].HexUnit.Location.Position.x, players[activePlayer].HexUnit.Location.Position.y);
    }

    public bool IsItMyUnit(HexUnit unit)
    {
        return players[activePlayer].HexUnit == unit;
    }
}
