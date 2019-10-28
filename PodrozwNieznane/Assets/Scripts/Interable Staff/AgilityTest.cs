using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgilityTest : InterableObject
{
    
    private void Start()
    {
        System.Random r = new System.Random();
        int[] players = new int[2];
        if (Object.ReferenceEquals(GameManager.instance.players[0].Character.getLevel(), null))
            players[0] = 1;
        else
            players[0] = GameManager.instance.players[0].Character.getLevel();
        if (Object.ReferenceEquals(GameManager.instance.players[1].Character.getLevel(), null))
            players[1] = 1;
        else
            players[1] = GameManager.instance.players[1].Character.getLevel();

        float average = (players[0] + players[1]) / 2;
        value = r.Next((int)average * 5, (int)average * 10);

    }

    override
    public void FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;
        if (GameManager.instance.players[active].Character.Agility.Value > value)
        {
            Debug.Log("Gratulacje! Pokonałeś tego łotrzyka. Zdobywasz poziom!");
            GameManager.instance.players[active].Character.LevelUp();
        }
        else
        {
            Debug.Log("Niestety twoja zręczność jest zbyt niska by pokonać tego łotrzyka. Wymagana zręczność: " + value);
        }
    }
}
