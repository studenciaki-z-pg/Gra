using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelligenceTest : InterableObject
{
    override
    public void FinallySomeoneFoundMe()
    {
        Debug.Log("It's your Intelligence Challenge");
    }
    override
    public void FinallySomeoneFoundMe(Character character)
    {
        Debug.Log("It's your Intelligence Challenge but your skill power is only: " + character.Intelligence.Value);
    }
}
