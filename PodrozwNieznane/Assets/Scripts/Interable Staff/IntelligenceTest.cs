using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligenceTest : InterableObject
{
    private void Start()
    {
        //Random r = new Random();
        int[] playersLevels = new int[2]; //ta nazwa tablicy dużo nie mówi

        //jak nie pisać:
        /*if (Object.ReferenceEquals(GameManager.instance.players[0].Character.getLevel(), null))
            players[0] = 1;
        else
            players[0] = GameManager.instance.players[0].Character.getLevel();
        if (Object.ReferenceEquals(GameManager.instance.players[1].Character.getLevel(), null)) //użycie takiej funkcji to troche przesada skoro i tak porównujesz do null
            players[1] = 1;
        else
            players[1] = GameManager.instance.players[1].Character.getLevel();*/

        //jak napisać to samo krócej i przejrzyściej:
        /*for (int i=0; i < players.Length; i++)
        {
            players[i] = GameManager.instance.players[i].Character.getLevel() ?? 1;
        }*/

        //porównywanie inta do nulla nie ma sensu, bo 'int' nigdy nie może być null (za to 'int?' już może)
        //w ogóle nie wiem o co tu chodzi skoro Character.PlayerLevel jest zainicjowane jako 1
        for (int i=0; i < playersLevels.Length; i++)
        {
            playersLevels[i] = GameManager.instance.players[i].Character.getLevel();
        }

        float average = (playersLevels[0] + playersLevels[1]) / 2.0f; //fajnie że tu był float, ale wynik dzielenia i tak był intem :P
        value = Random.Range((int)average * 5, (int)average * 10);

    }
override
    public int FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;
        if (GameManager.instance.players[active].Character.Intelligence.Value > value)
        {
            GameManager.instance.LogWindow.SendLog("Gratulacje!\nPokonałeś tego maga.\nZdobywasz poziom!");
            GameManager.instance.players[active].Character.LevelUp();
            return 0;
        }
        else
        {
            GameManager.instance.LogWindow.SendLog("Niestety twoja inteligencja jest zbyt niska.\nWymagana inteligencja: " + (value + 1));
            return 1;
        }
    }
}
