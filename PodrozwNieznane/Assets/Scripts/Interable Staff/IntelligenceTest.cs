using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligenceTest : InterableObject
{
    private void Start()
    {
        System.Random r = new System.Random();
        int[] players = new int[2]; //ta nazwa tablicy dużo nie mówi

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
        for (int i=0; i < players.Length; i++)
        {
            players[i] = GameManager.instance.players[i].Character.getLevel();
        }

        float average = (players[0] + players[1]) / 2.0f; //fajnie że tu był float, ale wynik dzielenia i tak był intem :P
        value = r.Next((int)average * 5, (int)average * 10);

    }
override
    public int FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;
        if (GameManager.instance.players[active].Character.Intelligence.Value > value)
        {
            Debug.Log("Gratulacje! Pokonałeś tego maga. Zdobywasz poziom!");
            GameManager.instance.players[active].Character.LevelUp();
            return 0;
        }
        else
        {
            Debug.Log("Niestety twoja inteligencja jest zbyt niska by pokonać tego wojownika. Wymagana inteligencja: " + value);
            return 1;
        }
    }
}
