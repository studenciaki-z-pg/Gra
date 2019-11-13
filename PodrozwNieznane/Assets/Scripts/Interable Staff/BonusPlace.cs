using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPlace : InterableObject
{
    private void Start()
    {

    }
    override
        public int FinallySomeoneFoundMe()
    {
        int active = GameManager.instance.activePlayer;

        int value = Random.Range(1, 6);
        GameManager.instance.players[active].Character.BonusSkillPoints(value);

        switch (Random.Range(1, 5))
        {
            case 1:
                for (int i = 0; i < value; i++)
                    GameManager.instance.players[active].Character.AddAgilityPoint();
                GameManager.instance.LogWindow.SendLog("Znajdujesz kapliczkę zręczności.\nTwoja zręczność zwiększa się o: " + value);
                break;
            case 2:
                for (int i = 0; i < value; i++)
                    GameManager.instance.players[active].Character.AddIntelligencePoint();
                GameManager.instance.LogWindow.SendLog("Znajdujesz kapliczkę zręczności.\nTwoja zręczność zwiększa się o: " + value);
                break;
            case 3:
                for (int i = 0; i < value; i++)
                    GameManager.instance.players[active].Character.AddStrengthPoint();
                GameManager.instance.LogWindow.SendLog("Znajdujesz kapliczkę zręczności.\nTwoja zręczność zwiększa się o: " + value);
                break;
            case 4:
                for (int i = 0; i < value; i++)
                    GameManager.instance.players[active].Character.AddVitalityPoint();
                GameManager.instance.LogWindow.SendLog("Znajdujesz kapliczkę zręczności.\nTwoja zręczność zwiększa się o: " + value);
                break;
        }
        return 0;
    }
}
