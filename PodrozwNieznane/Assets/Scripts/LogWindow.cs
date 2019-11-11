using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogWindow : MonoBehaviour
{
    [SerializeField] Text StatNameText;
    [SerializeField] Text StatModifiersLabelText;
    [SerializeField] Text StatModifiersText;

    private StringBuilder stringBuilder = new StringBuilder();

    private void OnValidate()
    {
        gameObject.SetActive(false);
    }
}
