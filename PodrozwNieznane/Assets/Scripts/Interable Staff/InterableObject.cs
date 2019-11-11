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
    public int FinallySomeoneFoundMe()
    {
        Debug.Log("Your princess is in another monster dungeon.");
        return 1;
    }

    

}
