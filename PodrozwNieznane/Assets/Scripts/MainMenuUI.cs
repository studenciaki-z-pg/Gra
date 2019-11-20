using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject instruction;
    [SerializeField] private Text HowToPlay;
    [SerializeField] private Text Interface;
    [SerializeField] private Text Controls;
    [SerializeField] GameObject options;

    private GameObject[] instructions;

    void Start()
    {
        //Main menu
        ShowInstruction(false);
        Controls.enabled = false;
        Interface.enabled = false;
        ShowOptions(false);
    }

    #region Menu

    //loads inputted level
    public void LoadLevel(string level)
    {
       // GameManager.instance.SetOptions();
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



}