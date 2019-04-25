using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Unit : MonoBehaviour
{
    public Vector3 destination;
    readonly float speed = 2f;
    public HexMap map;
    public int tileX;
    public int tileY;

    public List<Node> currentPath;
    private void Start()
    {
        destination = transform.position;
    }

    private void Update()
    {
        /*if (currentPath != null)
        {
            int currNode = 0;
            while (currNode < currentPath.Count-1)
            {
                Vector3 start = map.TileToCoord(currentPath[currNode].x, currentPath[currNode].y) + new Vector3(0,1f,0);
                Vector3 end = map.TileToCoord(currentPath[currNode+1].x, currentPath[currNode+1].y) + new Vector3(0, 1f, 0);
                
                Debug.DrawLine(start,end, Color.red);
                currNode++;
            }
        }
        */
        Vector3 dir = destination - transform.position;
        Vector3 vel = dir.normalized * speed * Time.deltaTime;
        
        vel = Vector3.ClampMagnitude(vel, dir.magnitude);
        transform.Translate(vel);
        
        

    }

    void NextTurn()
    {

    }

    /*
    public Unit()
    {
        name = "NoName";
    }

    public float Movement = 2;
    public float movementRemaining = 2;

    Hex destination;

    public void RefreshMovement()
    {
        movementRemaining = Movement;
    }

    public bool DoMove()
    {
        Debug.Log("DoMove");

        if (movementRemaining <= 0)
            return false;

        float movementCost = destination.movementDebuff + 1;

        if (movementCost > movementRemaining && movementRemaining < Movement)
        {
            return false;
        }

        //SetHex(destination);
        movementRemaining = Mathf.Max(movementRemaining - movementCost, 0);
        return movementRemaining > 0;
    }*/


}
