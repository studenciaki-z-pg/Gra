using UnityEngine;

public class InterableObject : MonoBehaviour
{
    protected int value;

    public InterableObject()
    {
        value = 0;
    }

    public void Activate()
    {

    }

    virtual
    public void FinallySomeoneFoundMe()
    {
        Debug.Log("Your princess is in another monster dungeon.");
    }


    virtual
    public void FinallySomeoneFoundMe(Character character)
    {
        Debug.Log("Your princess is in another monster dungeon.");
    }
    

}
