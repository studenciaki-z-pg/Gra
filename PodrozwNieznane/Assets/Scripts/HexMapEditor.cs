using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;

    int activeElevation;

    int activeUrbanLevel, activeFarmLevel, activePlantLevel;


    void Awake()
    {
        SetColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }



    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }
    void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.Elevation = activeElevation;
        cell.UrbanLevel = activeUrbanLevel;
        cell.FarmLevel = activeFarmLevel;
        cell.PlantLevel = activePlantLevel;
    }


    public void SetColor(int index)
    {
        activeColor = colors[index];
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }
    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }
    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }
    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }


}