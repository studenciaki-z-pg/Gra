using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Input firstName;
    [SerializeField] Input secondName;
    [SerializeField] Input maxLevel;

    [SerializeField] GameObject instruction;
    [SerializeField] GameObject options;

    private GameObject menu;

    void Start()
    {
        //Main menu
        ShowInstruction(false);
        ShowOptions(false);
    }

    #region Menu

    //loads inputted level
    public void LoadLevel(string level)
    {
        SetOptions();
        SceneManager.LoadScene(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowInstruction(bool state)
    {
        instruction.SetActive(state);
    }

    public void ShowOptions(bool state)
    {
        options.SetActive(state);
    }

    public void SetOptions()
    {
        OptionSetup.firstName = "Pierwszy gracz";//firstName.ToString();
        OptionSetup.secondName = "Drugi gracz";//secondName;
        OptionSetup.maxLevel = 10;//maxLevel;
    }

    #endregion

}
