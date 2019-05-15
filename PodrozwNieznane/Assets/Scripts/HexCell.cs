using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;
    [SerializeField]
    HexCell[] neighbors;
    int elevation = int.MinValue;


    public int SearchPhase { get; set;}
    public HexCell PathFrom { get; set; }
    public int SearchHeuristic { get; set; }
    public int SearchPriority
    {
        get { return distance + SearchHeuristic; }
    }
    public HexCell NextWithSamePriority { get; set; }



    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
    void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            Vector3 position = transform.localPosition;
            //Adding noise to our elevation
            position.y = value * HexMetrics.elevationStep *
            (HexMetrics.SampleNoise(position).y * 2f) *
            HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;
            Refresh();
        }
    }
    public Color Color { 
        get {
			return color;
		}
        set {
			if (color == value) {
				return;
			}
			color = value;
			Refresh();
		}
	}

	Color color;



    //Definig height of an elevation based on noise
    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(
            elevation, neighbors[(int)direction].elevation
        );
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(
            elevation, otherCell.elevation
        );
    }


    //--------------------
    //distance for pathfinding:

    int distance;
    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            //UpdateDistanceLabel();

        }
    }
    /*void UpdateDistanceLabel()
    {
        Text label = uiRect.GetComponent<Text>();
        label.text = distance == int.MaxValue ? "" : distance.ToString();
    }*/
    public void SetLabel (string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }


    public void DisableHighlight () {
		Image highlight = uiRect.GetChild(0).GetComponent<Image>();
		highlight.enabled = false;
	}
    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }


    //--------------------
    //cuboid features:

    int urbanLevel, farmLevel, plantLevel;
    public int UrbanLevel
    {
        get
        {
            return urbanLevel;
        }
        set
        {
            if (urbanLevel != value)
            {
                urbanLevel = value;
                RefreshSelfOnly();
            }
        }
    }
    public int FarmLevel
    {
        get
        {
            return farmLevel;
        }
        set
        {
            if (farmLevel != value)
            {
                farmLevel = value;
                RefreshSelfOnly();
            }
        }
    }
    public int PlantLevel
    {
        get
        {
            return plantLevel;
        }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
                RefreshSelfOnly();
            }
        }
    }
    //end
    //-------------------

    public void EditItself() //a copy of HexMapEditor.EditCell(HexCell cell), a very crude randomization
    {
        if (this)
        {
            //this.Color = HexMapEditor.colors[Random.Range(0, 4)]; //unaccessible:(
            Color[] coles = { new Color(0.26f, 0.87f, 0.20f), new Color(0.62f, 0.23f, 0.05f),
                new Color(0.16f, 0.45f, 0.86f), new Color(0.88f, 0.94f, 0.91f) };

            this.Color = coles[Random.Range(0, 4)];//int (int [inclusive], int [exlusive]) (max returned value = 3)
            this.Elevation = Random.Range(1, 4); //(0, 7); 
            this.UrbanLevel = (int)UnevenRandom(0f, 3.99f); 
            this.FarmLevel = (int)UnevenRandom(0f, 3.99f);
            this.PlantLevel = (int)UnevenRandom(0f, 3.99f); //float (float [inclusive], float [inclusive])

        }
    }
    float UnevenRandom(float from, float to)
    {
        float intermediate = (to + from) / 2;

        float temp = Random.Range(0f, 1f);
        if (temp < 0.75f)
        {
            return 0f;
        }
        if (temp<0.875f)//lower half of interval has 1x the chance of being chosen.
        {
            return Random.Range(from, intermediate);
        }
        else
        {
            return Random.Range(intermediate, to);
        }
    }


}

