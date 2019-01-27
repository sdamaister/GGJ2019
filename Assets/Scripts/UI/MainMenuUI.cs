using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// UI.Text.text example
//
// A Space keypress changes the message shown on the screen.
// Two messages are used.
//
// Inside Awake a Canvas and Text are created.

public class MainMenuUI : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Credits;

    void Awake()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        Credits.SetActive(false);
    }

    public void ShowCredits()
    {
        MainMenu.SetActive(false);
        Credits.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene("JoinScene", LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}