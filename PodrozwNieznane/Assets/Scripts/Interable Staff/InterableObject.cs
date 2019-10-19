using UnityEngine;

public class InterableObject : MonoBehaviour
{


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
