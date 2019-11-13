using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthTest : InterableObject
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
    public int FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;
        if (GameManager.instance.players[active].Character.Strength.Value > value)
        {
            GameManager.instance.LogWindow.SendLog("Gratulacje!\nPokonałeś tego wojownika.\nZdobywasz poziom!");
            GameManager.instance.players[active].Character.LevelUp();
            return 0;
        } else
        {
            GameManager.instance.LogWindow.SendLog("Niestety twoja siła jest zbyt niska by pokonać tego wojownika.\nWymagana siła: " + value);
            return 1;
        }
    }
}
