using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgilityTest : InterableObject
{
    
    private void Start()
    {
        int[] playersLevels = new int[2]; 
        //ta nazwa tablicy dużo nie mówi
       /* Random r = new Random();
        int[] players = new int[2];
        if (Object.ReferenceEquals(GameManager.instance.players[0].Character.getLevel(), null))
            players[0] = 1;
        else
            players[0] = GameManager.instance.players[0].Character.getLevel();
        if (Object.ReferenceEquals(GameManager.instance.players[1].Character.getLevel(), null))
            players[1] = 1;
        else
            players[1] = GameManager.instance.players[1].Character.getLevel();*/
        for (int i = 0; i < playersLevels.Length; i++)
        {
            playersLevels[i] = GameManager.instance.players[i].Character.getLevel();
        }

        float average = (playersLevels[0] + playersLevels[1]) / 2.0f;
        value = Random.Range((int)average * 5, (int)average * 10);

    }

    override
    public int FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;
        if (GameManager.instance.players[active].Character.Agility.Value > value)
        {
            GameManager.instance.LogWindow.SendLog("Gratulacje!\nPokonałeś tego łotrzyka.\nZdobywasz poziom!");
            GameManager.instance.players[active].Character.LevelUp();
            return 0;
        }
        else
        {
            GameManager.instance.LogWindow.SendLog("Niestety twoja zręczność jest zbyt niska.\nWymagana zręczność: " + value+1);
            return 1;
        }
    }
}