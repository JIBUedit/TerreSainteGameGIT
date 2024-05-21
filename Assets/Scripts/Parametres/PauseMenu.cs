using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject settingsWindows;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }

    void Paused()
    {
        TabinAttack.instance.enabled = false;
        PlayerMovement.instance.enabled = false;
        // activer notre menu pause / l'afficher
        pauseMenuUI.SetActive(true);
        // arrêter le temps
        Time.timeScale = 0;
        // changer le statut du jeu
        gameIsPaused = true;
    }

    public void Resume()
    {
        TabinAttack.instance.enabled = true;
        PlayerMovement.instance.enabled = true;
        // activer notre menu pause / l'afficher
        pauseMenuUI.SetActive(false);
        // arrêter le temps
        Time.timeScale = 1;
        // changer le statut du jeu
        gameIsPaused = false;
    }

    public void OpenSettingsWindow()
    {
        settingsWindows.SetActive(true);
    }

    public void CloseSettingsWindow()
    {
        settingsWindows.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}