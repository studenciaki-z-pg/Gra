using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour
{

    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;
    public InterableObject interableObject;

    [SerializeField]
    HexCell[] neighbors;
    [SerializeField] public
    InterableObject interableObjectPrefab;
    [SerializeField] public
    ItemChest ItemChestPrefab;
    [SerializeField] public 
    StrengthTest StrengthTestPrefab;
    [SerializeField] public 
    IntelligenceTest IntelligenceTestPrefab;
    [SerializeField] public 
    AgilityTest AgilityTestPrefab;

    int elevation = int.MinValue;
    int terrainTypeIndex = 0;
    int visibility;
    bool explored;

    /// <summary>
    /// Determines if player or item can be placed on HexCell, and if it can become visible
    /// </summary>
    public bool Explorable { get; set; }
    public HexUnit Unit { get; set; }
    public int SearchPhase { get; set;}
    public HexCell PathFrom { get; set; }
    public int SearchHeuristic { get; set; }
    public HexCell NextWithSamePriority { get; set; }
    public HexCellShaderData ShaderData { get; set; }
    public int Index { get; set; }
    /// <summary>
    /// Determines if player or item can be placed on HexCell
    /// </summary>
    public bool Walkable { get; set; }


    public bool IsValidDestination(HexUnit unit)
    {
        return /*!this.IsUnderwater &&*/ this.Walkable && this.IsExplored && !this.Unit;
    }

    public bool IsExplored
    {
        get
        {
            return explored && Explorable;
        }
        private set
        {
            explored = value;
        }
    }

    public int ViewElevation
    {
        get
        {
            return elevation >= waterLevel ? elevation : waterLevel;
        }
    }

    public int SearchPriority
    {
        get { return distance + SearchHeuristic; }
    }
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
            if (Unit)
            {
                Unit.ValidateLocation();
            }
        }
    }
    void RefreshSelfOnly()
    {
        chunk.Refresh();
        if (Unit)
        {
            Unit.ValidateLocation();
        }
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
            int originalViewElevation = ViewElevation;
            elevation = value;
            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }
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

    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                ShaderData.RefreshTerrain(this);
            }
        }
    }


    #region Label and highlight

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

    #endregion

    #region UrbanLevel, ItemLevel, PlantLevel properties

    int urbanLevel, itemLevel, plantLevel;
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
    public int ItemLevel
    {
        get
        {
            return itemLevel;
        }
        set
        {
            if (itemLevel != value)
            {
                itemLevel = value;
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

    #endregion

    #region Water-related properties

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }
    int waterLevel = 0;

    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }

    #endregion

    #region Fog & visibility

    public bool IsVisible
    {
        get
        {
            return visibility > 0 && Explorable;
        }
    }
    public void IncreaseVisibility()
    {
        visibility += 1;
        if (visibility == 1)
        {
            IsExplored = true;
            ShaderData.RefreshVisibility(this);
        }
    }
    public void DecreaseVisibility()
    {
        visibility -= 1;
        if (visibility == 0)
        {
            ShaderData.RefreshVisibility(this);
        }
    }

    public void ResetVisibility()
    {
        if (visibility > 0)
        {
            visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }

    #endregion

}