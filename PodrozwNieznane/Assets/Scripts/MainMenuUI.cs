using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private GameObject instruction;
    private GameObject menu;

    void Start()
    {
        //Main menu
        instruction = GameObject.FindWithTag("Instruction");
        ShowInstruction(false);
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

    #endregion

}
