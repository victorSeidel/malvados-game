using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenu;
    public void LoadGame(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }

    public void TooglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}


