using UnityEngine;
using UnityEngine.UI;


public class StatPanel : MonoBehaviour
{
    [SerializeField] Text CharacterName;
    [SerializeField] Text Level;
    [SerializeField] StatDisplay[] statDisplays;
    [SerializeField] string[] statNames;

    private CharacterStats[] stats;

    private void OnValidate()
    {
        statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
    }

    public void SetStats(params CharacterStats[] characterStats)
    {
        stats = characterStats;
        if(characterStats.Length > statDisplays.Length)
        {
            Debug.LogError("Nie wystarczająca ilość stat slotów");
            return;
        }

        for(int i = 0; i < stats.Length; i++)
        {
            statDisplays[i].gameObject.SetActive(i < stats.Length);

            if (i < stats.Length)
                statDisplays[i].Stat = stats[i];
        }
    }

    public void SetCharacterName(string newName)
    {
        CharacterName.text = newName;
    }

    public void SetCharacterLevel(int newLevel)
    {
        Level.text = "Poziom: "+newLevel.ToString();
    }

    public void  UpdateStatValues()
    {
        for(int i = 0; i<stats.Length; i++)
        {
            statDisplays[i].UpdateStatValue();
        }
    }

    public void UpdateStatNames()
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statDisplays[i].Name = statNames[i];
        }
    }

}
