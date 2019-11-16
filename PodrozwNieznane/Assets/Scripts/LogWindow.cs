﻿using System.Text;
using Unity.UNetWeaver;
using UnityEngine;
using UnityEngine.UI;

public class LogWindow : MonoBehaviour
{
    [SerializeField] Text LogText;

    private StringBuilder stringBuilder = new StringBuilder();

    private void OnValidate()
    {
        gameObject.SetActive(false);
    }

    public void HideLog()
    {
        gameObject.SetActive(false);
        GameManager.instance.hexGameUI.HighlightPlayer(true);
        
    }

    public void ShowLog()
    {
        GameManager.instance.hexGrid.ClearPath();
        GameManager.instance.hexGameUI.HighlightPlayer(false);
        gameObject.SetActive(true);
    }

    public void SendLog(string s)
    {
        LogText.text = s;
        ShowLog();
    }
}
