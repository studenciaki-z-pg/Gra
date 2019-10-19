using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgilityTest : InterableObject
{
    override
    public void FinallySomeoneFoundMe()
    {
        Debug.Log("It's your Agility Challenge");
    }
    override
    public void FinallySomeoneFoundMe(Character character)
    {
        Debug.Log("It's your Agility Challenge but your skill power is only: " + character.Agility.Value);
    }
}
