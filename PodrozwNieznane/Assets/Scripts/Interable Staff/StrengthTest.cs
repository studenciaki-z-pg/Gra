using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrengthTest : InterableObject
{
    override
    public void FinallySomeoneFoundMe()
    {
        Debug.Log("It's your Strength Challenge");
    }
    override
    public void FinallySomeoneFoundMe(Character character)
    {
        Debug.Log("It's your Strength Challenge but your skill power is only: " + character.Strength.Value);
    }
}
