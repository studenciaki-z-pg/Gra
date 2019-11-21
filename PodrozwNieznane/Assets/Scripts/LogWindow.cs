using System.Text;
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
        GameManager.instance.hexGameUI.Highlighting(true);
        GameManager.instance.Interacting = false;


    }

    public void ShowLog()
    {
        GameManager.instance.Interacting = true;
        GameManager.instance.hexGameUI.Highlighting(false);
        gameObject.SetActive(true);
    }

    public void SendLog(string s)
    {
        LogText.text = s;
        ShowLog();
    }
}
