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
        GameManager.instance.ActivateBackground();
    }

    public void ShowLog()
    {
        gameObject.SetActive(true);
        GameManager.instance.DeactivateBackground();
    }

    public void SendLog(string s)
    {
        LogText.text = s;
        ShowLog();
    }

}
