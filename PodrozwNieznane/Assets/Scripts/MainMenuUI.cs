using Unity.UNetWeaver;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Assets.Scripts;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject instruction;
    [SerializeField] private Text HowToPlay;
    [SerializeField] private Text Interface;
    [SerializeField] private Text Controls;
    [SerializeField] GameObject options;


    void Start()
    {
        //Main menu
        ShowInstruction(false);
        Controls.enabled = false;
        Interface.enabled = false;
        ShowOptions(false);

        //options

    }

    #region Menu

    //loads inputted level
    public void LoadLevel(string level)
    {
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

    public void ShowMenu()
    {
        instruction.SetActive(false);
        options.SetActive(false);
    }

    #endregion

    #region instruction

    public void ShowHowToPlay()
    {
        Controls.enabled = false;
        Interface.enabled = false;
        HowToPlay.enabled = true;
    }
    public void ShowInterface()
    {
        Controls.enabled = false;
        Interface.enabled = true;
        HowToPlay.enabled = false;
    }
    public void ShowControls()
    {
        Controls.enabled = true;
        Interface.enabled = false;
        HowToPlay.enabled = false;
    }

    #endregion

    #region options


    public void SetName1(InputField text)
    {
        OptionSetup.firstName = text.text;
    }


    public void SetName2(InputField text)
    {
        OptionSetup.secondName = text.text;
    }


    public void SetLevelCap(InputField text)
    {
        OptionSetup.maxLevel = int.Parse(text.text);
    }

    #endregion
}