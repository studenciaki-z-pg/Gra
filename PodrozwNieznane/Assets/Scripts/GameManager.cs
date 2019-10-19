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

public class GameManager : MonoBehaviour
{
    /* RoadMap
        Start szablonu GameManager

        Stworzenie game managera w scenie, przypisanie mu hex grida

        //--1--// - Menu Gry

         * Zacznij Gre
         * Instrukcje
         * Opcje
         * Credits
         * Wyjscie


        //--2--//

         * Stworzenie graczy (tu moze byc dodatkowy panel ktory pozwala wybrac kolor pionka graczom i nazw)
         * * Inicjalizacja
         * * ...
         * Instancja mapy (Awake Hex Grid)
         * ...
         * * Create Map
         * * Generacja obiektów
         * ...
         * Przeslanie do game managera listy pionkow ()
         * Operacje na pionkach (nadanie koloru, nazwy, kamery)

    */

    //#Kod

    //Zmienne globalne rozgrywki:

    //Wielkosc mapy
    //Poziom koncowy
    //...

    //Aktywny gracz, pasywny gracz i ich kolorki

    private int Size;
    private int LvlCap;
    private Players[] players = new Players[2];

    public HexGrid hexGrid;
    public int activePlayer = 0;

    public struct Players
    {
        public Character Character;
        public HexUnit HexUnit;
        public Color color;
    }

    public static GameManager instance;




    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Zycie na ryzyku zawsze jest przyjemne ~~ Daniel Milanowski 19.10.19r.
    /// </summary>
    private void Start()
    {

        //Menu

        //Start - wybor kolorow gracza
        players[0].color = Color.white;
        players[1].color = Color.black;

        //Stworzenie graczy
        players[0].Character = new Character();
        players[1].Character = new Character();

        //Inicjalizacja mapy
        hexGrid.CreateMap();

        //Przypisanie pionkow
        players[0].HexUnit = hexGrid.units[0];
        players[1].HexUnit = hexGrid.units[1];

        //Przypisanie kolorow
        players[0].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color",players[0].color);
        players[1].HexUnit.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", players[1].color);

        //Rozpoczecie gry
        activePlayer = UnityEngine.Random.Range(0, 1);
        Turn();

    }


    void Turn()
    {

        //TODO: Wyswietlenie komunikatu o turze na ekranie (UI/Camera)


        EnterState();

        //Tutaj w Update GM wychwytuje zdarzenia(interakcje i handluje je dopóki nie zostanie wciśniety przycisk koniec tury)
        //TODO: Handler do przycisku konca tury

        ExitState();

    }

    void EnterState()
    {
        //TODO: zmiana statystyk interface'u (UI/Camera)


        //TODO: Wycentrowanie kamery na pionku (UI/Camera)


        //TODO: zablokowanie dostepu do pionka drugiego gracza(?)


        //TODO:Refresh punktow ruchu (HexUnit)

    }

    private void Update()
    {
        //TODO: Update: Miejsce na dzialania gracza, dopoki nie kliknie przycisku koniec tury (Update)


    }

    void ExitState()
    {

        //TODO: Sprawdzenie warunkow zwyciestwa/porazki


        //Zmiana aktywnego gracza i przekazanie tury
        activePlayer = (activePlayer + 1) % 2;
        //Turn();
    }

    void MapRefresh()
    {
        //Od nowej mapy zaczyna gracz, ktory nie dotarl do obeliska
        activePlayer = (activePlayer + 1) % 2;

        //TODO: Usuniecie pionkow graczy
        //TODO: Generowanie mapy
        //TODO: Przypisanie pionkow

        Turn();
    }
}
