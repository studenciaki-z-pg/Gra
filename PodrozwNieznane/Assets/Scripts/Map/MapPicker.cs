using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPicker : MonoBehaviour
{

    private int localMapChoice;


    public int MapChoice { get; set; } = 5;


    private void OnValidate()
    {
        gameObject.SetActive(false);
    }

    public void ShowPicker()
    {
        gameObject.SetActive(true);
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
        localMapChoice = i;
    }

}
