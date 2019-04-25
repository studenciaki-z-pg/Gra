using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    Unit SelectedUnit;
    public HexMap map;
    void Update()
    {

        if(EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject Object = hitInfo.collider.transform.parent.gameObject;

            if ( Object.GetComponent<HexBeh>() != null)
            {
                Over_Hex( Object);
            }
            else if (Object.GetComponent<Unit>() != null)
            {
                 Over_Unit(Object);
            }

            if(Input.GetMouseButtonDown(1))
            {
                Debug.Log("Hitted: " + hitInfo.collider.transform.parent.name);
            }
        }
    }
    void Over_Hex(GameObject Object)
    {
        Debug.Log("Hitted: " + Object.name);
        if (Input.GetMouseButtonDown(0))
        {
            MeshRenderer mr = Object.GetComponentInChildren<MeshRenderer>();

            if (mr.material.color == Color.black)
            {
                mr.material.color = Color.blue;
            }
            else
            {
                mr.material.color = Color.black;
            }  
            if (SelectedUnit != null)
            {
                SelectedUnit.destination = Object.transform.position;
               // Move(Object);
            }
        }
    }
    void Over_Unit(GameObject Object)
    {
        Debug.Log("Hitted Unit: " + Object.name);
        if (Input.GetMouseButtonDown(0))
        {
           SelectedUnit = Object.GetComponent<Unit>();
        }
    }
    private void Move(GameObject Object)
    {
        map.PathMaker((int)map.TileToCoord((int)Object.transform.position.x, (int)Object.transform.position.y).x, (int)map.TileToCoord((int)Object.transform.position.x, (int)Object.transform.position.y).y);
    }
}
