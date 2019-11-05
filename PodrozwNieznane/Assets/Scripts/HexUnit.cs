using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class HexUnit : MonoBehaviour
{
    HexCell location, currentTravelLocation;
    float orientation;
    List<HexCell> pathToTravel;

    const float travelSpeed = 3f;
    const float rotationSpeed = 160f;
    const int visionRange = 3;

    public HexGrid Grid { get; set; }
    public static HexUnit unitPrefab;

    public int speed = 7;
    public int Speed
    {
        get => speed;
        set
        {
            if (speed != value)
                speed = value;
        }
    }

    public int VisionRange
    {
        get
        {
            return visionRange;
        }
    }

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
            if (currentTravelLocation)
            {
                Grid.IncreaseVisibility(location, visionRange);
                Grid.DecreaseVisibility(currentTravelLocation, visionRange);
                currentTravelLocation = null;
            }
        }
    }

    IEnumerator TravelPath()
    {
        Vector3 a, b, c = pathToTravel[0].Position;
        if(pathToTravel.Count>1) yield return LookAt(pathToTravel[1].Position);
        else yield return LookAt(pathToTravel[0].Position);

        Grid.DecreaseVisibility(
            currentTravelLocation ? currentTravelLocation : pathToTravel[0], 
            visionRange);

        float t = Time.deltaTime * travelSpeed; 

        for (int i = 1; i < pathToTravel.Count; i++)
        {
            currentTravelLocation = pathToTravel[i];
            a = c;
            b = pathToTravel[i - 1].Position;
            c = (b + currentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisibility(pathToTravel[i], visionRange);
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            Grid.DecreaseVisibility(pathToTravel[i], visionRange);
            t -= 1f;

            //Some glorious movement magic
            //Speed -= GetMoveCost(pathToTravel[i - 1], pathToTravel[i]);

        }
        currentTravelLocation = null;

        a = c;
        b = location.Position;
        c = b;
        Grid.IncreaseVisibility(location, visionRange);
        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        //Debug.Log(Grid.GetCell(location.Position).ItemLevel); // Milan interakcja ->>>>>>>>>>>>> przenies do glorious movement magic (up)
        //                                                                                          bo ten fragment kodu odpala się po calym ruchu a nie w trakcie,
        //                                                                                           chyba, ze tak mialo byc.
        if(Grid.GetCell(location.Position).ItemLevel != 0)
        {
            Grid.GetCell(location.Position).interableObject.FinallySomeoneFoundMe();
            Grid.GetCell(location.Position).ItemLevel = 0;
        }


        transform.localPosition = location.Position;
        orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;
        
    }

    IEnumerator LookAt(Vector3 point)
    {
        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);
        if(angle > 0f)
        {
            float speed = rotationSpeed / angle;

            for (float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed)
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }
        

        transform.LookAt(point);
        orientation = transform.localRotation.eulerAngles.y;
    }


    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                //location.DecreaseVisibility();
                Grid.DecreaseVisibility(location, visionRange);
                location.Unit = null;
            }
            location = value;
            value.Unit = this;

            //value.IncreaseVisibility();
            Grid.IncreaseVisibility(value, visionRange);

            transform.localPosition = value.Position;
        }
    }
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }
    public void Die()
    {
        if (location)
        {
            //location.DecreaseVisibility();
            Grid.DecreaseVisibility(location, visionRange);
        }
        location.Unit = null;
        Destroy(gameObject);
    }

    public void Travel(List<HexCell> path)
    {
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    public int GetMoveCost(HexCell fromCell, HexCell toCell)//dodac zaleznosc od speed
    {
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
        if (edgeType == HexEdgeType.Cliff) return -1;
        if (edgeType == HexEdgeType.Slope) return 3;
        return 1;
    }



}

