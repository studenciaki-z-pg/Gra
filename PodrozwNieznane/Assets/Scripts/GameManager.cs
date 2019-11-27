using System.Collections;
using Assets.Scripts;
using UnityEngine;
using Color = UnityEngine.Color;

//TODO: Prawdopodobieństwo wylosowania obiektu zależnie od rodzaju mapy
//TODO: Tekst na zakończenie rozgrywki + podsumowanie w oknie
//TODO: Pauza pomiędzy graczami by dać czas na zmianę gracza!!!!!!!!

public class GameManager : MonoBehaviour
{
    [SerializeField] Character char1;
    [SerializeField] Character char2;
    [SerializeField] HexMapCamera hexMapCamera;
    [SerializeField] MapPicker mapPicker;
    [SerializeField] LogWindow logWindow;
    [SerializeField] LogAnsWindow logAnsWindow;
    [SerializeField] GameObject[] EndOfTurnButton;


    //referencje
    public HexGrid hexGrid;
    public HexGameUI hexGameUI;
    public EquippableItem[] ListOfItems;
    public static GameManager instance;
    public LogWindow LogWindow;
    public LogAnsWindow LogAnsWindow;


    //zmienne
    public int activePlayer;
    public int winningLevel { get; set; } = 10;
    public Player[] players = new Player[2];

    public bool Travelling = false;
    public bool Interacting = false;



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
        LogWindow = logWindow;
        LogAnsWindow = logAnsWindow;
        MapType initType = MapType.CLASSIC; //(MapType)Random.Range(0, Enum.GetValues(typeof(MapType)).Length);
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
        SetOptions();

        //Rozpoczecie gry
        activePlayer = 0;
        hexGameUI.Highlighting(true);
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
        players[activePlayer].HexUnit.Location.DisableHighlight();
        activePlayer = (activePlayer + 1) % 2;

        players[activePlayer].HexUnit.Location.EnableHighlight(Color.cyan);
        players[activePlayer].HexUnit.Speed = players[activePlayer].Character.SpeedValue();
        hexMapCamera.SetCameraPosition(players[activePlayer].HexUnit.Location.Position.x, players[activePlayer].HexUnit.Location.Position.z, players[activePlayer].HexUnit.Location.Position);

        hexGameUI.SetSelectedUnit(players[activePlayer].HexUnit);
    }

    public void LevelUp()
    {
        //Koniec gry :/
        if (players[activePlayer].Character.getLevel() >= winningLevel)
            StartCoroutine(hexGameUI.showFinished());
    }

    public void NextRound()
    {
        hexGrid.mapGenerator.SetLandscape(mapPicker.MapChoice);
        hexGrid.CreateMap();
        InitializePlayerUnit();

        hexGameUI.Highlighting(true);
        //Zaczyna ten, ktory nie dotarl do teleportu, czyli nastepny gracz po prostu
        foreach (GameObject b in EndOfTurnButton)
        {
            if (b.active)
                b.SetActive(false);
            else
                b.SetActive(true);
        }
        NextPlayer();
    }

    public bool IsActiveUnit(HexUnit unit)
    {
        return players[activePlayer].HexUnit == unit;
    }

    public void OnFinish(HexUnit unit)
    {
        players[activePlayer].Character.LevelUp();
        mapPicker.ShowPicker(hexGrid.mapGenerator.GetLandscapeType()); //NextRound() is caled inside MapPicker
        hexGameUI.Highlighting(false);
    }

    public void SetOptions()
    {
        players[0].Character.SetCharacterName(OptionSetup.firstName);
        players[1].Character.SetCharacterName(OptionSetup.secondName);
        winningLevel = OptionSetup.maxLevel;
    }

}