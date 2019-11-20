using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    HexCell location, currentTravelLocation;
    float orientation;
    List<HexCell> pathToTravel;

    const float travelSpeed = 3f;
    const float rotationSpeed = 240f;
    const int visionRange = 3;

    public HexGrid Grid { get; set; }
    public static HexUnit unitPrefab;

    public static int initSpeed = 7;
    public bool state = false;

    #region Properties

    public int Speed { get; set; } = initSpeed;

    public bool Travelling { get; set; } = false;


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


    public int VisionRange
    {
        get
        {
            return visionRange;
        }
    }

    #endregion

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
        this.Travelling = true;
        Vector3 a, b, c = pathToTravel[0].Position;
        if(pathToTravel.Count>1) yield return LookAt(pathToTravel[1].Position);
        else yield return LookAt(pathToTravel[0].Position);

        Grid.DecreaseVisibility(
            currentTravelLocation ? currentTravelLocation : pathToTravel[0], 
            visionRange);

        float t = Time.deltaTime * travelSpeed;

        //move unit (with updating visibility)
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
            Speed -= GetMoveCost(pathToTravel[i - 1], pathToTravel[i]);

        }
        currentTravelLocation = null;


        //increase visibility at unit's final position:
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

        //cleanup:
        transform.localPosition = location.Position;
        orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(pathToTravel);
        pathToTravel = null;

        this.Travelling = false;
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
        if (edgeType == HexEdgeType.Slope) return 3 ;
        return 1;
    }

    public IEnumerator Action(HexCell dest)
    {
        //czekaj az pionek sie skonczy ruszac
        yield return new WaitUntil(() => Travelling == false);

        //przygotuj sciezke
        var pastLocation = Location;
        Grid.FindPath(Location, dest, this);
        Travel(Grid.GetPath(this));
        

        //czekaj az sie ruszy
        yield return new WaitUntil(() => Travelling == false);
        Grid.ClearPath();

        //Interakcja
        InteractWithSurroundings(dest);
        if (state)
        {
            Grid.ClearPath();
            GameManager.instance.hexGameUI.HighlightPlayer(true);
        }
        else
        {
            Grid.ClearPath();
            //Odwrot
            Grid.FindPath(Location, pastLocation, this);
            Travel(Grid.GetPath(this));
        }

        yield return new WaitUntil(() => Travelling == false);

        Grid.ClearPath();
        GameManager.instance.hexGameUI.HighlightPlayer(true);

    }

    /// <summary>
    /// Called when HexUnit reaches its destination HexCell, and while travelling
    /// </summary>
    void InteractWithSurroundings(HexCell surroundings)
    {
        if (surroundings.ItemLevel == -1)
        {
            GameManager.instance.OnFinish(this);
        }

        //Udalo sie wykonac akcje
        else if (surroundings.interableObject.FinallySomeoneFoundMe() == 0)
        {
            surroundings.ItemLevel = 0;
            state = true;
        }

        //Nie udalo sie wykonac akcji
        else if (surroundings.interableObject.FinallySomeoneFoundMe() == 1)
        {
            state = false;
        }
    }
}

