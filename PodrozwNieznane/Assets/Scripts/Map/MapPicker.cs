using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPicker : MonoBehaviour
{

    private MapType localMapChoice = MapType.CLASSIC;

    public MapType MapChoice { get; set; }


    private void OnValidate()
    {
        gameObject.SetActive(false);
    }

    public void ShowPicker(MapType initValue)
    {
        gameObject.SetActive(true);

        //Set checkmark on the initial value:
        string ToggleObjectName = "Map" + ((int)initValue + 1).ToString();
        Toggle ToggleObject = GameObject.Find(ToggleObjectName).GetComponent<Toggle>();
        ToggleObject.isOn = true;
    }

    public void HidePicker()
    {
        gameObject.SetActive(false);
    }

    public void OnConfirm()
    {
        MapChoice = localMapChoice;
        HidePicker();
        GameManager.instance.NextRound();
    }




    public void MapChoose(int i)
    {
        MapChoose((MapType)i);
    }

    public void MapChoose(MapType i)
    {
        localMapChoice = i;
    }

}
